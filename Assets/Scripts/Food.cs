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

    public override void Buy()
    {
        base.Buy();

    }
}
