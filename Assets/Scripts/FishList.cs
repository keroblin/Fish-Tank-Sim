using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishList:MonoBehaviour
{
    public Pool fishPool;
    public GameObject fishUiPrefab;
    public GameObject fishView;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishDescription;
    public TextMeshProUGUI fishPhText;
    public Image fishPhIcon;
    public TextMeshProUGUI fishHardnessText;
    public Image fishHardnessIcon;
    public TextMeshProUGUI fishTempText;
    public Image fishTempIcon;
    public TextMeshProUGUI fishLightText;
    public Image fishLightIcon;
    public List<FishUI> fishUIs;
    public List<Fish> fish;

    void Start()
    {
        Manager.Instance.onStatUpdate.AddListener(Sort);
        fish = Manager.Instance.allFish;
        foreach (Fish fish in fish)
        {
            GameObject instance = fishPool.Pull();
            FishUI ui = instance.GetComponent<FishUI>();
            ui.fish = fish;
            ui.Set();
            ui.entered.AddListener(delegate { Set(ui); });
            ui.exit.AddListener(delegate { Unset(ui);});
            instance.transform.SetParent(transform);
            fishUIs.Add(ui);
        }
        Sort();
    }

    void Sort() //better way to do this im sure
    {
        List<FishUI> sortedUIs = fishUIs;
        fishUIs = sortedUIs.OrderBy(x => x.fish.CalculateHappiness()).ToList();
        for(int i =0; i< fishUIs.Count; i++)
        {
            fishUIs[i].gameObject.transform.SetSiblingIndex(i);
        }
    }

    void Set(FishUI fishUI)
    {
        fishView.SetActive(true);
        Fish fish = fishUI.fish;
        Vector3 targetPos = new Vector3(fishUI.transform.localPosition.x, fishView.transform.localPosition.y, fishView.transform.localPosition.z);
        //todo: put a way to stop it going over the edge here
        fishView.transform.localPosition = targetPos;
        fishName.text = fish.name;
        fishDescription.text = fish.fishDescription;
        fishPhText.text = "pH: " + fish.minPH.ToString() + " - " + fish.maxPH.ToString();
        fishPhIcon.sprite = fish.GetHappinessIcon(fish.GetCompat(fish.minPH, fish.maxPH, Manager.Instance.tankPh));
        fishHardnessText.text = "Water Hardness: " + fish.minHardness.ToString() + "dGH - " + fish.maxHardness.ToString() + "dGH";
        fishHardnessIcon.sprite = fish.GetHappinessIcon(fish.GetCompat(fish.minHardness, fish.maxHardness, Manager.Instance.tankHardness));
        fishTempText.text = "Temperature: " + fish.minTemp.ToString() + "f - " + fish.maxTemp.ToString() +"f";
        fishTempIcon.sprite = fish.GetHappinessIcon(fish.GetCompat(fish.minTemp,fish.maxTemp,Manager.Instance.tankTemp));
        fishLightText.text = "Light: " + (fish.minLight * 100f).ToString() + "% - " + (fish.maxLight*100f).ToString() + "%";
        fishLightIcon.sprite = fish.GetHappinessIcon(fish.GetCompat(fish.minLight, fish.maxLight, Manager.Instance.tankLight));
    }
    void Unset(FishUI fishUI)
    {
        fishView.SetActive(false);
    }
}
