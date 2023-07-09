using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ItemList : CategoryList
{
    public TextMeshProUGUI money;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public MeshFilter mesh;

    public Purchasable currentPurchasable;

    Purchasable lastPurchasable;

    public enum Categories { SUBSTRATE, ORNAMENTS, LIVEPLANTS, HIDES, HEATING };

    public List<Purchasable> purchasables = new List<Purchasable>();
    public GameObject purchasePrefab;
    public List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();
    public override void OnReady()
    {
        //instantiate the uis
        //connect their buttons up
        money.text = "Your cash: £" + Manager.Instance.currentMoney.ToString("#.00");
        foreach (Purchasable purchasable in purchasables)
        {
            GameObject instance = Instantiate(purchasePrefab);
            PurchasableUI ui = instance.GetComponent<PurchasableUI>();
            ui.purchasable = purchasable;
            PurchaseableSetter(ui);
            ui.Set();
            instance.transform.SetParent(categoryUIs[(int)purchasable.category].transform);
            purchasableUIs.Add(ui);
        }

        UpdateSelection();
    }

    private void OnEnable()
    {
        UpdateSelection();
    }

    void UpdateSelection()
    {
        if (purchasables.Count != 0)
        {
            if(lastPurchasable == null)
            {
                currentPurchasable = purchasables[purchasables.Count - 1];
            }
            else
            {
                currentPurchasable = lastPurchasable;
            }
            SwapCategory((int)currentPurchasable.category);
            Select(currentPurchasable);
        }
        //otherwise do a no items got thing here
    }

    //interrupts the ready after an instance is created to set up any variables needed on the ui
    public virtual void PurchaseableSetter(PurchasableUI purchasableUI)
    {
    }

    public virtual void Select(Purchasable purchasable)
    {
        lastPurchasable = currentPurchasable;
        currentPurchasable = purchasable;
        itemTitle.text = purchasable.displayName;
        itemPrice.text = purchasable.price.ToString();
        itemDescription.text = purchasable.description;
        //mesh.mesh = purchasable.model;
    }
}
