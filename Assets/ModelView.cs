using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelView : MonoBehaviour
{
    public void RotLeft()
    {
        transform.Rotate(0,-45, 0);
    }
    public void RotRight()
    {
        transform.Rotate(0,45, 0);
    }
}
