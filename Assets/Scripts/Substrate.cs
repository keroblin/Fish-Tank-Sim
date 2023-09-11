using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Substrate", menuName = "ScriptableObjects/Substrate")]
public class Substrate : Item
{
    public override void Place()
    {
        Manager.Instance.currentTank.AddModifiers(this);
        Manager.Instance.currentTank.SwapSubstrate(this);
    }
    public override void Remove()
    {
        Manager.Instance.currentTank.RemoveModifiers(this);
        Manager.Instance.currentTank.SwapSubstrate(null);
    }

    public override void Sell()
    {
        base.Sell();
        Remove();
    }
}
