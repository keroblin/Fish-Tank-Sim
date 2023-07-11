using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : ItemList
{
    public Button place;
    //public Button putBack;
    public Button sell;
    public PlacingMenu menu;
    public List<Purchasable> purchasablesPlaced = new List<Purchasable>();
    public Purchasable currentSubstrate;
    public Pool placeablePool;

    public override void OnReady()
    {
        gameObject.SetActive(false);
        menu = Manager.Instance.placingMenu;
        menu.putBack.onClick.AddListener(PutBackPlaceable);
        menu.sell.onClick.AddListener(SellPlaceable);
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
        if (purchasablesPlaced.Contains(purchasable))
        {
            place.interactable = false;
        }
        else
        {
            place.interactable = true;
            place.onClick.AddListener(PlaceButtonClicked);
        }
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
                    if (purchasablesPlaced.Count > 0 && purchasablesPlaced.Contains(purchasables[i]))
                    {
                        ui.button.interactable = false;
                    }
                    else
                    {
                        ui.button.interactable = true;
                        ui.button.onClick.AddListener(delegate { Select(ui.purchasable); });
                    }
                    ui.Set(updatedInventory[i]);
                    instance.transform.SetParent(categoryUIs[(int)updatedInventory[i].category].transform);
                    purchasableUIs.Add(ui);
                }
            }
        }

        purchasables = updatedInventory;
        UpdateSelection();
    }

    void PlaceButtonClicked()
    {
        //open placing menu
        //hide our menu
        if (!purchasablesPlaced.Contains(currentPurchasable))
        {
            purchasableUIs[purchasables.IndexOf(currentPurchasable)].button.interactable = false;
            GameObject obj = placeablePool.Pull();
            Placeable placeable = obj.GetComponent<Placeable>();
            placeable.Set(currentPurchasable);
            placeable.placeableClicked.AddListener(delegate { menu.Select(placeable); });
            Manager.Instance.AddModifiers(currentPurchasable);
        }
    }

    void Sell()
    {
        if (purchasablesPlaced.Contains(currentPurchasable))
        {
            SellPlaceable();
        }
        else
        {
            Manager.Instance.Sell(currentPurchasable);
        }
    }

    public void SellPlaceable()
    {
        Placeable placeable = menu.currentPlaceable;
        placeablePool.Return(placeable.gameObject);
        Manager.Instance.RemoveModifiers(placeable.purchasable);
        Manager.Instance.Sell(placeable.purchasable);
        menu.currentPlaceable = null;
    }
    public void PutBackPlaceable()
    {
        Placeable placeable = menu.currentPlaceable;
        placeablePool.Return(placeable.gameObject);
        purchasablesPlaced.Remove(placeable.purchasable);
        purchasableUIs[purchasables.IndexOf(placeable.purchasable)].button.interactable = true;
        //enable the buttons again
    }
}
