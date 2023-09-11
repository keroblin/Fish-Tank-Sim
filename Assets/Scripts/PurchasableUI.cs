using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasableUI : MonoBehaviour
{
    public Button button;
    public MeshFilter meshFilter;
    public TextMeshProUGUI displayName;
    public GameObject quantityObj;
    public TextMeshProUGUI quantityUI;
    public Purchasable purchasable;

    public void Start()
    {
        Manager.Instance.onQuantityChange.AddListener(ChangeQuantity);
    }

    public void Set(Purchasable _purchasable)
    {
        purchasable = _purchasable;
        displayName.text = purchasable.name;
        meshFilter.mesh = purchasable.model;
        //shop menu links itself to the onclick to update the description
    }

    public void ChangeQuantity()
    {
        int quantity = Manager.Instance.allPurchasables[purchasable];
        quantityObj.SetActive(quantity > 0);
        if (quantity > 0)
        {
            quantityUI.text = quantity.ToString();
        }
    }
}
