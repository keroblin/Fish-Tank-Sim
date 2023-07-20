using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu:ShopCategory
{
    public Button buy;
    public override void ToggleOn(Category category = null)
    {
        base.ToggleOn(category);
        itemList.onSelect.RemoveAllListeners();
        itemList.onSelect.AddListener(Select);
        itemList.UpdateList(Manager.Instance.allPurchasables);
    }
    public override void ToggleOff(Category category = null)
    {
        base.ToggleOff(category);
    }

    public void Select()
    {
        buy.onClick.RemoveAllListeners();
        buy.onClick.AddListener(delegate { Manager.Instance.Buy(itemList.currentPurchasable); });
    }
}
