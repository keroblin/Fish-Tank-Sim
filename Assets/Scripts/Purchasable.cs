using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Purchasable", menuName = "ScriptableObjects/Purchasable")]

public class Purchasable : ScriptableObject
{
    public Mesh model;
    public string displayName;
    public string description;
    public float price;
    public ShopMenu.Categories category;
    public float pHmod;
    public float dGHMod;
    public float lightMod;
    public float tempMod;
}
