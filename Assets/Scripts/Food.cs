using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Food", menuName = "ScriptableObjects/Food")]
public class Food : Purchasable
{
    [Range(0, 2)]
    public float portionSize;
    public float rotTime; //time in seconds
    public List<Fish> favourites; //adds some happiness to fish in this list
    public List<Fish> likes; //just fills these fish up
                             //if no fish are present in these, they might smell the food, but then go elsewhere

    public override void Buy()
    {
        base.Buy();

    }
}
