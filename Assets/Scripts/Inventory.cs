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
        List<Purchasable> updatedInventory = Manager.Instance.inventory;

        if (purchasableUIs.Count > updatedInventory.Count) //if there are more than required, remove them
        {
            List<PurchasableUI> excess;
            if (updatedInventory.Count > 0) 
            {
                excess = purchasableUIs.GetRange(updatedInventory.Count - 1, purchasableUIs.Count - 1);
                foreach (PurchasableUI ui in excess)
                {
                    Debug.Log("Returning " + ui.purchasable.displayName);
                    pool.Return(ui.gameObject);
                }
                purchasableUIs.RemoveRange(updatedInventory.Count - 1, purchasableUIs.Count - 1); //remove any excess objects
            }
            else
            {
                pool.Return(purchasableUIs[0].gameObject);
                purchasableUIs.RemoveAt(0);
            }

            
        }
        else //otherwise add or set them them
        {
            for (int i = 0; i < updatedInventory.Count; i++)
            {
                if (i < purchasableUIs.Count) //if there is an object already
                {
                    purchasableUIs[i].Set(updatedInventory[i]);
                    continue;
                }
                else //if there isnt an object at this index, make a new one
                {
                    GameObject instance = pool.Pull();
                    PurchasableUI ui = instance.GetComponent<PurchasableUI>();
                    ui.Set(updatedInventory[i]);
                    ui.button.onClick.AddListener(delegate { Select(ui.purchasable); });
                    instance.transform.SetParent(categoryUIs[(int)updatedInventory[i].category].transform);
                    purchasableUIs.Add(ui);
                }
            }
        }

        purchasables = updatedInventory;
        UpdateSelection();
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
