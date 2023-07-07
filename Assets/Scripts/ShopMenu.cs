using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class ShopMenu : ItemList
{
    public Button buy;

    public override void PurchaseableSetter(PurchasableUI purchasableUI)
    {
        purchasableUI.button.onClick.AddListener(delegate { Select(purchasableUI.purchasable); });
    }

    public override void Select(Purchasable purchasable)
    {
        buy.onClick.RemoveAllListeners();
        buy.onClick.AddListener(Buy);
        base.Select(purchasable);
    }

    void Buy()
    {
        //make the inventory buy this item
        if (currentMoney-currentPurchasable.price > 0.00)
        {
            currentMoney -= currentPurchasable.price;
            //add to inventory here
            money.text = "Your cash: £" + currentMoney.ToString("#.00");
        }
        else
        {
            print("Not enough money!");
        }
    }
}
