using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : ItemList
{
    public Button place;
    public Button sell;

    public override void PurchaseableSetter(PurchasableUI purchasableUI)
    {
        purchasableUI.button.onClick.AddListener(delegate { Select(purchasableUI.purchasable); });
    }

    public override void Select(Purchasable purchasable)
    {
        place.onClick.RemoveAllListeners();
        place.onClick.AddListener(Place);
        sell.onClick.RemoveAllListeners();
        sell.onClick.AddListener(Sell);
        base.Select(purchasable);
    }

    void Place()
    {
        //open placing menu
        //hide our menu
    }

    void Sell()
    {
        //remove from inventory here
        currentMoney += currentPurchasable.price;
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
    }
}
