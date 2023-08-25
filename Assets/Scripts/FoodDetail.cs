using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodDetail : ShopDetail
{
    public TextMeshProUGUI portionSize;
    public override void Set(Purchasable purchasable)
    {
        Food food = purchasable as Food;
        portionSize.text = "Portion size: " + food.portionSize.ToString() + " /3";
    }
}
