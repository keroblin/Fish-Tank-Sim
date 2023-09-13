using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemBehaviour : Placeable
{
    //handles own movement

    float zMod;
    Vector2 input;
    int dir = 0;
    Bounds bounds;
    Camera cam;
    bool isDown = false;

    float increment = 0.05f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 255, 0, 0.3f);
        Gizmos.DrawCube(transform.localPosition + new Vector3(input.x * increment, 0f, input.y * increment), col.size);
    }

    private void Start()
    {
        cam = Camera.main;
        bounds = Manager.Instance.currentTank.tankBounds;
        placeableClicked.AddListener(delegate { PlacementManager.Instance.Select(this); });
    }

    public override void Set(Purchasable _purchasable, bool fromPool = false)
    {
        Debug.Log("Set item to " +  _purchasable.name);
        purchasable = _purchasable;
        meshFilter.mesh = purchasable.model;
        meshRenderer.material = purchasable.material;
        mat = meshRenderer.material; //gets unique version of mat
        if (editable)
        {
            SetColor(color);
            menuOffset = new Vector3(0f, purchasable.model.bounds.extents.y, -(purchasable.model.bounds.extents.z + .3f));
        }
        col.size = purchasable.model.bounds.size;
        meshFilter.gameObject.transform.localPosition = new Vector3(0, purchasable.model.bounds.extents.y, 0);
        col.center = purchasable.model.bounds.center;
        transform.SetParent(Manager.Instance.placingRef.transform, false);
    }

    private void OnMouseDown()
    {
        //open menu
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Clicked();
        }
    }

    public override void Clicked()
    {
        placeableClicked.Invoke();

        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position); //current position on screen point (constant)
    }

    private void Update()
    {
        if (editable)
        {
            Debug.Log(CameraMove.isUp + " cam state");
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

                if (Mathf.Abs(Input.GetAxisRaw("Mouse ScrollWheel")) > 0)
                {
                    if (Mathf.Abs(zMod + Input.GetAxisRaw("Mouse ScrollWheel")) < 4f)
                    {
                        zMod += Input.GetAxisRaw("Mouse ScrollWheel");
                        Debug.Log("zMod: " + zMod);
                    }
                }

                Move();
                input = Vector2.zero;
            }
        }
    }

    void Move()
    {
        Camera cam = Camera.main;
        if (input.magnitude > 0 && !isDown)
        {
            Vector3 target = transform.position + new Vector3(input.x * increment, 0f, input.y * increment);
            isDown = true;

            if (bounds.Contains(target))
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item") || collision.gameObject.CompareTag("TankWall"))
        {
            Debug.Log("hit");
            this.mat.color = Color.red;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item") || collision.gameObject.CompareTag("TankWall"))
        {
            this.mat.color = color;
        }
    }

    private void OnMouseDrag()
    {
        if (selected && editable)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            int layerMask = ~((1 << LayerMask.NameToLayer("Fish")) | (1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Item")));
            RaycastHit hit;
            Vector3 target = transform.position;
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen + (zMod / 10)));
                target.y = transform.position.y;
            }

            if (bounds.Contains(target))
            {
                transform.position = target;
                if (this.mat.color != color)
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
