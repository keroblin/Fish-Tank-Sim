using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public Rigidbody rb;

    float length = .5f;
    //Quaternion dir;
    float dir;
    RaycastHit hit;
    Transform target;

    public enum States { IDLE, HIDE, FEED, FIGHT, SMELL};

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hit.point, .05f);
    }
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (rb.SweepTest(transform.forward, out hit, length))
        {
            Debug.Log("Did Hit, in range");
            Vector3 reflection = Vector3.Reflect(transform.position, -hit.point);
            Debug.DrawRay(hit.point, Vector3.Reflect(transform.position, -hit.point) - hit.point, Color.yellow);

            dir = Mathf.Sign( -hit.point.magnitude);
            rb.AddForce(-hit.point - transform.forward * Time.deltaTime * 1f);
            rb.AddTorque(dir * transform.up  * (1f*Time.deltaTime));
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

}
