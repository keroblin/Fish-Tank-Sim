using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ItemList : CategoryList
{
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public MeshFilter mesh;

    public Purchasable currentPurchasable;

    public enum Categories { SUBSTRATE, ORNAMENTS, LIVEPLANTS, HIDES, HEATING };

    public List<Purchasable> purchasables = new List<Purchasable>();
    public GameObject purchasePrefab;
    public List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();

    public Pool pool;
    public override void OnReady()
    {
        //instantiate the uis
        //connect their buttons up
        pool = Manager.Instance.purchasableUiPool;
        foreach (Purchasable purchasable in purchasables)
        {
            GameObject instance = pool.Pull();
            PurchasableUI ui = instance.GetComponent<PurchasableUI>();
            ui.purchasable = purchasable;
            PurchaseableSetter(ui);
            ui.Set(purchasable);
            instance.transform.SetParent(categoryUIs[(int)purchasable.category].transform);
            purchasableUIs.Add(ui);
        }

        UpdateSelection();
    }

    private void OnEnable()
    {
        UpdateSelection();
    }

    public void UpdateSelection()
    {
        if (purchasables.Count != 0)
        {
            currentPurchasable = purchasables[purchasables.Count - 1];
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
        currentPurchasable = purchasable;
        itemTitle.text = purchasable.displayName;
        itemPrice.text = purchasable.price.ToString();
        itemDescription.text = purchasable.description;
        //mesh.mesh = purchasable.model;
    }
}
