using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu : MonoBehaviour
{
    public Dictionary<Food, PurchasableUI> hotBar = new();
    public List<PurchasableUI> foodBar; //hmmmm unsure!!
    public Dictionary<Food,PurchasableUI> foodInventory = new();
    public ShopDetail view;

    public GameObject hotBarParent;
    public GameObject allViewParent;
    public GameObject hotbar;
    public GameObject allView;

    bool mouseClicked;
    void Open()
    {
        hotbar.SetActive(true);
    }
    void Close()
    {
        hotbar.SetActive(false);
    }
    void Select()
    {
        //tell feeding it can feed
    }

    void UpdateList()
    {

    }

    void Start()
    {
        //load in saved foodbar here
        foreach (Purchasable purchasable in Manager.Instance.allPurchasableSOs)
        {
            if(purchasable is Food)
            {
                Food food = purchasable as Food;
                PurchasableUI purchasableUI = Manager.Instance.purchasableUiPool.Pull().GetComponent<PurchasableUI>();
                purchasableUI.gameObject.transform.SetParent(hotBarParent.transform, false);
                purchasableUI.button.onClick.AddListener(Feeding.Instance.Open);
                purchasableUI.button.onClick.AddListener(delegate { Feeding.Instance.currentFood = food; });
                purchasableUI.Set(food);
                if (Manager.Instance.allPurchasables[food] <= 0) { purchasableUI.button.interactable = false; purchasableUI.icon.color = Color.black; } else { purchasableUI.button.interactable = true; purchasableUI.icon.color = Color.white; }
                Manager.Instance.onQuantityChange.AddListener(delegate { if (Manager.Instance.allPurchasables[food] <= 0) { purchasableUI.button.interactable = false; purchasableUI.icon.color = Color.black; } else { purchasableUI.button.interactable = true; purchasableUI.icon.color = Color.white; } });
            }
            else
            {
                continue;
            }
        }
    }

    //get all the foods
    //have a selected foods and assign them to buttons, if there isnt a food assigned, then turn it off
    //click and drag selected foods into the food selection?
    //when clicked, make the cursor the icon for the food and allow food to be placed, until the food menu is closed
    //use the quantities bought in shop to manage this

}
