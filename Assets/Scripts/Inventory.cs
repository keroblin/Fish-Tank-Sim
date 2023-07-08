using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : ItemList
{
    public Button place;
    public Button sell;

    public override void OnReady()
    {
        Manager.Instance.onBuy.AddListener(UpdateInv);
        Manager.Instance.onSell.AddListener(UpdateInv);
        UpdateInv();
        base.OnReady();
    }

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

    void UpdateInv()
    {
        purchasables = Manager.Instance.inventory;
        //DELETE ME ASAP //////////////////////////////////
        foreach (PurchasableUI ui in purchasableUIs) //VERY VERY TEMPORARY AND EXPENSIVE JUST TO TEST UNTIL I DO POOLING
        {
            Destroy(ui.gameObject);
        }
        purchasableUIs.Clear();
        base.OnReady();
    }

    void Place()
    {
        //open placing menu
        //hide our menu
    }

    void Sell()
    {
        //remove from inventory here
        Manager.Instance.Sell(currentPurchasable);
        UpdateInv();
        
        //purchasableUIs.Remove(currentPurchasable);
        //need some way to update it here
        money.text = "Your cash: £" + Manager.Instance.currentMoney.ToString("#.00");
    }
}
