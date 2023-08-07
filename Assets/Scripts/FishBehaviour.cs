using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    /*
     The plan is:
    - fish have a raycast for each direction from their head, or a spherecast
    - when the main forward raycast is hit, that tells it to turn
    - when the front one is free but side ones are hitting, if it is facing the raycast hit too hard then it should turn
    - we should turn in the opposite direction to whichever ones are hit progressively, and move forward
    - fish are all connected to feeding events and should have a target set for the food
    - the main target is the target when the fish are not hitting on their forward cast and are not leaning too far in to the side hits
    - we should store a variable for the avoidance target which is where the fish is aiming to turn to get free until it is free maybe
    - we could maybe have all this work off coroutines acting like states in update
     
     */
    public Rigidbody rb;

    float length = .5f;
    float dirX;
    float dirY;
    RaycastHit hit;
    GameObject targetObj;
    Vector3 targetPosition;
    bool targetReached;
    bool waiting;
    bool avoiding;
    Bounds bounds;
    float breakAmount;

    public enum States { IDLE, HIDE, FEED, FIGHT, SMELL };
    public States state;

    /*List<HitDir> hits = new List<HitDir>();
    class HitDir
    {
        public RaycastHit hit;
        public bool hitting;
        public Vector3 dir;
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hit.point, .05f);
    }
    void Start()
    {
        bounds = Manager.Instance.tankBounds;
        Feeding.OnFoodPlaced += FoodPlaced;

        /*hits.Add(new HitDir());
        hits.Add(new HitDir());
        hits.Add(new HitDir());
        hits.Add(new HitDir());*/
    }

    private void FixedUpdate()
    {
        //int layerMask = (1 << 11) & (1<<0) & (1<<10); //ignore fish layer
        int layerMask = ~(1 << 11);
        if (state == States.IDLE) //if we dont have a target, potter around
        {
            if(targetObj != null)
            {
                targetObj = null;
            }
            if(targetPosition == null) //if we dont have a position, choose a random one
            {
                targetPosition = new Vector3(
                    UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x), 
                    UnityEngine.Random.Range(-bounds.extents.y, bounds.extents.y), 
                    UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z)
                    );
            }
            MoveTowardTarget();
        }

        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward);
        if (Physics.Raycast(transform.position, transform.forward, out hit, length,layerMask))
        {
            if(hit.collider.gameObject != gameObject)
            {
                Debug.Log("Did Hit, in range");
                Avoid(hit);
            }
        }

        //hits[0].dir = transform.TransformDirection(transform.forward);

        //these are like directional checks for the sides, forward should be treated a bit differently though
        /*hits[0].dir = transform.TransformDirection(transform.up);
        hits[1].dir = transform.TransformDirection(-transform.up);
        hits[2].dir = transform.TransformDirection(-transform.right);
        hits[3].dir = transform.TransformDirection(transform.right);

        for (int i= 0; i< 5; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, hits[i].dir, out hit, length, layerMask))
            {
                hits[i].hit = hit;
                Debug.Log("Did Hit, in range");
                hits[i].hitting = true;
                Avoid(hit);
            }
        }*/
    }

    public void Avoid(RaycastHit hit)
    {
        breakAmount = 0;
        dirX = Mathf.Sign(-hit.point.x);
        dirY = Mathf.Sign(-hit.point.y);
        Debug.Log(dirY + ": y");
        Debug.Log(dirX + ": x");
        rb.AddForce(-transform.forward * Time.deltaTime * .5f);
        rb.AddTorque(dirY * transform.up * (1f * Time.deltaTime));
        rb.AddTorque(dirX * transform.right * (1f * Time.deltaTime));
    }

    IEnumerator WaitAtTarget()
    {
        yield return new WaitForSeconds(2f);
        targetReached = true;
        yield return null;
    }
    public void MoveTowardTarget()
    {
        if (!avoiding)
        {
            if (targetObj != null)
            {
                targetPosition = targetObj.transform.position;
            }
            rb.AddForce(transform.forward * Time.deltaTime * 6f);
            breakAmount += .1f * Time.deltaTime;
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, breakAmount); //break the turn
            if (Vector3.Distance(transform.position, targetPosition) < .5f)
            {
                if (state == States.IDLE && !waiting)
                {
                    StartCoroutine("WaitAtTarget");
                }
            }
        }
    }

    public void FoodPlaced(GameObject food)
    {
        if(Vector3.Distance(transform.position, targetObj.transform.position) > Vector3.Distance(transform.position, food.transform.position))
        {
            targetObj = food;
        }
    }

    public void FoodEaten(GameObject food)
    {
        if(targetObj == food)
        {
            targetObj = null;
            state = States.SMELL; //smell state lasts for a while, if it sees other fish that are targeting then it targets their food, otherwise if the timer runs out it goes idle
            //go out of target mode and swim about, if we enter the radius of a food, then target it
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            targetObj = other.gameObject;
            state = States.FEED;
        }
    }
}
