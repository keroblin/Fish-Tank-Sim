using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    public TextMeshProUGUI money;
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public MeshFilter mesh;

   public Purchasable currentPurchasable;

    //possibly move these into another script called money manager or something for easy saving and so this doesnt have to be running/accessible to sell items
    const float totalMoney = 200.00f;
    public float currentMoney = totalMoney;

    public enum Categories { SUBSTRATE, ORNAMENTS, LIVEPLANTS, HIDES, HEATING };
    public List<GameObject> categoryUIs = new List<GameObject>();
    public List<Button> categoryButtons = new List<Button>();

    public List<Purchasable> purchasables = new List<Purchasable>();
    public GameObject purchasePrefab;
    List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();

    void Start()
    {
        //instantiate the uis
        //connect their buttons up
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
        foreach (Purchasable purchasable in purchasables)
        {
            GameObject instance = Instantiate(purchasePrefab);
            PurchasableUI ui = instance.GetComponent<PurchasableUI>();
            ui.purchasable = purchasable;
            PurchaseableSetter(ui);
            ui.Set();
            instance.transform.SetParent(categoryUIs[(int)purchasable.category].transform);
        }

        currentPurchasable = purchasables[0];
        SwapCategory((int)currentPurchasable.category);
        Select(currentPurchasable);
    }

    public virtual void PurchaseableSetter(PurchasableUI purchasableUI)
    {
    }

    public void SwapCategory(int index)
    {
        for (int i = 0; i < categoryUIs.Count-1; i++)
        {
            if (i == index)
            {
                categoryUIs[i].SetActive(true);
                categoryButtons[i].interactable = false;
            }
            else
            {
                categoryButtons[i].interactable = true;
                categoryUIs[i].SetActive(false);
            }
        }
    }

    public virtual void Select(Purchasable purchasable)
    {
        itemTitle.text = purchasable.displayName;
        itemPrice.text = purchasable.price.ToString();
        itemDescription.text = purchasable.description;
        mesh.mesh = purchasable.model;
    }
}
