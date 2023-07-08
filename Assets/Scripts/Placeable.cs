using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Purchasable purchasable;
    public Color color = Color.white;
    public Vector3 menuOffset;

    void Start()
    {
        //meshFilter.mesh = purchasable.model;
    }

    public void StartMove()
    {
        //idk yet! maybe i'll just do it w arrows in the placeable menu
    }
    public void StartRotate()
    {
        //idk yet! maybe i'll just do it w arrows in the placeable menu
    }
    public void Sell()
    {
        //do stat stuff here
        //deal w this object
        Manager.Instance.OnPlaceableRemoved(this);
        Manager.Instance.Sell(purchasable);
    }
    public void PutBack()
    {
        Manager.Instance.OnPlaceableRemoved(this);
        //Manager.Instance.inventory.Add(purchasable);
    }

    void SetMove()
    {

    }

    void SetColor()
    {

    }

    void OnPlace()
    {
        Manager.Instance.OnPlaceablePlaced(this);
    }

    void PickUp()
    {
        Manager.Instance.OnPlaceableRemoved(this);
        Manager.Instance.placingMenu.Set(this);
    }

    private void OnMouseDown()
    {
        //open menu
        PickUp();
    }
}
