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
    List<Placeable> placeablesPlaced = new List<Placeable>();
    public Pool placeablePool;

    public override void ToggleOn(Category category = null)
    {
        base.ToggleOn(category);
        itemList.onSelect.RemoveAllListeners();
        itemList.onSelect.AddListener(Select);
        UpdateInv();
        itemList.UpdateSelection();
    }
    public override void ToggleOff(Category category = null)
    {
        base.ToggleOff(category);
    }
    void Start()
    {
        onSelect.AddListener(Select);
        menu = Manager.Instance.placingMenu;
        menu.putBack.onClick.AddListener(PutBackPlaceable);
        menu.sell.onClick.AddListener(SellPlaceable);
        menu.onSelect.AddListener(OnSelect);
        Manager.Instance.onSell.AddListener(UpdateInv);
        UpdateInv();
        itemList.UpdateSelection();
    }

    void Select()
    {
        if (itemList.currentPurchasable)
        {
            place.onClick.RemoveAllListeners();
            if (purchasablesPlaced.Contains(itemList.currentPurchasable))
            {
                place.interactable = false;
                itemList.meshRenderer.material = placeablesPlaced.Find(x => x.purchasable == itemList.currentPurchasable).mat;
            }
            else
            {
                place.interactable = true;
                itemList.meshRenderer.material = itemList.currentPurchasable.material;
                if (itemList.currentPurchasable.category != (int)Manager.ItemCategories.SUBSTRATE)
                {
                    place.onClick.AddListener(PlaceButtonClicked);
                }
                else
                {
                    place.onClick.AddListener(delegate { Manager.Instance.SwapSubstrate(itemList.currentPurchasable as Item); });
                }

            }
            sell.onClick.RemoveAllListeners();
            sell.onClick.AddListener(Sell);
        }
    }

    void UpdateInv()
    {
        itemList.UpdateList(Manager.Instance.inventory);
    }

    void PlaceButtonClicked()
    {
        //open placing menu
        //hide our menu
        if (!purchasablesPlaced.Contains(itemList.currentPurchasable))
        {
            GameObject obj = placeablePool.Pull();
            Placeable placeable = obj.GetComponent<Placeable>();
            placeable.Set(itemList.currentPurchasable);
            placeable.placeableClicked.AddListener(delegate { menu.Select(placeable); });
            Manager.Instance.AddModifiers(itemList.currentPurchasable as Item);
            purchasablesPlaced.Add(itemList.currentPurchasable);
            placeablesPlaced.Add(placeable);
        }
    }

    void OnSelect() //when the placebale menu selects an item
    {
        itemList.currentPurchasable = menu.currentPlaceable.purchasable;
        Select();
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
            if(itemList.currentPurchasable.category == (int)Manager.ItemCategories.SUBSTRATE)
            {
                Manager.Instance.RemoveModifiers(itemList.currentPurchasable as Item); //check that this is working!!
                Manager.Instance.SwapSubstrate(Manager.Instance.nullSubstrate);
            }
            Manager.Instance.Sell(itemList.currentPurchasable);
            UpdateInv();
            itemList.UpdateSelection();
        }
    }

    public void SellPlaceable()
    {
        Placeable placeable;
        if (menu.currentPlaceable != null && itemList.currentPurchasable != menu.currentPlaceable.purchasable)
        {
            placeable = menu.currentPlaceable;
        }
        else
        {
            placeable = placeablesPlaced.Find(x => x.purchasable == itemList.currentPurchasable);
        }
        Manager.Instance.RemoveModifiers(placeable.purchasable as Item);
        Manager.Instance.Sell(placeable.purchasable);
        if (menu.currentPlaceable)
        {
            menu.currentPlaceable.selected = false;
            menu.currentPlaceable = null;
        }
        purchasablesPlaced.Remove(placeable.purchasable);
        placeablesPlaced.Remove(placeable);
        UpdateInv();
        itemList.UpdateSelection();
        placeablePool.StartCoroutine("ReturnRigidbody", placeable.gameObject);
    }
    public void PutBackPlaceable()
    {
        Placeable placeable = menu.currentPlaceable;
        placeablePool.StartCoroutine("ReturnRigidbody",placeable.gameObject);
        purchasablesPlaced.Remove(placeable.purchasable);
        itemList.purchasableUIs[itemList.purchasables.IndexOf(placeable.purchasable)].Set(placeable.purchasable);
        menu.currentPlaceable.selected = false;
        placeablesPlaced.Remove(menu.currentPlaceable);
        itemList.UpdateSelection();
        //enable the buttons again
    }
}
