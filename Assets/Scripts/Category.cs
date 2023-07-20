using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Category : MonoBehaviour
{
    public Button button;
    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    public void Toggle(Category category = null)
    {
        if (this != category)
        {
            if (button.interactable)
            {
                return;
            }
            else
            {
                onDeselect.Invoke();
                button.interactable = true;
                ToggleOff(category);
            }
        }
        else
        {
            onSelect.Invoke();
            button.interactable = false;
            ToggleOn(category);
        }
    }

    public virtual void ToggleOn(Category category = null) 
    {
        gameObject.SetActive(true);
    }
    public virtual void ToggleOff(Category category = null)
    {
        gameObject.SetActive(false);
    }
}
