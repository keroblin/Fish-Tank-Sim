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
        ItemBehaviour behaviour;
        if(prefab == null)
        {
            behaviour = PlacementManager.Instance.Place(this,Manager.Instance.itemPool) as ItemBehaviour;
        }
        else
        {
            behaviour = PlacementManager.Instance.Place(this) as ItemBehaviour;
        }
    }
    public override void Remove()
    {
        base.Remove();
        Manager.Instance.currentTank.RemoveModifiers(this);
    }
}
