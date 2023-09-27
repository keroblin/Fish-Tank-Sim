using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class FishBehaviour : Placeable
{
    public Fish fish;
    public float ageInTicks;
    public Target primaryTarget = new Target();
    public Vector3 currentTargetPosition;
    public bool targetReached;
    public SpriteRenderer alert;

    float length = .5f;
    float speed;
    
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

    public override void Set(Purchasable _purchasable, bool fromPool = false)
    {
        purchasable = _purchasable;
        fish = _purchasable as Fish;
        bounds = Manager.Instance.currentTank.tankBounds;
        Feeding.Instance.OnFoodPlaced += FoodPlaced;
        speed = fish.speed;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case States.IDLE:
                if (primaryTarget == null || primaryTarget.position == Vector3.zero || targetReached) //if we dont have a position, choose a random one
                {
                    Debug.Log("Setting idle pos on " + fish.name);
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
                if (currentFood != null && currentFood.go != null)
                {
                    SetPrimaryTarget(currentFood.go.transform.position, States.FEED);
                }
                else
                {
                    TargetReached();
                }
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
        rb.AddForce(transform.forward * Time.deltaTime * speed); //move forward

        Vector3 cross = Vector3.Cross(transform.forward, (currentTargetPosition - transform.position).normalized); //how much to rotate by
        //steer toward target
        Debug.Log("Steering");

        if (cross.magnitude > 0.01) //if we arent facing the target, turn to face it
        {
            Vector3 target;
            target = cross;
            rb.AddTorque(target);
            rb.AddTorque((transform.up - Vector3.up) * -cross.magnitude); //upright correction, does flips atm
            rb.angularVelocity = Vector3.Lerp(Vector3.zero, rb.angularVelocity, -cross.magnitude); //move more based on distance
            //add stuff to modify it to keep it pointing up here
            //we are rotating the normalised cross product of the distance from the target to us
            //
        }

        //NOT COMPLETE/////////////


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
            rb.AddForce(-rb.velocity * Time.deltaTime * speed);
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
            speed = fish.maxSpeed;
        }
        else if (temp.isLiked)
        {
            speed = fish.speed;
        }
        else
        {
            Debug.Log(fish.name + " didnt like");
            return;
        }
        Debug.Log(fish.name + " liked in some way");
        SetPrimaryTarget(food.transform.position, States.FEED);
        currentFood = temp;
        currentFood.foodBehaviour.Use += RemoveFood;
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
        speed = fish.speed;
        Debug.Log(fish.name + "'s hunger is " + hunger);
    }

    void RemoveFood(FoodBehaviour foodBehaviour)
    {
        foodBehaviour.Use -= RemoveFood;
        currentFood = null;
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
            if(currentFood != null && currentFood.go == collision.gameObject) //maybe change and query the food to check if its liked, maybe do this in foodbehaviour
            {
                Eat(currentFood);
            }
        }
    }

    public void CheckHarmony()
    {
        
    }

    public override void SendOff()
    {
        FishManager.instance.RemoveFish(fish); 
        base.SendOff();
    }
}
