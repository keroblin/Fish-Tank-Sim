using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Burst.CompilerServices;

public class Feeding : MonoBehaviour
{
    bool isDown;
    bool isOpen = false;
    Camera cam;
    Bounds bounds;
    public delegate void FoodPlaced(GameObject food);
    public event FoodPlaced OnFoodPlaced;
    public GameObject foodRef;
    public Pool foodPool;
    public GameObject foodIndicator;
    public SpriteRenderer foodIndicatorSprite;
    public Sprite foodBag;
    public Sprite foodPour;
    public Sprite foodEmpty;
    public Sprite foodNotAllowed;

    [SerializeField] public Food currentFood { get;set;}

    public static Feeding Instance { get; private set; }

    bool inBounds;
    Vector3 indicatorTarget;
    Vector3 debugHitPoint;

    private void OnEnable()
    {
        Instance = this;
        cam = Camera.main;
        bounds = Manager.Instance.currentTank.tankBounds;
        bounds.size *= 1.3f;
        Manager.Instance.enterView.AddListener(Close);
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
                indicatorTarget = hit.point;
                if (!isDown)
                {
                    if (Manager.Instance.allPurchasables[currentFood] > 0)
                    {
                        foodIndicatorSprite.sprite = foodBag;
                    }
                    else
                    {
                        foodIndicatorSprite.sprite = foodEmpty;
                    }
                }
            }
            else if(!bounds.Contains(hit.point))
            {
                inBounds = false;
                foodIndicatorSprite.sprite = foodNotAllowed;
            }

            debugHitPoint = hit.point;

            foodIndicator.transform.position = new Vector3(indicatorTarget.x, foodIndicator.transform.position.y, foodIndicator.transform.position.z);
            
            //do something here to check quantity of food available

            if(inBounds && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0))
            {
                if (Manager.Instance.allPurchasables[currentFood] > 0)
                {
                    foodIndicatorSprite.sprite = foodPour;
                }
                else
                {
                    foodIndicatorSprite.sprite = foodNotAllowed;
                }
                Debug.Log("Pouring");
            }

            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && !isDown && Manager.Instance.allPurchasables[currentFood] > 0)
            {
                SpawnFood();
                Debug.Log("Clicked");
                isDown = true;
            }
            else if(isDown)
            {
                isDown = false;
            }
        }
    }

    public void Open()
    {
        isOpen = true;
        foodIndicator.gameObject.SetActive(true);
        //do indicator stuff here
    }
    public void Close()
    {
        isOpen = false;
        foodIndicator.gameObject.SetActive(false);
        //do closing indicator here
    }

    private void SpawnFood()
    {
        GameObject newFood;
        if (currentFood.prefab == null)
        {
            newFood = foodPool.Pull();
        }
        else
        {
            newFood = Instantiate(currentFood.prefab);
        }
        newFood.transform.SetParent(null, false);
        newFood.transform.position = foodIndicator.transform.position;
        Manager.Instance.allPurchasables[currentFood]--;
        Manager.Instance.onQuantityChange.Invoke();
        FoodBehaviour behaviour = newFood.GetComponent<FoodBehaviour>();
        behaviour.food = currentFood;
        behaviour.Use += RemoveFood;
        newFood.SetActive(true);
        if (OnFoodPlaced != null && newFood != null)
        {
            OnFoodPlaced(newFood);
        }
    }

    void RemoveFood(FoodBehaviour food)
    {
        foodPool.ReturnRigidbody(food.gameObject);
        food.Use -= RemoveFood;
        food.gameObject.SetActive(false);
        food.food = null;
    }
}
