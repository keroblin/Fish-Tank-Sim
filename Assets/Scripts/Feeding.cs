using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeding : MonoBehaviour
{
    //Food currentFood;
    //food object should have a big massive area
    //when fish enter the area they go toward it
    bool isDown;
    bool isOpen = true;
    Camera cam;
    Bounds bounds;
    public delegate void FoodPlaced(GameObject food);
    public static event FoodPlaced OnFoodPlaced;
    public Pool foodPool;
    public GameObject foodRef;
    public Food currentFood;

    bool inBounds;
    Vector3 debugHitPoint;
    void Start()
    {
        cam = Camera.main;
        bounds = Manager.Instance.tankBounds;
        bounds.size *= 1.3f;
    }

    private void OnDrawGizmos()
    {
        if (!inBounds) { Gizmos.color = Color.red; }
        else { Gizmos.color = Color.green; }
        Gizmos.DrawSphere(debugHitPoint, .2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            if (Input.GetMouseButtonDown(0) && !isDown)
            {
                SpawnFood();
                Debug.Log("Clicked");
                isDown = true;
                return;
            }
            else
            {
                isDown = false;
            }
        }
    }

    public void Open()
    {
        isOpen = true;
    }
    public void Close()
    {
        isOpen = false;
    }

    private void SpawnFood()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int layerMask = ~(1 << 11) & ~(1 << 2); //fish

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.Log("Did not Hit");
        }

        if (bounds.Contains(hit.point))
        {
            inBounds = true;
            GameObject newFood = foodPool.Pull();
            newFood.transform.SetParent(null, false);
            newFood.transform.position = hit.point;
            FoodBehaviour behaviour = newFood.GetComponent<FoodBehaviour>();
            behaviour.food = currentFood;
            newFood.SetActive(true);
            OnFoodPlaced.Invoke(newFood);
        }
        else
        {
            inBounds = false;
        }

        debugHitPoint = hit.point;
    }
}
