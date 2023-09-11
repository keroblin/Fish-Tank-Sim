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
        Manager.Instance.currentTank.onStatUpdate.AddListener(UpdateStats);
        UpdateStats();
    }
    public void UpdateStats()
    {
        ph.value = Manager.Instance.currentTank.tankPh;
        temp.value = Manager.Instance.currentTank.tankTemp;
        lightLevel.value = Manager.Instance.currentTank.tankLight;
        hardness.value = Manager.Instance.currentTank.tankHardness;
    }
}
