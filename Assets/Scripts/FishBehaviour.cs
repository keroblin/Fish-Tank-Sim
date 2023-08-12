using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishBehaviour;
using static UnityEngine.GraphicsBuffer;

public class FishBehaviour : MonoBehaviour
{
    /*
     The plan is:
    - movetarget moves the fish to the target 
    - there is a queue of targets
    - avoidances should go on top of these and respond and switch back if the fish is free to move again and not worried about hitting an object
    - feeding should override literally everything else
     */
    public Rigidbody rb;
    public Target primaryTarget = new Target();
    public Vector3 currentTargetPosition;
    public bool targetReached;

    float length = .5f;
    float dirX;
    float dirY;
    RaycastHit hit;
    
    bool waiting = false;
    Bounds bounds;
    float brakeAmount = 1;

    GameObject currentFood;

    public enum States { IDLE, AVOID, HIDE, FEED, SMELL };
    public States state;
    [System.Serializable]
    public class Target //might use, will see
    {
        public Vector3 position;
        public States type;
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
                rb.AddForce(-transform.forward * Time.deltaTime * .5f);
                break;
            case States.SMELL:
                break;
            case States.FEED:
                MoveTowardTarget();
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
                if (hit.collider.gameObject != gameObject)
                {
                    state = States.AVOID;
                    currentTargetPosition = -hit.point;
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
        primaryTarget.type = _state;
        primaryTarget.position = target;
        currentTargetPosition = target;
    }

    public void MoveTowardTarget()
    {
        Debug.Log("running");
        rb.AddForce(transform.forward * Time.deltaTime * 6f); //move forward

        Vector3 cross = Vector3.Cross(transform.forward, (currentTargetPosition - transform.position).normalized); //how much to rotate by
        if (brakeAmount < 1f) //stop turning
        {
            Debug.Log("Braking");
            brakeAmount += .1f * Time.deltaTime;
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, brakeAmount); //break the turn
        }
        else
        {
            //steer toward target
            Debug.Log("Steering");
            rb.AddTorque(cross);
            /* if (cross.magnitude > .05f) //if we arent facing the target, turn to face it
             {
                 rb.AddTorque(cross);
             }
             else //otherwise brake the turn
             {
                 brakeAmount = 0f;
             }*/
        }

        if (Vector3.Distance(transform.position, currentTargetPosition) < .15f) //if we're within distance
        {
            TargetReached();
            //return true;
        }
        else
        {
            //return false;
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
        SetPrimaryTarget(food.transform.position, States.SMELL);
        /*if(Vector3.Distance(transform.position, currentTargetPosition) > Vector3.Distance(transform.position, food.transform.position))
        {
            SetPrimaryTarget(food.transform.position, States.SMELL);
        }*/
    }

    private void OnTriggerEnter(Collider other) //if we reach the food
    {
        if (other.CompareTag("Food"))
        {
            currentFood = other.gameObject;
            SetPrimaryTarget(currentFood.transform.position, States.FEED);
        }
    }
}
