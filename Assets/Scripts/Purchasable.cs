using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Purchasable", menuName = "ScriptableObjects/Purchasable")]

public class Purchasable : ScriptableObject
{
    public List<Tag> tags;
    public bool stackable = false;
    public bool placeable = true;
    public GameObject prefab;
    public Sprite icon;
    public Mesh model;
    public Material material;
    [Multiline]
    public string description;
    public float price;

    public virtual void Buy()
    {
        Manager.Instance.Buy(this);
    }
    public virtual void Sell()
    {
        Manager.Instance.Sell(this);
    }
    public virtual void Place()//doesn't have to be used if its not placeable
    {
        PlacementManager.Instance.Place(this);
    }
    public virtual void Remove()
    {
        //PlacementManager.Instance.PutBackPlaceable(this);
    }
}
