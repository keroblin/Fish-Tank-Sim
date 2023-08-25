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
        if(category != null)
        {
            if (this != category)
            {
                if (!button.interactable)
                {
                    onDeselect.Invoke();
                    button.interactable = true;
                    this.ToggleOff();
                }
            }
            else
            {
                onSelect.Invoke();
                button.interactable = false;
                this.ToggleOn();
            }
        }
        else
        {
            onDeselect.Invoke();
            button.interactable = true;
            this.ToggleOff();
        }
        
    }

    public virtual void ToggleOn() 
    {
        //Debug.Log("Toggled on " + name);
        gameObject.SetActive(true);
    }
    public virtual void ToggleOff()
    {
        //Debug.Log("Toggled off " + name);
        gameObject.SetActive(false);
    }
}
