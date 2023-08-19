using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoodBehaviour : MonoBehaviour
{
    public Food food;
    public delegate void onUsed(FoodBehaviour food); //tells feeding to return it
    public onUsed Use;
    public float timeInTank;

    IEnumerator RotTimer()
    {
        while (gameObject.activeSelf)
        {
            if(timeInTank < food.rotTime)
            {
                timeInTank += 1;
                yield return new WaitForSeconds(1f);
            }
            else
            {
                //rot, probs turn brown the colour, maybe change the texture and do a particle effect
            }
        }
    }
}
