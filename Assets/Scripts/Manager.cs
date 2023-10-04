using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour, ISaving
{
    //a class that is singleton to store essential variables for many different types of objects and situations
    public static Manager Instance;

    public Pool itemPool;
    public Pool fishAiPool;
    public Pool foodPool;

    public AudioSource universalSFX;
    public AudioSource universalMusic;

    public GameObject placingRef;
    public const float basePh = 7f, baseLight = .8f, baseTemp = 68f, baseHardness = 7f, baseDirt = 0f;
    public const float totalMoney = 200.00f;
    public float currentMoney = totalMoney;
    public float hourlyRate = 5.00f;
    public TextMeshProUGUI money;

    public Tank currentTank;

    public enum FishPersonalities { BOTTOMFEEDER, HUNTER, CASUAL, TERRITORIAL };

    public Animator menuAnim;

    public Sprite sad;
    public Sprite ok;
    public Sprite happy;
    public AudioClip talkSound;
    public List<Fish> allFish;
    public List<Purchasable> allPurchasableSOs;
    public List<Request> allRequestSOs;
    public Dictionary<Purchasable,int> allPurchasables = new Dictionary<Purchasable, int>(); //ask everything else when grabbing quantity to do it from here
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

    public GameObject tankPrefab;

    private void OnEnable()
    {
        Saving.savers.Add(this);
    }
    private void OnDestroy()
    {
        Saving.savers.Remove(this);
    }
    public void Save()
    {
        Saving.currentSave.money = currentMoney;
        SerializeTools.SerializeableDictionary<string, int> serializedInventory = new();
        foreach (KeyValuePair<Purchasable,int> entry in allPurchasables)
        {
            serializedInventory.Add(entry.Key.name, entry.Value);
        }
        Saving.currentSave.inventory = serializedInventory;
    }
    public void Load()
    {
        currentMoney = Saving.currentSave.money;
        foreach(KeyValuePair<string, int> entry in Saving.currentSave.inventory)
        {
            Purchasable purchasable = allPurchasableSOs.Find(x => x.name == entry.Key);
            if (allPurchasables.ContainsKey(purchasable))
            {
                allPurchasables[purchasable] = entry.Value;
            }
            else
            {
                allPurchasables.Add(purchasable, entry.Value);
            }
        }
    }

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
        Saving.Load();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Saving.Save();
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Saving.Load();
        }
    }

    public void Buy(Purchasable purchasable)
    {
        if (currentMoney - purchasable.price > 0.00) //do something here to deal with stacking
        {
            if (!allPurchasables.ContainsKey(purchasable) || allPurchasables[purchasable] == 0)
            {
                //Debug.Log("Inventory did not contain purchasable, adding it");
                allPurchasables[purchasable]++;
            }
            else if (purchasable.stackable)
            {
                //Debug.Log("Stackable! Adding to quantity...");
                allPurchasables[purchasable]++;
                //Debug.Log("Quantity is " + allPurchasables[purchasable].ToString());
            }
            else if (!purchasable.stackable && (allPurchasables.ContainsKey(purchasable) || allPurchasables[purchasable] > 0))
            {
                //Debug.Log("Already bought!");
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
            allPurchasables.Remove(purchasable);
        }

        money.text = "Your cash: £" + currentMoney.ToString("#.00");
        onSell.Invoke();
        onQuantityChange.Invoke();
    }
}