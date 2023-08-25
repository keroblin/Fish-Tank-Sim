using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CategoryList : MonoBehaviour
{
    public List<Category> categories = new List<Category>();
    Category currentCategory;

    //to implement! was buggy
    //auto connect category buttons to their objects to toggle
    void OnEnable()
    {
        for (int i = 0; i < categories.Count; i++)
        {
            int index = i;
            Category workingCategory = categories[index];
            workingCategory.button.onClick.AddListener(delegate { SwapCategory(index, workingCategory); });
        }
    }

    public void SwapCategory(int index = 0, Category category = null) //workaround for weird corruption, categories were out of range in event connections only on start
    {
        //Debug.Log("Swapping to number " + index.ToString());
        //currentCategory = categories[index];
        if(category == null && index >= 0)
        {
            if(index == -1) //intentional null
            {
                currentCategory = null;
            }
            else
            {
                //Debug.Log("Category was null, using index");
                currentCategory = categories[index];
            }
        }
        else
        {
            currentCategory = category;
        }
        foreach (Category c in categories)
        {
            //Debug.Log("Toggling " + c.name + " " + (c == currentCategory).ToString());
            c.Toggle(currentCategory);
        }
    }
}
