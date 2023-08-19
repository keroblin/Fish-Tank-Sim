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

    float zMod;
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

    private void Update()
    {
        if (selected)
        {
            if (!Input.GetKeyDown(KeyCode.Q) && dir == -1) { dir = 0; }
            if (!Input.GetKeyDown(KeyCode.E) && dir == 1) { dir = 0; }

            if (Input.GetKeyDown(KeyCode.Q) && dir == 0) { dir = -1; }
            if (Input.GetKeyDown(KeyCode.E) && dir == 0) { dir = 1; }

            if (dir != 0)
            {
                transform.Rotate(0, 45 * dir, 0);
            }

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            }

            if(Mathf.Abs(Input.GetAxisRaw("Mouse ScrollWheel")) > 0)
            {
                if(Mathf.Abs(zMod + Input.GetAxisRaw("Mouse ScrollWheel")) < 4f)
                {
                    zMod += Input.GetAxisRaw("Mouse ScrollWheel");
                    Debug.Log("zMod: " + zMod);
                }
            }

            Move();
            input = Vector2.zero;
        }
    }

    void Move()
    {
        if (input.magnitude > 0 && !isDown)
        {
            Vector3 target = transform.position + new Vector3(input.x * increment, 0f, input.y * increment);
            isDown = true;

            if(bounds.Contains(target))
            {
                transform.position = target;
            }

            return;
        }
        else if (isDown && input.magnitude <= 0)
        {
            isDown = false;
            return;
        }
    }

    public void SetInput(Vector2 _input)
    {
        input = _input.normalized;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") || other.CompareTag("TankWall"))
        {
            Debug.Log("hit");
            this.mat.color = Color.red;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") || other.CompareTag("TankWall"))
        {
            this.mat.color = color;
        }

    }

    private void OnMouseDrag()
    {
        if (selected)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            int layerMask = ~((1 << LayerMask.NameToLayer("Fish")) | (1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Item")));
            Debug.Log("Layermask: " + layerMask);

            RaycastHit hit;
            Vector3 target = transform.position;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                target = new Vector3(hit.point.x, hit.point.y, zMod);
                Debug.Log("Did Hit");
            }
            else
            {
                Debug.Log("Did not Hit");
            }

            if (bounds.Contains(target))
            {
                Debug.Log("Hit layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
                transform.position = target;
                if(this.mat.color != color)
                {
                    this.mat.color = color;
                }
            }
            else
            {
                this.mat.color = Color.red;
            }
        }
    }
}
