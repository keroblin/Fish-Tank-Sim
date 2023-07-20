using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //this is an extremely basic version!
    //I want to make it slerp nicely into position using a public curve to control the movement
    //i also want to use quaternions instead of eulerangles

    Vector3 initRot;
    Vector3 currentTarget;
    bool isUp = false;
    void Start()
    {
        initRot = transform.eulerAngles;
    }

    public void MoveUp()
    {
        if (!isUp)
        {
            transform.Rotate(90,0,0);
            isUp = true;
        }
    }
    public void MoveDown()
    {
        if (isUp)
        {
            transform.Rotate(-90, 0,0);
            isUp = false;
        }
    }

    public void RotateSide(int dir)
    {
        /*if(transform.eulerAngles.y + (45*dir) > 360)
        {

            transform.Rotate(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        }
        else if (transform.eulerAngles.y + (45*dir) < 0)
        {
            transform.Rotate(transform.eulerAngles.x, 360, transform.eulerAngles.z);
        }
        currentTarget = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y + (90*dir),transform.eulerAngles.z);*/
        if (isUp)
        {
            transform.Rotate(0, 0, 45 * dir);
        }
        else
        {
            transform.Rotate(0, 45 * dir, 0);
        }
        //StopAllCoroutines();
        //ProgressRotation(pos);
    }

    /*IEnumerator ProgressRotation(Vector3 target)
    {
        while(transform.eulerAngles.magnitude > target.magnitude+.1f)
        {
            Vector3 lerped = Vector3.Lerp(transform.eulerAngles, target, 5.0f*Time.deltaTime);
            transform.Rotate(lerped);
            yield return new WaitForEndOfFrame();
        }
        transform.Rotate(target);
    }*/
}
