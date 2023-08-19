using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Manager : MonoBehaviour
{
    //a class that is singleton to store essential variables for many different types of objects and situations
    public static Manager Instance;
    public PlacingMenu placingMenu;
    public GameObject placingRef;
    const float basePh = 7f, baseLight = .8f, baseTemp = 68f, baseHardness = 7f, baseHygeine = 100f;
    const float totalMoney = 200.00f;
    public float currentMoney = totalMoney;
    public TextMeshProUGUI money;

    public float tankHygeine = baseHygeine;
    public float tankPh = basePh;
    [Range(0f, 1f)]
    public float tankLight = baseLight;
    public float tankTemp = baseTemp;
    public float tankHardness = baseHardness;

    public enum ItemCategories { SUBSTRATE, ORNAMENTS, LIVEPLANTS, HEATING };
    public enum FoodCategories { FLAKES, PELLETS, FROZEN, LIVE, VEG};
    public enum FishCategories { COLDWATER, TROPICAL, BRACKISH };
    public enum FishPersonalities { BOTTOMFEEDER, HUNTER, CASUAL, TERRITORIAL };

    public Animator menuAnim;

    public Sprite sad;
    public Sprite ok;
    public Sprite happy;
    public List<Fish> allFish;
    public List<Purchasable> allPurchasables;
    public List<Purchasable> inventory;
    public Item currentSubstrate;
    public Item nullSubstrate;
    public MeshFilter substrateMesh;
    public MeshRenderer substrateRenderer;

    public UnityEvent onStatUpdate;
    public UnityEvent onBuy;
    public UnityEvent onSell;

    public Pool purchasableUiPool;

    public Bounds tankBounds;

    bool useDefault = true;

    private void OnDrawGizmos()
    {
        //tank bounds testing
        Gizmos.color = new Color(255,0,255,.3f);
        Gizmos.DrawCube(tankBounds.center, tankBounds.size);
    }

    private void Awake()
    {
        //replace these w saved values
        currentMoney = totalMoney;
        if (useDefault)
        {
            tankPh = basePh;
            tankLight = baseLight;
            tankTemp = baseTemp;
            tankHardness = baseHardness;
        }
        Instance = this;
    }
    private void Start()
    {
        onStatUpdate.Invoke();
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
    }

    public void AddModifiers(Item item)
    {
        tankPh += item.pHMod;
        tankTemp += item.tempMod;
        tankHardness += item.dGHMod;
        tankLight += item.lightMod;
        onStatUpdate.Invoke();
    }
    public void RemoveModifiers(Item purchasable)
    {
        tankPh -= purchasable.pHMod;
        tankTemp -= purchasable.tempMod;
        tankHardness -= purchasable.dGHMod;
        tankLight -= purchasable.lightMod;
        onStatUpdate.Invoke();
    }

    public void Buy(Purchasable purchasable)
    {
        if (currentMoney - purchasable.price > 0.00 && !inventory.Contains(purchasable))
        {
            currentMoney -= purchasable.price;
            inventory.Add(purchasable);
        }
        else
        {
            if (inventory.Contains(purchasable))
            {
                print("Already bought");
            }
            else
            {
                print("Not enough money!");
            }
        }
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
        onBuy.Invoke();
    }

    public void Sell(Purchasable purchasable)
    {
        if (inventory.Contains(purchasable))
        {
            inventory.Remove(purchasable);
            currentMoney += purchasable.price;
        }
        money.text = "Your cash: £" + currentMoney.ToString("#.00");
        onSell.Invoke();
    }

    public void SwapSubstrate(Item item)
    {
        if (currentSubstrate)
        {
            RemoveModifiers(currentSubstrate);
        }
        currentSubstrate = item;
        if (item == nullSubstrate)
        {
            currentSubstrate.model = null;
        }
        AddModifiers(currentSubstrate);
        substrateRenderer.material = currentSubstrate.material;
        substrateMesh.mesh = currentSubstrate.model;
    }
}