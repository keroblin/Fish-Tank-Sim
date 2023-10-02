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
    //public GameObject fishGrid;
    public RectTransform fishViewTransform;
    public GameObject fishView;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishDescription;
    public FishStatView statView;
    public List<FishUI> fishUIs;
    public List<Fish> fish;

    void Start()
    {
        Manager.Instance.currentTank.onStatUpdate.AddListener(Sort);
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
        fishUIs = sortedUIs.OrderBy(x => x.fish.CalculateCompat()).ToList();
        for(int i =0; i< fishUIs.Count; i++)
        {
            fishUIs[i].gameObject.transform.SetSiblingIndex(i);
        }
    }

    void Set(FishUI fishUI)
    {
        fishView.SetActive(true);
        Fish fish = fishUI.fish;
        Vector3 targetPos = new Vector3(fishUI.transform.position.x, fishView.transform.position.y, 0f);
        //todo: put a way to stop it going over the edge here
        //maybe add the increment plus the index of the array? that feels bad though :/
        fishView.transform.position = targetPos;
        /*Debug.Log(fishViewTransform.anchoredPosition);
        if ((fishViewTransform.anchoredPosition + fishViewTransform.sizeDelta).x < 0)
        {
            fishViewTransform.anchoredPosition = new Vector2(0+ (fishViewTransform.sizeDelta.x*2), fishViewTransform.anchoredPosition.y);
        }*/
        fishName.text = fish.name;
        fishDescription.text = fish.description;
        statView.Set(fish);

    }
    void Unset(FishUI fishUI)
    {
        fishView.SetActive(false);
    }
}
