using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //this is an extremely basic version!
    //I want to make it slerp nicely into position using a public curve to control the movement
    //i also want to use quaternions instead of eulerangles

    public GameObject front;
    public GameObject back;
    public GameObject left;
    public GameObject right;
    public GameObject top;
    public GameObject bottom;

    List<GameObject> sides = new List<GameObject>();

    Camera cam;
    GameObject face;
    public static bool isUp = false;
    void Start()
    {
        cam = Camera.main;
        sides.Add(front);
        sides.Add(back);
        sides.Add (left);
        sides.Add(right);
        sides.Add(top);
        sides.Add(bottom);

        face = front;
        SetClickthrough();
    }

    public void Rot(int dir)
    {
        switch (dir)
        {
            case 1:
            case 2:
                if (isUp)
                {
                    transform.Rotate(0, 0, 45 * dir);
                }
                else
                {
                    transform.Rotate(0, 45 * dir, 0);
                }
                break;
            case 3:
                if (!isUp)
                {
                    transform.Rotate(90, 0, 0);
                    isUp = true;
                    Debug.Log("local cam state " + isUp);
                }
                break;
            case 4:
                if (isUp)
                {
                    transform.Rotate(-90, 0, 0);
                    isUp = false;
                    Debug.Log("local cam state " + isUp);
                }
                break;
        }
        SetClickthrough();
    }

   /* IEnumerator ProgressRotation(Vector3 target)
    {
        while(transform.eulerAngles.magnitude > target.magnitude+.1f)
        {
            Vector3 lerped = Vector3.Lerp(transform.eulerAngles, target, 5.0f*Time.deltaTime);
            transform.Rotate(lerped);
            yield return new WaitForEndOfFrame();
        }
        transform.Rotate(target);
    }*/

    void SetClickthrough()
    {
        face.layer = 9;
        int layerMask = (1 << 9); //tank only
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            face = hit.collider.gameObject;
            //Debug.Log("Ignoring face: " + hit.collider.gameObject.name);
        }
        else
        {
            //Debug.Log("Did not Hit");
        }

        foreach (GameObject go in sides)
        {
            if(go != face)
            {
                go.layer = 9;
            }
            else
            {
                go.layer = 2;
            }
        }
    }
}
