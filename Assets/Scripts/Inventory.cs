using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : ShopCategory
{
    public Button place;
    //public Button putBack;
    public Button sell;
    public PlacingMenu menu;
    public List<Purchasable> purchasablesPlaced = new List<Purchasable>();
    public Purchasable currentSubstrate;
    public Pool placeablePool;

    public override void ToggleOn(Category category = null)
    {
        base.ToggleOn(category);
        itemList.onSelect.RemoveAllListeners();
        itemList.onSelect.AddListener(Select);
        UpdateInv();
    }
    public override void ToggleOff(Category category = null)
    {
        base.ToggleOff(category);
    }
    void Start()
    {
        gameObject.SetActive(false);
        onSelect.AddListener(Select);
        menu = Manager.Instance.placingMenu;
        menu.putBack.onClick.AddListener(PutBackPlaceable);
        menu.sell.onClick.AddListener(SellPlaceable);
        menu.onSelect.AddListener(OnSelect);
        Manager.Instance.onSell.AddListener(UpdateInv);
        UpdateInv();
    }

    void Select()
    {
        place.onClick.RemoveAllListeners();
        if (purchasablesPlaced.Contains(itemList.currentPurchasable))
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
    }

    void UpdateInv()
    {
        itemList.UpdateList(Manager.Instance.inventory);
        foreach(PurchasableUI ui in itemList.purchasableUIs)
        {
            if (purchasablesPlaced.Count > 0 && purchasablesPlaced.Contains(ui.purchasable))
            {
                ui.button.interactable = false;
            }
            else
            {
                ui.button.interactable = true;
            }
        }
    }

    void PlaceButtonClicked()
    {
        //open placing menu
        //hide our menu
        if (!purchasablesPlaced.Contains(itemList.currentPurchasable))
        {
            itemList.purchasableUIs[itemList.purchasables.IndexOf(itemList.currentPurchasable)].button.interactable = false;
            GameObject obj = placeablePool.Pull();
            Placeable placeable = obj.GetComponent<Placeable>();
            placeable.Set(itemList.currentPurchasable);
            placeable.placeableClicked.AddListener(delegate { menu.Select(placeable); });
            Manager.Instance.AddModifiers(itemList.currentPurchasable);
            purchasablesPlaced.Add(itemList.currentPurchasable);
        }
    }

    void OnSelect() //when the placebale menu selects an item
    {
        itemList.currentPurchasable = menu.currentPlaceable.purchasable;
    }

    void Sell()
    {
        if (purchasablesPlaced.Contains(itemList.currentPurchasable))
        {
            Debug.Log("Selling placed purchasable: " +  itemList.currentPurchasable.name);
            SellPlaceable();
            return;
        }
        else
        {
            Debug.Log("Selling bought purchasable: " + itemList.currentPurchasable.name);
            Manager.Instance.Sell(itemList.currentPurchasable);
        }
    }

    public void SellPlaceable()
    {
        Placeable placeable = menu.currentPlaceable;
        placeablePool.StartCoroutine("ReturnRigidbody", placeable.gameObject);
        Manager.Instance.RemoveModifiers(placeable.purchasable);
        Manager.Instance.Sell(placeable.purchasable);
        menu.currentPlaceable.selected = false;
        menu.currentPlaceable = null;
        purchasablesPlaced.Remove(placeable.purchasable);
        UpdateInv();
    }
    public void PutBackPlaceable()
    {
        Placeable placeable = menu.currentPlaceable;
        placeablePool.StartCoroutine("ReturnRigidbody",placeable.gameObject);
        purchasablesPlaced.Remove(placeable.purchasable);
        itemList.purchasableUIs[itemList.purchasables.IndexOf(placeable.purchasable)].button.interactable = true;
        itemList.purchasableUIs[itemList.purchasables.IndexOf(placeable.purchasable)].Set(placeable.purchasable);
        menu.currentPlaceable.selected = false;
        UpdateInv();
        //enable the buttons again
    }
}
