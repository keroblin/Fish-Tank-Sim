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
    public void Set()
    {
        displayName.text = purchasable.displayName;
        //meshFilter.mesh = purchasable.model;
        //shop menu links itself to the onclick to update the description
    }
}
