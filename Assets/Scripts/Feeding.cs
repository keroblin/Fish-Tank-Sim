using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Feeding : MonoBehaviour
{
    //Food currentFood;
    //food object should have a big massive area
    //when fish enter the area they go toward it
    bool isDown;
    bool isOpen = false;
    Camera cam;
    Bounds bounds;
    public delegate void FoodPlaced(GameObject food);
    public event FoodPlaced OnFoodPlaced;
    public Pool foodPool;
    public GameObject foodRef;
    [SerializeField] public Food currentFood { get;set;}

    public static Feeding Instance { get; private set; }

    bool inBounds;
    Vector3 debugHitPoint;

    private void OnEnable()
    {
        Instance = this;
        cam = Camera.main;
        bounds = Manager.Instance.currentTank.tankBounds;
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
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && !isDown)
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
            behaviour.Use += RemoveFood;
            newFood.SetActive(true);
            if(OnFoodPlaced != null && newFood != null)
            {
                OnFoodPlaced(newFood);
            }
        }
        else
        {
            inBounds = false;
        }

        debugHitPoint = hit.point;
    }

    void RemoveFood(FoodBehaviour food)
    {
        foodPool.ReturnRigidbody(food.gameObject);
        food.Use -= RemoveFood;
        food.gameObject.SetActive(false);
        food.food = null;
    }
}
