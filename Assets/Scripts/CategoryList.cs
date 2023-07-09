using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CategoryList : MonoBehaviour
{
    public List<GameObject> categoryUIs = new List<GameObject>();
    public List<Button> categoryButtons = new List<Button>();

    private void Start()
    {
        OnReady();
    }

    public virtual void OnReady()
    {
    }

    //to implement! was buggy
    //auto connect category buttons to their objects to toggle

    /*void Start()
    {
        for (int i = 0; i < categoryUIs.Count-1; i++)
        {
            //Debug.Log(categoryUIs[i].name + " is number " + i.ToString());
            //categoryButtons[i].onClick.AddListener(delegate { SwapCategory(i); });
            //Debug.Log("connecting " + categoryButtons[i].name + " to " + i.ToString());
        }
    }*/

    public void SwapCategory(int index)
    {
        //Debug.Log("Swapping to number " + index.ToString());
        for (int i = 0; i < categoryUIs.Count; i++)
        {
            if (i == index)
            {
                categoryUIs[i].SetActive(true);
                categoryButtons[i].interactable = false;
            }
            else
            {
                categoryButtons[i].interactable = true;
                categoryUIs[i].SetActive(false);
            }
        }
    }
}
