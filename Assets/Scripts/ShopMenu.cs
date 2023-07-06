using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public TextMeshProUGUI itemTitle;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public MeshFilter mesh;
    public Button buy;

    Purchasable currentPurchasable;

    float totalMoney;
    float currentMoney;

    public enum Categories {SUBSTRATE,ORNAMENTS,LIVEPLANTS,HIDES,HEATING};
    public List<GameObject> categoryUIs = new List<GameObject>();

    public List<Purchasable> purchasables = new List<Purchasable>();
    public GameObject purchasePrefab;
    List<PurchasableUI> purchasableUIs = new List<PurchasableUI>();

    void Start()
    {
        //instantiate the uis
        //connect their buttons up
        foreach(Purchasable purchasable in purchasables)
        {
            GameObject instance = Instantiate(purchasePrefab);
            PurchasableUI ui = instance.GetComponent<PurchasableUI>();
            ui.purchasable = purchasable;
            ui.button.onClick.AddListener(delegate { Select(purchasable); });
            ui.Set();
            instance.transform.SetParent(categoryUIs[(int)purchasable.category].transform);
        }

        currentPurchasable = purchasables[0];
        SwapCategory((int)currentPurchasable.category);
        Select(currentPurchasable);
    }

    public void SwapCategory(int index)
    {
        for(int i=0; i<categoryUIs.Count; i++)
        {
            if(i == index)
            {
                categoryUIs[i].SetActive(true);
            }
            else
            {
                categoryUIs[i].SetActive(false);
            }
        }
    }

    void Select(Purchasable purchasable)
    {
        buy.onClick.RemoveAllListeners();
        //buy.onClick.AddListener(delegate { }); //connect buy to inventory's get function based on this purchasable
        itemTitle.text = purchasable.displayName;
        itemPrice.text = purchasable.price.ToString();
        itemDescription.text = purchasable.description;
        mesh.mesh = purchasable.model;
    }
}
