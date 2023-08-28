using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Substrate", menuName = "ScriptableObjects/Substrate")]
public class Substrate : Item
{
    public override Placeable Place()
    {
        Manager.Instance.AddModifiers(this);
        Manager.Instance.SwapSubstrate(this);
        return null;
    }
    public override void Remove()
    {
        Manager.Instance.RemoveModifiers(this);
        Manager.Instance.SwapSubstrate(null);
    }

    public override void Sell()
    {
        base.Sell();
        Remove();
    }
}
