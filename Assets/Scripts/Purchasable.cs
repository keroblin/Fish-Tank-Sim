using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Purchasable", menuName = "ScriptableObjects/Purchasable")]

public class Purchasable : ScriptableObject
{
    public int category;
    public Sprite icon;
    public Mesh model;
    public Material material;
    [Multiline]
    public string description;
    public float price;


}
