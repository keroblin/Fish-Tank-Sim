using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public Button putBack;
    public PlacingMenu menu;
    public List<Purchasable> purchasablesPlaced = new List<Purchasable>();
    List<Placeable> placeablesPlaced = new List<Placeable>();
    public Pool placeablePool;
    void Start()
    {
        menu = Manager.Instance.placingMenu;
        menu.putBack.onClick.AddListener(PutBackPlaceable);
        //menu.sell.onClick.AddListener(SellPlaceable); ////////////todo
        menu.onSelect.AddListener(OnSelect);
    }

    public void Place(Shop.ShopItem item)
    {
        Placeable placeable;
        if(item.purchasable.prefab != null)
        {
            //instantiate
            placeable = Instantiate(item.purchasable.prefab).GetComponent<Placeable>();
        }
        else
        {
            GameObject obj = placeablePool.Pull();
            placeable = obj.GetComponent<Placeable>();
        }
        placeable.Set(item.purchasable);
        placeable.placeableClicked.AddListener(delegate { menu.Select(placeable); });
        //placeable.Placed();
        //replace this with the above
        //Manager.Instance.AddModifiers(itemList.currentPurchasable as Item);
        purchasablesPlaced.Add(item.purchasable);
        placeablesPlaced.Add(placeable);
    }

    void OnSelect() //when the placeable menu selects an item
    {
        //select the placeable in the shop too
        //Shop.Select(menu.currentPlaceable.purchasable);
        //Select(); ////////////////todo
    }

    public void Sell(Placeable placeable)
    {
        /*if (menu.currentPlaceable != null && itemList.currentPurchasable != menu.currentPlaceable.purchasable) ////////////todo
        {
            placeable = menu.currentPlaceable;
        }
        else
        {
            //placeable = placeablesPlaced.Find(x => x.purchasable == itemList.currentPurchasable); ///////////todo
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

        //////////////////todo
        //UpdateInv();
        //itemList.UpdateSelection();
        placeablePool.StartCoroutine("ReturnRigidbody", placeable.gameObject);*/
    }
    public void PutBackPlaceable()
    {
        Placeable placeable = menu.currentPlaceable;
        placeablePool.StartCoroutine("ReturnRigidbody", placeable.gameObject);
        purchasablesPlaced.Remove(placeable.purchasable);
        //itemList.purchasableUIs[itemList.purchasables.IndexOf(placeable.purchasable)].Set(placeable.purchasable); /////////todo
        menu.currentPlaceable.selected = false;
        placeablesPlaced.Remove(menu.currentPlaceable);
        //itemList.UpdateSelection(); //////////todo
        //enable the buttons again
    }
}
