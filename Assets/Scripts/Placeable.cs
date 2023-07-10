using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class Placeable : MonoBehaviour
{
    //handles own movement
    public MeshFilter meshFilter;
    public Purchasable purchasable;
    public Color color = Color.white;
    public Vector3 menuOffset;
    public bool selected = false;
    public UnityEvent placeableClicked;
    //Camera cam;

    private void Start()
    {
        //cam = Camera.main;
    }

    public void Set(Purchasable _purchasable)
    {
        purchasable = _purchasable;
        meshFilter.mesh = purchasable.model;
        transform.SetParent(Manager.Instance.placingRef.transform, false);
    }

    private void OnMouseDown()
    {
        //open menu
        placeableClicked.Invoke();
    }
    /*private void OnMouseDrag()
    {
        Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);
        if (target < )
            transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
    }*/
}
