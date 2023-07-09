using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Manager : MonoBehaviour
{
    //a class that is singleton to store essential variables for many different types of objects and situations
    public static Manager Instance;
    public PlacingMenu placingMenu;
    const float basePh = 7f, baseLight = .5f, baseTemp = 68f, baseHardness = 7f;
    const float totalMoney = 200.00f;
    public float currentMoney = totalMoney;


    public float tankPh = basePh;
    [Range(0f, 1f)]
    public float tankLight = baseLight;
    public float tankTemp = baseTemp;
    public float tankHardness = baseHardness;

    public Sprite sad;
    public Sprite ok;
    public Sprite happy;

    public List<Purchasable> allPurchasables;
    public List<Purchasable> inventory;
    public List<Placeable> placedObjects;

    public UnityEvent onStatUpdate;
    public UnityEvent onBuy;
    public UnityEvent onSell;

    bool useDefault = false;

    private void Awake()
    {
        //replace these w saved values
        currentMoney = totalMoney;
        if(useDefault)
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
    }

    public void OnPlaceablePlaced(Placeable placeable)
    {
        placedObjects.Add(placeable);
        tankPh += placeable.purchasable.pHMod;
        tankTemp += placeable.purchasable.tempMod;
        tankHardness += placeable.purchasable.dGHMod;
        tankLight += placeable.purchasable.lightMod;
        onStatUpdate.Invoke();
    }
    public void OnPlaceableRemoved(Placeable placeable)
    {
        placedObjects.Remove(placeable);
        tankPh -= placeable.purchasable.pHMod;
        tankTemp -= placeable.purchasable.tempMod;
        tankHardness -= placeable.purchasable.dGHMod;
        tankLight -= placeable.purchasable.lightMod;
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
            print("Not enough money!");
        }
        onBuy.Invoke();
    }

    public void Sell(Purchasable purchasable)
    {
        if (inventory.Contains(purchasable))
        {
            inventory.Remove(purchasable);
            currentMoney += purchasable.price;
        }
        onSell.Invoke();
    }
}
