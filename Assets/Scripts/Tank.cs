using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    //gonna move over some tank stat specific manager stuff in here to make it easier to have multiple tanks
    public Bounds tankBounds;
    public bool drawBounds;

    public float hygieneInterval;
    public float digestionInterval;
    public float illnessInterval;
    public float currentTime;

    public float tankHygeine = Manager.baseHygeine;
    public float tankPh = Manager.basePh;
    [Range(0f, 1f)]
    public float tankLight = Manager.baseLight;
    public float tankTemp = Manager.baseTemp;
    public float tankHardness = Manager.baseHardness;

    public Item currentSubstrate;
    public Item nullSubstrate;
    public MeshFilter substrateMesh;
    public MeshRenderer substrateRenderer;

    public Slider harmony;
    public Slider cleanliness;
    public Image style;
    public Slider valueSlider;
    public TextMeshProUGUI valueText;

    public UnityEvent onStatUpdate;
    bool useDefault = true;
    private void OnDrawGizmos()
    {
        //tank bounds testing
        if (drawBounds)
        {
            Gizmos.color = new Color(255, 0, 255, .3f);
            Gizmos.DrawCube(tankBounds.center, tankBounds.size);
        }
    }

    private void Awake()
    {
        if (useDefault)
        {
            tankPh = Manager.basePh;
            tankLight = Manager.baseLight;
            tankTemp = Manager.baseTemp;
            tankHardness = Manager.baseHardness;
        }
    }

    public void AddModifiers(Item item)
    {
        tankPh += item.pHMod;
        tankTemp += item.tempMod;
        tankHardness += item.dGHMod;
        tankLight += item.lightMod;
        onStatUpdate.Invoke();
    }
    public void RemoveModifiers(Item item)
    {
        tankPh -= item.pHMod;
        tankTemp -= item.tempMod;
        tankHardness -= item.dGHMod;
        tankLight -= item.lightMod;
        onStatUpdate.Invoke();
    }

    public void SwapSubstrate(Item item)
    {
        if (currentSubstrate)
        {
            RemoveModifiers(currentSubstrate);
        }
        currentSubstrate = item;
        if (item == null)
        {
            currentSubstrate = new Substrate();
        }
        AddModifiers(currentSubstrate);
        substrateRenderer.material = currentSubstrate.material;
        substrateMesh.mesh = currentSubstrate.model;
    }

    private void Update()
    {
        currentTime += Time.deltaTime; //probably do something different so this number doesnt get infinitely high

        if(currentTime >= hygieneInterval)
        {
            //do function stuff here
            //reset timer
        }
    }

    /*
    while (liveFish.Count > 0)
        {
            foreach (FishBehaviour fish in liveFish)
            {
                if (fish.happiness > 2)
                {
                    fish.hunger -= .5f;
                }
                else
                {
                    fish.hunger -= 1f;
                }
            }
            yield return new WaitForSecondsRealtime(180f);
        }
    */
}
