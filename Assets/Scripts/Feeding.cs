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
    void Start()
    {
        cam = Camera.main;
        bounds = Manager.Instance.tankBounds;
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
        //TEMP!!!!!!!!!!!
        GameObject newFood = foodPool.Pull();
        newFood.transform.SetParent(null, false);
        newFood.transform.position = new Vector3(1.72f, .92f, 0f);
        newFood.SetActive(true);
        OnFoodPlaced.Invoke(newFood);

        /*Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int layerMask = ~(1 << 11); //fish

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
            Debug.Log("In bounds");
            GameObject newFood = foodPool.Pull();
            newFood.transform.SetParent(null, false);
            newFood.transform.position = hit.point;

            newFood.SetActive(true);
            //OnFoodPlaced.Invoke(newFood);
        }
        else
        {
            Debug.Log("Out of bounds at " + hit.point + ", bounds are " + bounds.extents);
        }
        */
    }
}
