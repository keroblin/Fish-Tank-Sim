using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    //a class that is singleton to store essential variables for many different types of objects and situations
    public static Manager Instance;

    public Pool itemPool;
    public Pool fishAiPool;
    public Pool foodPool;

    public GameObject placingRef;
    public const float basePh = 7f, baseLight = .8f, baseTemp = 68f, baseHardness = 7f, baseDirt = 0f;
    public const float totalMoney = 200.00f;
    public float currentMoney = totalMoney;
    public TextMeshProUGUI money;

    public Tank currentTank;

    public enum FishPersonalities { BOTTOMFEEDER, HUNTER, CASUAL, TERRITORIAL };

    public Animator menuAnim;

    public Sprite sad;
    public Sprite ok;
    public Sprite happy;
    public List<Fish> allFish;
    public List<Purchasable> allPurchasableSOs;
    public Dictionary<Purchasable,int> allPurchasables = new Dictionary<Purchasable, int>(); //ask everything else when grabbing quantity to do it from here
    public List<Purchasable> inventory;
    public UnityEvent onBuy;
    public UnityEvent onSell;
    public UnityEvent onQuantityChange;
    public UnityEvent enterEdit;
    public UnityEvent enterView;

    public Pool purchasableUiPool;

    public Camera editCam;
    public Camera viewCam;
    public GameObject editObjs;
    public GameObject viewObjs;
    public GameObject editButton;
    public GameObject viewButton;

    private void Awake()
    {
        //replace these w saved values
        currentMoney = totalMoney;

        foreach (Purchasable purchasable in allPurchasableSOs)
        {
            //replace 0 with saved value here
            int quantity = 0;
            allPurchasables.Add(purchasable,quantity);
        }
        Instance = this;
    }

    private void Start()
    {
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
    }

    public void SwapMode(bool isEditMode)
    {
        viewButton.gameObject.SetActive(isEditMode);
        viewCam.gameObject.SetActive(!isEditMode);
        viewObjs.SetActive(!isEditMode);
        editButton.gameObject.SetActive(!isEditMode);
        editCam.gameObject.SetActive(isEditMode);
        editObjs.SetActive(isEditMode);

        if (isEditMode)
        {
            viewCam.tag = "Untagged";
            editCam.tag = "MainCamera";
        }
        else
        {
            editCam.tag = "Untagged";
            viewCam.tag = "MainCamera";
        }
    }

    public void Buy(Purchasable purchasable)
    {
        if (currentMoney - purchasable.price > 0.00) //do something here to deal with stacking
        {
            if (!inventory.Contains(purchasable))
            {
                Debug.Log("Inventory did not contain purchasable, adding it");
                inventory.Add(purchasable);
                allPurchasables[purchasable]++;
            }
            else if (purchasable.stackable)
            {
                Debug.Log("Stackable! Adding to quantity...");
                allPurchasables[purchasable]++;
                Debug.Log("Quantity is " + allPurchasables[purchasable].ToString());
            }
            else
            {
                Debug.Log("Already bought!");
                return;
            }
        }
        currentMoney -= purchasable.price;
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
        onBuy.Invoke();
        onQuantityChange.Invoke();
    }

    public void Sell(Purchasable purchasable)
    {
        if (allPurchasables[purchasable] > 0)
        {
            allPurchasables[purchasable]--;
            currentMoney += purchasable.price;
        }

        if (allPurchasables[purchasable] <=0) 
        { 
            inventory.Remove(purchasable);
        }

        money.text = "Your cash: £" + currentMoney.ToString("#.00");
        onSell.Invoke();
        onQuantityChange.Invoke();
    }

    public void SubmitTankRequest()
    {
        //review tank
        //if its accepted send it off
        //if not, go back to the tank we're in and show a 'poof' effect on the request
        //add any bonuses
        //send stuff to request ui to turn on and off
    }
    public void CancelTankRequest()
    {

    }
}