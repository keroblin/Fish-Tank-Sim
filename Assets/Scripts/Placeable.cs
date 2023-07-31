using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    Vector2 input;
    int dir = 0;
    Bounds bounds;
    Camera cam;
    bool isDown = false;

    float increment = 0.05f;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 255, 0, 0.3f);
        Gizmos.DrawCube(transform.localPosition + new Vector3(input.x * increment, 0f, input.y * increment), col.size);
    }

    private void Start()
    {
        cam = Camera.main;
        bounds = Manager.Instance.tankBounds;
    }

    public void Set(Purchasable _purchasable)
    {
        purchasable = _purchasable;
        meshFilter.mesh = purchasable.model;
        meshRenderer.material = _purchasable.material;
        mat = meshRenderer.material; //gets unique version of mat
        SetColor(color);
        menuOffset = new Vector3(0f, purchasable.model.bounds.extents.y, -(purchasable.model.bounds.extents.z + .3f));
        col.size = purchasable.model.bounds.size;
        meshFilter.gameObject.transform.localPosition = new Vector3(0, purchasable.model.bounds.extents.y, 0);
        col.center = purchasable.model.bounds.center;
        transform.SetParent(Manager.Instance.placingRef.transform, false);
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
            placeableClicked.Invoke();
        }
    }

    private void FixedUpdate() //to be changed
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        if (Input.GetKeyDown(KeyCode.Plus) && dir == 0){ dir = 1; }
        if (Input.GetKeyDown(KeyCode.Minus) && dir == 0) { dir = -1; }
        if (!Input.GetKeyDown(KeyCode.Plus) && !Input.GetKeyDown(KeyCode.Minus)){ dir = 0;}

        if (selected)
        {
            //rotate
            if (dir != 0)
            {
                transform.Rotate(0, 45 * dir, 0);
            }
            //move the object
            if (input.magnitude > 0 && !isDown)
            {
                Vector3 target = transform.position + new Vector3(input.x * increment, 0f, input.y * increment);

                Vector2 xSides = new Vector2(target.x - col.size.x/2, target.x + col.size.x/2);
                Vector2 zSides = new Vector2(target.z - col.size.z/2, target.z + col.size.z/2);
                Vector2 boundsXSides = new Vector2(-bounds.extents.x, bounds.extents.x);
                Vector2 boundsYSides = new Vector2(-bounds.extents.z, bounds.extents.z);

                if (xSides.x > boundsXSides.x && xSides.y < boundsXSides.y && zSides.x > boundsYSides.x && zSides.y < boundsYSides.y)//if within the bounds
                {
                    transform.position = target;
                }
                isDown = true;
                return;
            }
            else if (isDown && input.magnitude <= 0)
            {
                isDown = false;
                return;
            }
        }
        input = Vector2.zero;
    }

    public void SetInput(Vector2 _input)
    {
        input = _input.normalized;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Debug.Log("hit");
            this.mat.color = Color.red;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            this.mat.color = color;
        }

    }

    /*private void OnMouseDrag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 6;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask))
        {
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.Log("Did not Hit");
        }

        Vector3 size = Vector3.Scale(col.size * 2, hit.point);
        if (bounds.Contains(size))
        {
            transform.position = hit.point;
        }
    } */
}
