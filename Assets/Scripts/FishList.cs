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
    public Slider fishPhMin;
    public Slider fishPhMax;
    public Slider fishHardnessMin;
    public Slider fishHardnessMax;
    public Slider fishTempMin;
    public Slider fishTempMax;
    public Slider fishLightMin;
    public Slider fishLightMax;
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

        //fishPhMin.maxValue = fish.maxPH / 2;
        //fishPh
    }
    void Unset(FishUI fishUI)
    {
        fishView.SetActive(false);
    }
}
