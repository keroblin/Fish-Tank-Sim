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

    public override Placeable Place()
    {
        Manager.Instance.AddModifiers(this);
        Placeable p = PlacementManager.Instance.Place(this);
        return p;
    }
    public override void Remove()
    {
        base.Remove();
        Manager.Instance.RemoveModifiers(this);
    }
}
