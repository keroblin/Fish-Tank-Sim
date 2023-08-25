using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Shop:MonoBehaviour
{
    //literally just handles the shop
    //the shopdetail shows details about each item
    //this script creates buttons based on categories and allows us to buy, sell or place
    
    public UnityEvent onPlace;
    //may not use
    public UnityEvent onSelect;
    public UnityEvent onSet;

    public Button buy;
    public Button sell;
    public Button place;

    public PlacingMenu menu;

    public Pool pool;
    public Pool placeablesPool;
    public GameObject placementParent;

    public List<ShopItem> boughtItems;
    public List<MainCategory> mainCategories;
    public MainCategory currentMainCategory;
    public ShopCategory currentCategory;
    public int currentCategoryIndex;
    //public CategoryList mainCategoryList;
    //public CategoryList subCategoryList;

    public GameObject noneSelectedMask;
    public ShopDetail shopDetail;

    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;

    public ItemPreview itemPreview;

    List<PurchasableUI> purchasableUIs;
    Purchasable currentPurchasable;
    ShopItem selectedShopItem;
    [System.Serializable]
    public class ShopItem
    {
        public Purchasable purchasable;
        [HideInInspector]public PurchasableUI ui;
        [HideInInspector] public List<Placeable> placeables = new List<Placeable>();
        [HideInInspector] public int quantity = 0;
    }
    [System.Serializable]
    public class MainCategory
    {
        public List<ShopCategory> subcategories;
        public Category category;
        public CategoryList subList;
        public ShopDetail detail;
    }
    [System.Serializable]
    public class ShopCategory
    {
        public Category category;
        public List<ShopItem> items;
    }

    private void Start()
    {
        //get all the purchasableuis and set up their events
        //not sure how best to do this

        for(int i = 0; i < mainCategories.Count; i++)
        {
            int index = i;
            mainCategories[index].category.button.onClick.AddListener(delegate { SetMainCategory(index); });
            foreach (ShopCategory category in mainCategories[index].subcategories)
            {
                category.category.onSelect.AddListener(UpdateSelection);
                foreach (ShopItem item in category.items)
                {
                    GameObject instance = pool.Pull();
                    PurchasableUI ui = instance.GetComponent<PurchasableUI>();
                    ShopItem _item = item;
                    item.ui = ui;
                    ui.Set(item.purchasable);
                    ui.button.onClick.RemoveAllListeners();
                    ui.button.onClick.AddListener(delegate { Select(_item);});
                    ui.transform.SetParent(category.category.gameObject.transform);
                }
            }
        }

        buy.onClick.AddListener(Buy);
        sell.onClick.AddListener(Sell);
        place.onClick.AddListener(Place);
    }

    public void SetMainCategory(int index)
    {
        if(selectedShopItem != null)
        {
            selectedShopItem.ui.button.interactable = true;
        }
        if(currentMainCategory != null && currentMainCategory.subList != null)
        {
            currentMainCategory.subList.SwapCategory(-1);
        }
        
        MainCategory cat = mainCategories[index];
        currentMainCategory = cat;
        Debug.Log("Set detail " + cat.detail.name);
        if (shopDetail != null && shopDetail.gameObject.activeSelf)
        {
            shopDetail.gameObject.SetActive(false);
        }
        shopDetail = cat.detail;
        shopDetail.gameObject.SetActive(true);
        cat.subList.SwapCategory(0);
        UpdateSelection();
    }

    ///<summary>
    //this is the shop
    //in the shop we have our main categories and our subcategories
    //when the game starts, or in a debug function or something
    //we should create buttons for the categories
    //and child the content to objects
    //each category should contain a list of associated purchasables?
    //we use these to spawn and check if they are assigned to the parent category
    //we need to make it flexible enough to work with the food
    //maybe purchasables have their own buy and sell functionality?
    //like when we buy an item, change the values in the manager directly
    //or we could have a tag system in the purchasables, might be annoying tho
    ///</summary>

    public void UpdateSelection()
    {
        if (selectedShopItem == null || currentCategory.items.Count == 0)//if there isnt anything in the list
        {
            itemPreview.Set(null);
            noneSelectedMask.SetActive(true);
        }
        else if (currentCategory.items.Count > 0) 
        {
            noneSelectedMask.SetActive(false);
            Select(currentCategory.items[0]);
        }
    }

    public void Select(ShopItem item)
    {
        if(selectedShopItem != null)
        {
            selectedShopItem.ui.button.interactable = true;
        }
        item.ui.button.interactable = false;
        noneSelectedMask.SetActive(false);
        currentPurchasable = item.purchasable;
        itemTitle.text = item.purchasable.name;
        itemPrice.text = "Price: £" + item.purchasable.price.ToString("#.00");
        itemDescription.text = item.purchasable.description;

        itemPreview.Set(item.purchasable);
        shopDetail.Set(item.purchasable);
        
        
        //account for placeables here
        onSelect.Invoke();
        selectedShopItem = item;
        UpdateButtons();
    }

    void UpdateButtons()
    {
        if (selectedShopItem.quantity > 0)
        {
            buy.interactable = selectedShopItem.purchasable.stackable;
            place.interactable = selectedShopItem.quantity > selectedShopItem.placeables.Count;
        }
        else
        {
            buy.interactable = true;
            place.interactable = false;
        }
        
    }

    public void Buy()
    {
        if (selectedShopItem.purchasable.stackable)
        {
            selectedShopItem.quantity++;
        }
        else
        {
            //add item to inventory
            selectedShopItem.quantity = 1;
            boughtItems.Add(selectedShopItem);
        }
        selectedShopItem.purchasable.Buy();
        selectedShopItem.ui.ChangeQuantity(selectedShopItem.quantity);
        UpdateButtons();
    }

    public void Sell()
    {
        
        if (selectedShopItem.quantity > 0)
        {
            if (selectedShopItem.purchasable.stackable)
            {
                selectedShopItem.quantity--;
            }
            else
            {
                //remove from inventory, enable buy button
                selectedShopItem.quantity = 0;
                boughtItems.Remove(selectedShopItem);
            }

            if (selectedShopItem.placeables.Count > 0)
            {
                if (menu.currentPlaceable != null)
                {
                    selectedShopItem.placeables.Remove(menu.currentPlaceable);
                }
                else
                {
                    selectedShopItem.placeables.RemoveAt(0);
                }
            }

            selectedShopItem.ui.ChangeQuantity(selectedShopItem.quantity);
            selectedShopItem.purchasable.Sell();
        }
        else
        {
            Debug.Log("Nothing to sell!");
        }
        UpdateButtons();
    }

    public void Place()
    {
        //turn off interactable on place button
        GameObject obj;
        Placeable placeable;
        if(selectedShopItem.purchasable.prefab != null)
        {
            obj = Instantiate(selectedShopItem.purchasable.prefab);
        }
        else
        {
            obj = placeablesPool.Pull(); //change to placeables pool
        }
        obj.transform.SetParent(placementParent.transform,false);
        placeable = obj.GetComponent<Placeable>();
        selectedShopItem.placeables.Add(placeable);
        selectedShopItem.purchasable.Place();
        UpdateButtons();
    }

    public static string CheckSetPosNegative(float val, TextMeshProUGUI ui)
    {
        if (val < 0)
        {
            ui.color = Color.red;
            return "-";
        }
        else if (val > 0)
        {
            ui.color = Color.green;
            return "+";
        }
        else
        {
            ui.color = Color.white;
            return "";
        }
    }
}
