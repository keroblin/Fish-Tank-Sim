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
    public TextMeshProUGUI itemPh;
    public TextMeshProUGUI itemLight;
    public TextMeshProUGUI itemTemp;
    public TextMeshProUGUI itemHardness;
    public MeshFilter mesh;
    public MeshRenderer meshRenderer;
    float meshViewDefaultDistance;

    public Purchasable currentPurchasable;
    public GameObject noneSelectedMask;

    public List<Purchasable> purchasables = new List<Purchasable>();
    public List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();

    public Pool pool;

    private void Start()
    {
        Manager.Instance.onBuy.AddListener(delegate { UpdateList(purchasables); });
        Manager.Instance.onSell.AddListener(delegate { UpdateList(purchasables); });
        meshViewDefaultDistance = mesh.transform.position.z;
    }

    public void UpdateSelection()
    {
        if(currentPurchasable == null || purchasables.Count == 0)
        {
            currentPurchasable = null;
            mesh.mesh = null;
            noneSelectedMask.SetActive(true);
        }
        else if(purchasables.Count > 0)
        {
            noneSelectedMask.SetActive(false);
            if (currentPurchasable == null || !purchasables.Contains(currentPurchasable))
            {
                currentPurchasable = purchasables[0];
            }
            SwapCategory((int)currentPurchasable.category);
            Select(currentPurchasable);
        }
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
                purchasableUIs.Clear();
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
                var workingPurchasable = _purchasables[index];
                int workingCategoryIndex = workingPurchasable.category;
                Debug.Log(workingPurchasable.GetType().ToString());
                Category workingCategory = categories[workingCategoryIndex];

                workingUI.Set(workingPurchasable);
                workingUI.button.onClick.RemoveAllListeners();
                workingUI.button.onClick.AddListener(delegate { Select(workingPurchasable); });
                workingUI.transform.SetParent(workingCategory.transform);
            }
        }

        purchasables = _purchasables;
    }

    void Select(Purchasable purchasable)
    {
        noneSelectedMask.SetActive(false);
        currentPurchasable = purchasable;
        itemTitle.text = purchasable.name;
        itemPrice.text = "Price: £" + purchasable.price.ToString("#.00");
        itemDescription.text = purchasable.description;

        mesh.mesh = purchasable.model;
        meshRenderer.material = purchasable.material;

        Vector3 meshSize = mesh.mesh.bounds.size;
        meshSize.Scale(mesh.transform.localScale);
        mesh.transform.position = new Vector3(mesh.transform.position.x, mesh.transform.position.y, meshViewDefaultDistance + meshSize.magnitude);
        onSelect.Invoke();

        switch (purchasable.GetType().ToString())
        {
            case "Item":
                Item item = purchasable as Item;
                itemPh.text = "pH: " + CheckSetPosNegative(item.pHMod, itemPh) + item.pHMod.ToString();
                itemHardness.text = "Water Hardness: " + CheckSetPosNegative(item.dGHMod, itemHardness) + item.dGHMod.ToString() + "dGH";
                itemTemp.text = "Temperature: " + CheckSetPosNegative(item.tempMod, itemTemp) + item.tempMod.ToString() + "f";
                itemLight.text = "Light: " + CheckSetPosNegative(item.lightMod, itemLight) + (item.lightMod * 100f).ToString() + "%";
                break;
            case "Fish":
                break;
            case "Food":
                break;
        }
    }

    string CheckSetPosNegative(float val,TextMeshProUGUI ui)
    {
        if (val < 0)
        {
            ui.color = Color.red;
            return "-";
        }
        else if(val > 0)
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
