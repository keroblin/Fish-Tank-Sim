using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishList:MonoBehaviour
{
    public GameObject fishUiPrefab;
    public List<FishUI> fishUIs;
    public List<Fish> fish;
    void Start()
    {
        //instantiate the uis
        //connect their buttons up
        foreach (Fish fish in fish)
        {
            GameObject instance = Instantiate(fishUiPrefab);
            FishUI ui = instance.GetComponent<FishUI>();
            ui.fish = fish;
            ui.Set();
            instance.transform.SetParent(transform);
        }
    }
}
