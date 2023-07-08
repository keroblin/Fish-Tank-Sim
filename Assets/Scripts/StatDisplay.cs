using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public Slider ph;
    public Slider temp;
    public Slider lightLevel;
    public Slider hardness;
    private void Start()
    {
        Manager.Instance.onStatUpdate.AddListener(UpdateStats);
        UpdateStats();
    }
    public void UpdateStats()
    {
        ph.value = Manager.Instance.tankPh;
        temp.value = Manager.Instance.tankTemp;
        lightLevel.value = Manager.Instance.tankLight;
        hardness.value = Manager.Instance.tankHardness;
    }
}
