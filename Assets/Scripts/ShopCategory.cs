using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCategory : Category
{
    public ItemList itemList;
    public override void ToggleOn(Category category = null)
    {
        if(!Manager.Instance.menuAnim.GetCurrentAnimatorStateInfo(0).IsName("ShopIn"))
        {
            Manager.Instance.menuAnim.Play("ShopIn");
        }
    }
    public override void ToggleOff(Category category = null)
    {
        if (!(category is ShopCategory))
        {
            if(!Manager.Instance.menuAnim.GetCurrentAnimatorStateInfo(0).IsName("ShopOut"))
            {
                Manager.Instance.menuAnim.Play("ShopOut");
            }
        }
    }
}
