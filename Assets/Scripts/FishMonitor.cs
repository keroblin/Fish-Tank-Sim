using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishMonitor : MonoBehaviour
{
    public GameObject visuals;
    public static FishMonitor instance;
    public Camera fishCam;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishComment;
    public Slider happiness;
    public Slider hunger;
    void Start()
    {
        instance = this;
    }

    public void Set(FishBehaviour fishB)
    {
        visuals.SetActive(true);
        fishName.text = fishB.fish.name;
        happiness.value = fishB.happiness; //maybe do this in an event or something so its always up to date
        hunger.value = fishB.hunger;
        fishComment.text = "Not sure how to implement this yet!";
        fishCam.transform.SetParent(fishB.gameObject.transform, false);
        fishCam.transform.localPosition = Vector3.zero;
        fishCam.transform.localPosition = new Vector3(-(fishB.fish.model.bounds.size.x * fishB.meshFilter.gameObject.transform.localScale.x) * 4, 0f, 0f);
    }
}
