using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Windows;
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

    public PlacementManager placementManager;
    public ItemMenu menu;

    public Pool pool;

    public List<MainCategory> mainCategories;
    public MainCategory currentMainCategory;
    public ShopCategory currentCategory;

    public int placeableIndex;
    public Placeable selectedPlaceable;

    public GameObject noneSelectedMask;
    public ShopDetail shopDetail;

    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;

    public ItemPreview itemPreview;

    List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();
    Purchasable currentPurchasable;
    Purchasable selectedItem;
    
    [System.Serializable]
    public class MainCategory
    {
        public Category category;
        public CategoryList subList;
        public ShopDetail detail;
        public List<ShopCategory> subcategories;
    }
    [System.Serializable]
    public class ShopCategory
    {
        public Category category;
        public List<Purchasable> items;
    }

    private void Start()
    {
        //get all the purchasableuis and set up their events
        //not sure how best to do this
        menu.sell.onClick.AddListener(Sell);
        menu.putBack.onClick.AddListener(PutBack);

        for(int i = 0; i < mainCategories.Count; i++)
        {
            int index = i;
            mainCategories[index].category.button.onClick.AddListener(delegate { SetMainCategory(index); });
            foreach (ShopCategory category in mainCategories[index].subcategories)
            {
                category.category.onSelect.AddListener(UpdateSelection);
                foreach (Purchasable item in category.items)
                {
                    GameObject instance = pool.Pull();
                    PurchasableUI ui = instance.GetComponent<PurchasableUI>();
                    ui.Set(item);
                    ui.button.onClick.RemoveAllListeners();
                    ui.button.onClick.AddListener(delegate { Select(item);});
                    ui.transform.SetParent(category.category.gameObject.transform);
                    purchasableUIs.Add(ui);
                }
            }
        }

        buy.onClick.AddListener(Buy);
        sell.onClick.AddListener(Sell);
        place.onClick.AddListener(Place);

        PlacementManager.Instance.PlaceableSelected += SelectPlaceableItem;
    }

    public void SetMainCategory(int index)
    {
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

    public void UpdateSelection()
    {
        if (selectedItem == null || currentCategory.items.Count == 0)//if there isnt anything in the list
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

    public void SelectPlaceableItem(Placeable placeable)
    {
        selectedPlaceable = placeable;
        Select(placeable.purchasable);
    }

    public void Select(Purchasable purchasable) //might change to just be purchasable
    {
        noneSelectedMask.SetActive(false);
        currentPurchasable = purchasable;
        itemTitle.text = purchasable.name;
        itemPrice.text = "Price: £" + purchasable.price.ToString("#.00");
        itemDescription.text = purchasable.description;

        itemPreview.Set(purchasable);
        shopDetail.Set(purchasable);
        
        
        //account for placeables here
        onSelect.Invoke();
        selectedItem = purchasable;
        UpdateButtons();
    }

    void UpdateButtons()
    {
        if (Manager.Instance.allPurchasables[selectedItem] > 0)
        {
            buy.interactable = selectedItem.stackable;
            place.interactable = Manager.Instance.allPurchasables[selectedItem] > PlacementManager.Instance.GetAmountPlaced(selectedItem); //not sure how to deal with this hmm
        }
        else
        {
            buy.interactable = true;
            place.interactable = false;
        }

        foreach(PurchasableUI ui in purchasableUIs)
        {
            ui.button.interactable = ui.purchasable != selectedItem;
        }
    }

    public void Buy()//redundant i think, does stuff in manager
    {
        selectedItem.Buy();
        UpdateButtons();
    }

    public void Sell()
    {
        if (Manager.Instance.allPurchasables[selectedItem] > 0)
        {
            if (PlacementManager.Instance.GetAmountPlaced(selectedItem) > 0)
            {
                Debug.Log("Detected sellable placeables");
                if (!selectedPlaceable)
                {
                    Debug.Log("Menu hasnt selected a placeable, getting most recent");
                    selectedPlaceable = PlacementManager.Instance.GetMostRecentPlaceable(selectedItem);
                }
                PlacementManager.Instance.PutBackPlaceable(selectedPlaceable);
                PlacementManager.Instance.instances.Remove(selectedPlaceable);
                selectedItem.Remove();
            }

            selectedItem.Sell();
        }
        else
        {
            Debug.Log("Nothing to sell!");
        }
        UpdateButtons();
    }

    public void Place()
    {
        Debug.Log("Placing " + selectedItem.name);
        selectedItem.Place();
        UpdateButtons();
    }

    public void PutBack()
    {
        PlacementManager.Instance.PutBackPlaceable(selectedPlaceable);
        UpdateSelection();
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
