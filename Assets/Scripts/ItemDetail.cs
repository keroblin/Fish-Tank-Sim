using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDetail : ShopDetail
{
    public TextMeshProUGUI itemPh;
    public TextMeshProUGUI itemLight;
    public TextMeshProUGUI itemTemp;
    public TextMeshProUGUI itemHardness;
    public override void Set(Purchasable purchasable)
    {
        Item item = purchasable as Item;
        itemPh.text = "pH: " + Shop.CheckSetPosNegative(item.pHMod, itemPh) + item.pHMod.ToString();
        itemHardness.text = "Water Hardness: " + Shop.CheckSetPosNegative(item.dGHMod, itemHardness) + item.dGHMod.ToString() + "dGH";
        itemTemp.text = "Temperature: " + Shop.CheckSetPosNegative(item.tempMod, itemTemp) + item.tempMod.ToString() + "f";
        itemLight.text = "Light: " + Shop.CheckSetPosNegative(item.lightMod, itemLight) + (item.lightMod * 100f).ToString() + "%";
    }
}
