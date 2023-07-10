using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class ShopMenu : ItemList
{
    public Button buy;

    public override void OnReady()
    {
        purchasables = Manager.Instance.allPurchasables;
        base.OnReady();
    }

    public override void PurchaseableSetter(PurchasableUI purchasableUI)
    {
        Debug.Log("Instantiating on shopmenu " + purchasableUI.displayName);
        purchasableUI.button.onClick.AddListener(delegate { Select(purchasableUI.purchasable); });
    }

    public override void Select(Purchasable purchasable)
    {
        buy.onClick.RemoveAllListeners();
        buy.onClick.AddListener(delegate { Manager.Instance.Buy(currentPurchasable); });
        base.Select(purchasable);
    }
}
