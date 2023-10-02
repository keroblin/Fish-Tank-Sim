using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishStatView : MonoBehaviour
{
    public TextMeshProUGUI fishPhText;
    public Image fishPhIcon;
    public TextMeshProUGUI fishHardnessText;
    public Image fishHardnessIcon;
    public TextMeshProUGUI fishTempText;
    public Image fishTempIcon;
    public TextMeshProUGUI fishLightText;
    public Image fishLightIcon;

    public void Set(Fish fish)
    {
        fishPhText.text = "pH: " + fish.minPH.ToString() + " - " + fish.maxPH.ToString();
        fishPhIcon.sprite = fish.GetCompatIcon(fish.GetCompat(fish.minPH, fish.maxPH, Manager.Instance.currentTank.tankPh));
        fishHardnessText.text = "Water Hardness: " + fish.minHardness.ToString() + "dGH - " + fish.maxHardness.ToString() + "dGH";
        fishHardnessIcon.sprite = fish.GetCompatIcon(fish.GetCompat(fish.minHardness, fish.maxHardness, Manager.Instance.currentTank.tankHardness));
        fishTempText.text = "Temperature: " + fish.minTemp.ToString() + "f - " + fish.maxTemp.ToString() + "f";
        fishTempIcon.sprite = fish.GetCompatIcon(fish.GetCompat(fish.minTemp, fish.maxTemp, Manager.Instance.currentTank.tankTemp));
        fishLightText.text = "Light: " + (fish.minLight * 100f).ToString() + "% - " + (fish.maxLight * 100f).ToString() + "%";
        fishLightIcon.sprite = fish.GetCompatIcon(fish.GetCompat(fish.minLight, fish.maxLight, Manager.Instance.currentTank.tankLight));
    }
}
