using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class FishUI : MonoBehaviour
{
    //want to make a nice static ref for the fish icons somewhere eventually, but this works for now
    //the stat sliders and stuff should be held in the fishStatsUI script and reffed there
    public Image fishIcon;
    public Image fishHappiness;
    public Fish fish;
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
}
