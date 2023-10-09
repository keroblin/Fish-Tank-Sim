using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FishBehaviour : Placeable
{
    public Fish fish;
    public float ageInTicks;
    public Target primaryTarget = new Target();
    public Vector3 currentTargetPosition;
    public bool targetReached;
    public SpriteRenderer alert;
    public Vector3 alertOffset;
    public Material alertMat;
    int shaderPos = Shader.PropertyToID("_Position");

    float length = .5f;
    float speed;
    
    bool waiting = false;
    Bounds bounds;

    public FoodData currentFood;
    public float hunger = .5f;
    public float happiness = .5f;
    public float harmony = .5f;

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
            this.isFavourite = fish.favouriteFoods.Contains(foodBehaviour.food);
            this.isLiked = !fish.hatedFoods.Contains(foodBehaviour.food);
        }
    }

    public List<Target> targets = new List<Target>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(currentTargetPosition, .05f);
    }

    private void Start()
    {
        alert.transform.parent = null;
        alert.transform.position = Vector3.zero;
        alert.transform.rotation = Quaternion.identity;
        alertMat = alert.material;
    }
    public override void Set(Purchasable _purchasable, bool fromPool = false)
    {
        purchasable = _purchasable;
        fish = _purchasable as Fish;
        //Debug.Log("Set to " + fish.name);
        bounds = Manager.Instance.currentTank.tankBounds;
        Feeding.Instance.OnFoodPlaced += FoodPlaced;
        speed = fish.speed;
        if (fish.dislikedFish.Count < 0)
        {
            harmony = 1;
        }
    }

    private void FixedUpdate()
    {
        alertMat.SetVector(shaderPos, gameObject.transform.position+alertOffset);
        switch (state)
        {
            case States.IDLE:
                if (primaryTarget == null || primaryTarget.position == Vector3.zero || targetReached) //if we dont have a position, choose a random one
                {
                   // Debug.Log("Setting idle pos on " + fish.name);
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
        rb.AddForce(transform.forward * Time.deltaTime * speed); //move forward

        Vector3 cross = Vector3.Cross(transform.forward, (currentTargetPosition - transform.position).normalized); //how much to rotate by
        //steer toward target

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

        //could be improved!/////////////


        if (Vector3.Distance(transform.position, currentTargetPosition) < .2f) //if we're within distance
        {
            TargetReached();
        }
    }

    public void TargetReached()
    {
        //Debug.Log("target reached");
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
        //Debug.Log("Waiting at target...");
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
        //Debug.Log("Finished wait. Removed target.");
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
        //Debug.Log(fish.name + " is eating " + food.food.name);
        food.foodBehaviour.Use(currentFood.foodBehaviour);
        if(hunger > 0)
        {
            if (food.isFavourite)
            {
                if(happiness + .1f < 1)
                {
                    happiness += .1f;
                }
                else
                {
                    happiness = 1;
                }
            }
           
            if(hunger + food.food.portionSize < 1)
            {
                hunger += food.food.portionSize;
            }
            else
            {
                hunger = 1;
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

    public float GetHappiness()
    {
        //i dont like this calculation! i feel like theres a way to do it a bit better
        //if the fish is really hungry, take down their happiness
        //if the fish is really unhappy, take down their hunger faster
        //if the fish dislikes other fish, take down their happiness a lot
        if (fish.CalculateCompat() > 0.6f)
        {
            happiness += .5f;
        }
        else if (fish.CalculateCompat() < 0.3f)
        {
            happiness -= .5f;
        }

        if (harmony > .3f)
        {
            Debug.Log("Harmony above .3");
            happiness += .2f;
        }
        else
        {
            Debug.Log("Harmony below .3");
            happiness -= .1f;
        }

        if (hunger > 0)
        {
            float hungerTarget;
            if (happiness > .8)
            {
                hungerTarget = hunger -= .05f;
            }
            else
            {
                hungerTarget = hunger -= .1f;
            }
            if(hunger - hungerTarget < 0)
            {
                hungerTarget = 0f;
            }
        }

        if (hunger < .2 && happiness > 0)
        {
            happiness -= .1f;
        }


        if (happiness < .25)
        {
            alert.enabled = true;
            alert.sprite = fish.GetCompatIcon(happiness*2);
        }
        else
        {
            alert.enabled = false;
        }

        return happiness;
    }

    public override void SendOff()
    {
        FishManager.instance.RemoveFish(fish);
        Destroy(alert);
        base.SendOff();
    }
}
