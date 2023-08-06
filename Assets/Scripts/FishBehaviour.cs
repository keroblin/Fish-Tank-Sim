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
    //Quaternion dir;
    float dir;
    RaycastHit hit;
    GameObject target;

    public enum States { IDLE, HIDE, FEED, FIGHT, SMELL };
    public States state;

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hit.point, .05f);
    }
    void Start()
    {
        Feeding.OnFoodPlaced += FoodPlaced;
    }

    private void FixedUpdate()
    {
        if (rb.SweepTest(transform.forward, out hit, length))
        {
            Debug.Log("Did Hit, in range");
            Vector3 reflection = Vector3.Reflect(transform.position, -hit.point);
            Debug.DrawRay(hit.point, Vector3.Reflect(transform.position, -hit.point) - hit.point, Color.yellow);

            dir = Mathf.Sign(-hit.point.magnitude);
            rb.AddForce(-hit.point - transform.forward * Time.deltaTime * 1f);
            rb.AddTorque(dir * transform.up * (1f * Time.deltaTime));
            rb.AddTorque(dir * transform.right * (1f * Time.deltaTime));
        }
        else
        {
            dir = 0;
            Debug.Log("Did not Hit");
            rb.AddForce(transform.forward * Time.deltaTime * 6f);
            rb.AddTorque(transform.up * (-3f * Time.deltaTime) * dir);
            rb.AddTorque(transform.right * (-3f * Time.deltaTime) * dir);
        }
    }

    public void FoodPlaced(GameObject food)
    {
        if(Vector3.Distance(transform.position, target.transform.position) > Vector3.Distance(transform.position, food.transform.position))
        {
            target = food;
        }
    }

    public void FoodEaten(GameObject food)
    {
        if(target == food)
        {
            target = null;
            state = States.SMELL; //smell state lasts for a while, if it sees other fish that are targeting then it targets their food, otherwise if the timer runs out it goes idle
            //go out of target mode and swim about, if we enter the radius of a food, then target it
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            target = other.gameObject;
            state = States.FEED;
        }
    }
}
