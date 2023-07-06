using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public MeshFilter meshFilter;
    Purchasable purchasable;
    public Color color = Color.white;

    void Start()
    {
        meshFilter.mesh = purchasable.model;
    }

    void SetMove()
    {

    }

    void SetColor()
    {

    }

    private void OnMouseDown()
    {
        //open menu
    }
}
