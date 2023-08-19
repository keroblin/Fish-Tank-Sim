using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBehaviour;
using static UnityEngine.GraphicsBuffer;

public class FishBehaviour : MonoBehaviour
{
    public Fish fish;
    public Rigidbody rb;
    public Target primaryTarget = new Target();
    public Vector3 currentTargetPosition;
    public bool targetReached;

    float length = .5f;
    
    bool waiting = false;
    Bounds bounds;

    public FoodData currentFood;
    public float maxHunger;
    public float hunger;
    public float maxHappiness;
    public float happiness;

    public enum States { IDLE, AVOID, HIDE, FEED, SMELL };
    public States state;
    [System.Serializable]
    public class Target //might use, will see
    {
        public Vector3 position;
        public States type;
    }
    public class FoodData
    {
        public GameObject go;
        public FoodBehaviour foodBehaviour;
        public Food food;
        public bool isFavourite;
        public bool isLiked;
        public void Set(GameObject foodObj,Fish fish)
        {
            this.go = foodObj;
            this.foodBehaviour = foodObj.GetComponent<FoodBehaviour>();
            this.food = foodBehaviour.food;
            this.isFavourite = food.favourites.Contains(fish);
            this.isLiked = food.likes.Contains(fish);
        }
    }

    public List<Target> targets = new List<Target>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(currentTargetPosition, .05f);
    }
    void Start()
    {
        bounds = Manager.Instance.tankBounds;
        Feeding.OnFoodPlaced += FoodPlaced;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case States.IDLE:
                if (primaryTarget.position == Vector3.zero || primaryTarget == null || targetReached) //if we dont have a position, choose a random one
                {
                    Debug.Log("Setting idle pos");
                    Vector3 idleTarget = new Vector3(
                       UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x),
                       UnityEngine.Random.Range(-bounds.extents.y, bounds.extents.y),
                       UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z)
                       );
                    idleTarget += bounds.center;
                    SetPrimaryTarget(idleTarget, States.IDLE);
                    targetReached = false;
                }
                break;
            case States.AVOID:
                rb.AddForce(-rb.velocity * Time.deltaTime * 8f);
                break;
            case States.SMELL:
                break;
            case States.FEED:
                SetPrimaryTarget(currentFood.go.transform.position, States.FEED);
                break;
            case States.HIDE:
                break;
            default:
                state = States.IDLE; 
                break;
        }

        Debug.DrawRay(transform.position, transform.forward);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, length);
        if(hits.Length > 0 && !waiting)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.gameObject != gameObject && Vector3.Distance(transform.position, primaryTarget.position) > Vector3.Distance(transform.position,hit.point))
                {
                    state = States.AVOID;
                    currentTargetPosition = hit.normal;
                }
            }
        }
        else if(state == States.AVOID)
        {
            state = primaryTarget.type;
            currentTargetPosition = primaryTarget.position;
        }

        if (!waiting)
        {
            MoveTowardTarget();
        }
    }
    public void SetPrimaryTarget(Vector3 target, States _state)
    {
        //Debug.Log("Set target to " + target)
        primaryTarget.type = _state;
        primaryTarget.position = target;
        currentTargetPosition = target;
    }

    public void MoveTowardTarget()
    {
        Debug.Log("running");
        rb.AddForce(transform.forward * Time.deltaTime * 6f); //move forward

        Vector3 cross = Vector3.Cross(transform.forward, (currentTargetPosition - transform.position).normalized); //how much to rotate by
        //steer toward target
        Debug.Log("Steering");

        if (cross.magnitude > 0.01) //if we arent facing the target, turn to face it
        {
            rb.AddTorque(cross);
            rb.angularVelocity = Vector3.Lerp(Vector3.zero, rb.angularVelocity, cross.magnitude);
        }

        if (Vector3.Distance(transform.position, currentTargetPosition) < .2f) //if we're within distance
        {
            TargetReached();
        }
    }

    public void TargetReached()
    {
        Debug.Log("target reached");
        switch (state)
        {
            case States.IDLE:
                StartCoroutine("WaitAtTarget");
                break;
            case States.FEED:
                StartCoroutine("WaitAtTarget");
                currentFood = null;
                state = States.IDLE;
                //send signal to a fish manager to tell all the other fish that the food has been eaten
                //send to feeding to say that the current food has been eaten and should be returned to pool and reset
                break;
        }
    }

    IEnumerator Brake()
    {
        while (rb.velocity.magnitude > 0f)
        {
            rb.AddForce(-rb.velocity * Time.deltaTime * 6f);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    IEnumerator WaitAtTarget()
    {
        Debug.Log("Waiting at target...");
        waiting = true;
        StartCoroutine("Brake");
        yield return new WaitForSeconds(2f);
        targetReached = true;
        if(primaryTarget != null)
        {
            currentTargetPosition = primaryTarget.position;
        }
        else
        {
            primaryTarget = null;
            state = States.IDLE;
        }
        Debug.Log("Finished wait. Removed target.");
        waiting = false;
        yield return null;
    }

    public void FoodPlaced(GameObject food) //go toward the noise of the placement basically
    {
        FoodData temp = new FoodData();
        temp.Set(food, fish);
        if (temp.isFavourite)
        {
            //go extra fast
        }
        else if (temp.isLiked)
        {
            //go toward it
        }
        else
        {
            //ignore it
            return;
        }
        currentFood = temp;
        SetPrimaryTarget(currentFood.go.transform.position, States.FEED);
        //SetPrimaryTarget(food.transform.position, States.SMELL);
        /*if(Vector3.Distance(transform.position, currentTargetPosition) > Vector3.Distance(transform.position, food.transform.position))
        {
            SetPrimaryTarget(food.transform.position, States.SMELL);
        }*/
    }

    void Eat(FoodData food)
    {
        Debug.Log(fish.name + " is eating " + food.food.name);
        food.foodBehaviour.Use(currentFood.foodBehaviour);
        if(hunger > 0)
        {
            if (food.isFavourite)
            {
                if(happiness < 4)
                {
                    happiness += 1;
                }
            }
           
            if(hunger - food.food.portionSize > 0)
            {
                hunger -= food.food.portionSize;
            }
            else
            {
                hunger = 0;
            }
        }
        Debug.Log(fish.name + "'s hunger is " + hunger);
    }

    private void OnTriggerEnter(Collider other) //when in range of food
    {
        if (other.gameObject.CompareTag("Food"))
        {
            currentFood = new FoodData();
            currentFood.Set(other.gameObject, fish);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if(currentFood.go == collision.gameObject) //maybe change and query the food to check if its liked, maybe do this in foodbehaviour
            {
                Eat(currentFood);
            }
        }
    }
}
