using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMenu : MonoBehaviour
{
    public List<Food> FoodList;
    public List<PurchasableUI> foodButtons;
    public List<PurchasableUI> hotbarSelection;
    public ShopDetail view;

    public GameObject hotbar;

    bool mouseClicked;
    void Open()
    {
        hotbar.SetActive(true);
    }
    void Close()
    {
        hotbar.SetActive(false);
    }
    void Select()
    {
        //tell feeding it can feed
    }

    void UpdateList()
    {

    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //check if mouse is still held after leaving button
        //if so, as soon as it exits, show an indicator of the food's icon on the cursor
        //when you click, placeables are spawned and the bag is tipped
    }

    //get all the foods
    //have a selected foods and assign them to buttons, if there isnt a food assigned, then turn it off
    //click and drag selected foods into the food selection
    //when clicked, make the cursor the icon for the food and allow food to be placed, until the food menu is closed
    //use the quantities bought in shop to manage this

}
