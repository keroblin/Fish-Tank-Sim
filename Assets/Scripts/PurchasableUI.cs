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
    public Image icon;
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
        if(_purchasable.icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = _purchasable.icon;
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
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
