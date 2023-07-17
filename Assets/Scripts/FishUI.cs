using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FishUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image fishIcon;
    public Image fishHappiness;
    public Fish fish;
    public UnityEvent entered;
    public UnityEvent exit;

    public void Set()
    {
        Manager.Instance.onStatUpdate.AddListener(UpdateStats);
        fishIcon.sprite = fish.fishIcon;
        UpdateStats();
    }

    public void UpdateStats()
    {
        fishHappiness.sprite = fish.GetHappinessIcon();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse entered " + fish.fishName);
        entered.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exit " + fish.fishName);
        exit.Invoke();
    }
}
