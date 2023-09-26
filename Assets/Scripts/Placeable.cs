using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class Placeable : MonoBehaviour
{
    //handles own movement
    public Material mat;
    public Rigidbody rb;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Purchasable purchasable;
    public Color color = Color.white;
    public Vector3 menuOffset;
    public bool selected = false;
    public UnityEvent placeableClicked;
    public BoxCollider col;

    public bool editable;

    private void Start()
    {
        placeableClicked.AddListener(delegate { PlacementManager.Instance.Select(this); });
        Manager.Instance.enterEdit.AddListener(delegate { this.editable = true; });
        Manager.Instance.enterView.AddListener(delegate { this.editable = false; });
    }

    public virtual void Set(Purchasable _purchasable, bool fromPool = false)
    {
        purchasable = _purchasable;
        if (fromPool)
        {
            meshFilter.mesh = purchasable.model;
            meshRenderer.material = _purchasable.material;
            mat = meshRenderer.material; //gets unique version of mat
            if (editable)
            {
                SetColor(color);
                menuOffset = new Vector3(0f, purchasable.model.bounds.extents.y, -(purchasable.model.bounds.extents.z + .3f));
            }
            col.size = purchasable.model.bounds.size;
            meshFilter.gameObject.transform.localPosition = new Vector3(0, purchasable.model.bounds.extents.y, 0);
            col.center = purchasable.model.bounds.center;
        }
    }

    public void SetColor(Color _color)
    {
        color = _color;
        this.mat.color = color;
    }

    private void OnMouseDown()
    {
        //open menu
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Clicked();
        }
    }

    public virtual void Clicked()
    {
        placeableClicked.Invoke();
    }

    public virtual void SendOff()
    {
        Manager.Instance.allPurchasables[purchasable]--;
        PlacementManager.Instance.PutBackPlaceable(this);
    }

    private void OnDestroy()
    {
        placeableClicked.RemoveAllListeners();
    }
}
