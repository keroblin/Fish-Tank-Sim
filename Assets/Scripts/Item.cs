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

    public override void Place()
    {
        Manager.Instance.currentTank.AddModifiers(this);
        if(prefab == null)
        {
            PlacementManager.Instance.Place(this,Manager.Instance.itemPool);
        }
        else
        {
            PlacementManager.Instance.Place(this);
        }
    }
    public override void Remove()
    {
        base.Remove();
        Manager.Instance.currentTank.RemoveModifiers(this);
    }
}
