using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class ItemList : CategoryList
{
    public UnityEvent onSelect;
    public UnityEvent onSet;

    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public MeshFilter mesh;

    public Purchasable currentPurchasable;
    public GameObject noneSelectedMask;

    public enum Categories { SUBSTRATE = 0, ORNAMENTS = 1, LIVEPLANTS = 2, HEATING = 3 };

    public List<Purchasable> purchasables = new List<Purchasable>();
    public List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();

    public Pool pool;

    private void Start()
    {
        Manager.Instance.onBuy.AddListener(delegate { UpdateList(purchasables); });
        Manager.Instance.onSell.AddListener(delegate { UpdateList(purchasables); });
    }

    public void UpdateSelection()
    {
        if (purchasables.Count != 0)
        {
            noneSelectedMask.SetActive(false);
            currentPurchasable = purchasables[purchasables.Count - 1];
            SwapCategory((int)currentPurchasable.category);
            Select(currentPurchasable);
        }
        else
        {
            currentPurchasable = null;
            noneSelectedMask.SetActive(true);
        }
        //otherwise do a no items got thing here
    }

    public void UpdateList(List<Purchasable> _purchasables)
    {
        if (purchasableUIs.Count > _purchasables.Count) //if there are more than required, remove them
        {
            List<PurchasableUI> excess;
            if (_purchasables.Count > 0 && purchasableUIs.Count > 0)
            {
                Debug.Log("Purchasable count: " + _purchasables.Count + "\n UI count: " + purchasableUIs.Count);
                excess = purchasableUIs.GetRange(_purchasables.Count, purchasableUIs.Count - _purchasables.Count);
                Debug.Log("Removing excess from " + excess[0].purchasable.name + " to " + excess[excess.Count-1].purchasable.name);
                foreach (PurchasableUI ui in excess)
                {
                    //Debug.Log("Returning " + ui.purchasable.name);
                    pool.Return(ui.gameObject);
                }
                purchasableUIs.RemoveRange(_purchasables.Count, purchasableUIs.Count - _purchasables.Count); //remove any excess objects
            }
            else if (_purchasables.Count <= 0)
            {
                Debug.Log("No purchasables! Clearing uis...");
                foreach(PurchasableUI ui in purchasableUIs)
                {
                    pool.Return(ui.gameObject);
                }
                purchasableUIs = new List<PurchasableUI>();
                return;
            }
        }

        //declaring vars here to solve issues in delegates https://stackoverflow.com/questions/36611327/unity-indexoutofrangeexception-that-i-cant-solve
        for (int i = 0; i < _purchasables.Count; i++)
        {
            if (i > purchasableUIs.Count - 1) //if there isnt an object already, make a new one
            {
                GameObject instance = pool.Pull();
                PurchasableUI ui = instance.GetComponent<PurchasableUI>();
                purchasableUIs.Add(ui);
            }

            if (purchasableUIs[i].purchasable != _purchasables[i])
            {
                int index = i;
                PurchasableUI workingUI = purchasableUIs[index];
                Purchasable workingPurchasable = _purchasables[index];
                int workingCategoryIndex = (int)workingPurchasable.category;
                Category workingCategory = categories[workingCategoryIndex];

                workingUI.Set(workingPurchasable);
                workingUI.button.onClick.RemoveAllListeners();
                workingUI.button.onClick.AddListener(delegate { Select(workingPurchasable); });
                workingUI.transform.SetParent(workingCategory.transform);
            }
        }

        purchasables = _purchasables;
        //UpdateSelection();
    }

    /*void SetUI(PurchasableUI _ui, Purchasable _purchasable, int index) //passing in to hopefully ensure security in delegates
    {
        _ui.Set(_purchasable);
        _ui.button.onClick.RemoveAllListeners();
        _ui.button.onClick.AddListener(delegate { Select(_purchasable); });
        Category workingCategory = categories[(int)_purchasable.category];
        _ui.transform.SetParent(workingCategory.transform);
    }*/

    void Select(Purchasable purchasable)
    {
        currentPurchasable = purchasable;
        itemTitle.text = purchasable.name;
        itemPrice.text = "£" + purchasable.price.ToString("#.00");
        itemDescription.text = purchasable.description;
        onSelect.Invoke();
        //mesh.mesh = purchasable.model;
    }
}
