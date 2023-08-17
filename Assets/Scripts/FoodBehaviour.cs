using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoodBehaviour : MonoBehaviour
{
    public Food food;
    public UnityEvent onUsed; //tells feeding to return it
}
