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
    private void Update()
    {
        if (timeInTank < food.rotTime)
        {
            timeInTank += Time.deltaTime;
        }
        else
        {
            //rot, probs turn brown the colour, maybe change the texture and do a particle effect
            timeInTank = -1;
            //spawn a rot particle effect
            Manager.Instance.currentTank.AddRottenFood();
            Destroy(gameObject);
        }
    }
}
