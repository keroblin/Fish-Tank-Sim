using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : Purchasable
{
    [Header("SUBSTRATE, ORNAMENTS, LIVEPLANTS, HEATING")]
    public float pHMod;
    public float dGHMod;
    public float lightMod;
    public float tempMod;
}
