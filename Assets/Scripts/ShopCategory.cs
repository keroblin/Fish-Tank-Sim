using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCategory : Category
{
    public ItemList itemList;
    public override void ToggleOn(Category category = null)
    {
        gameObject.SetActive(true);
    }
    public override void ToggleOff(Category category = null)
    {
        if (!(category is ShopCategory))
        {
            gameObject.SetActive(false);
        }
    }
}
