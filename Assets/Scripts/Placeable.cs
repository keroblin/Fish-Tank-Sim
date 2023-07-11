using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class Placeable : MonoBehaviour
{
    //handles own movement
    public Material mat;
    public Rigidbody rb;
    public MeshFilter meshFilter;
    public Purchasable purchasable;
    public Color color = Color.white;
    public Vector3 menuOffset;
    public bool selected = false;
    public UnityEvent placeableClicked;
    public BoxCollider col;
    Vector2 input;
    Bounds bounds;
    Camera cam;
    bool isDown = false;

    float increment = 0.05f;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255,255,0,0.3f);
        Gizmos.DrawCube(transform.localPosition + new Vector3(input.x * increment, 0f, input.y * increment), col.size);
    }

    private void Start()
    {
        cam = Camera.main;
        mat = meshFilter.gameObject.GetComponent<MeshRenderer>().material; //gets unique version of mat
        bounds = Manager.Instance.tankBounds;
    }

    public void Set(Purchasable _purchasable)
    {
        purchasable = _purchasable;
        meshFilter.mesh = purchasable.model;
        menuOffset = new Vector3(0, purchasable.model.bounds.extents.y + .5f, 0);
        col.size = purchasable.model.bounds.size;
        meshFilter.gameObject.transform.localPosition = new Vector3(0,purchasable.model.bounds.extents.y,0);
        col.center = meshFilter.gameObject.transform.localPosition;
        transform.SetParent(Manager.Instance.placingRef.transform, false);
    }

    private void OnMouseDown()
    {
        //open menu
        placeableClicked.Invoke();
    }

    private void FixedUpdate()
    {
        if(selected)
        {
            //move the object
            input = new Vector2(Input.GetAxisRaw("Horizontal"), (Input.GetAxisRaw("Vertical"))).normalized;
            if (input.magnitude > 0 && !isDown)
            {
                Vector3 target = transform.position + new Vector3(input.x * increment, 0f, input.y * increment);

                Vector3 size = Vector3.Scale(col.size * 2, target);
                if (bounds.Contains(size))
                {
                    transform.position = target;
                }
                isDown = true;
                return;
            }
            else if(isDown && input.magnitude <= 0)
            {
                isDown = false;
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Item"))
        {
            Debug.Log("hit");
            this.mat.color = Color.red;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Item"))
        {
            this.mat.color = color;
        }
    }

    //private void OnMouseDrag()
    //{

    //Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);

    /*RaycastHit hit;
    // Does the ray intersect any objects excluding the player layer
    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 3))
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        Debug.Log("Did Hit");
    }
    else
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        Debug.Log("Did not Hit");
    }
    if (target < )
        transform.position = cam.ScreenToWorldPoint(Input.mousePosition);*/
    //}
}
