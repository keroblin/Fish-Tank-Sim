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
    public Purchasable purchasable;
    public void Set(Purchasable _purchasable)
    {
        purchasable = _purchasable;
        displayName.text = purchasable.displayName;
        //Refresh();
        //meshFilter.mesh = purchasable.model;
        //shop menu links itself to the onclick to update the description
    }
    /*public void Refresh()
    {
        displayName.text = purchasable.displayName;
    }*/
}
