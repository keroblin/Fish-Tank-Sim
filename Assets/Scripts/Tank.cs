using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    //gonna move over some tank stat specific manager stuff in here to make it easier to have multiple tanks
    public Bounds tankBounds;
    public bool drawBounds;

    public float tickInterval = 5f;
    public float currentTime;

    bool useDefault = true;
    public float tankDirtiness = Manager.baseDirt;
    public float tankHarmony = 0;
    public float tankPh = Manager.basePh;
    [Range(0f, 1f)]
    public float tankLight = Manager.baseLight;
    public float tankTemp = Manager.baseTemp;
    public float tankHardness = Manager.baseHardness;
    public float value = 0.0f;

    public Item assignedSubstrate;
    public Item nullSubstrate;
    public MeshFilter substrateMesh;
    public MeshRenderer substrateRenderer;
    public Material glass;

    public UnityEvent onStatUpdate;
    public UnityEvent onTankTick;

    public Request assignedRequest;

    public List<Placeable> assignedPlaceables;

    public Dictionary<Tag, int> tags = new Dictionary<Tag, int>();
    public Tag mostCommonStyle = null;
    public Fish mostCommonFishType = null;
    private void OnDrawGizmos()
    {
        //tank bounds testing
        if (drawBounds)
        {
            Gizmos.color = new Color(255, 0, 255, .3f);
            Gizmos.DrawCube(tankBounds.center, tankBounds.size);
        }
    }

    private void Start()
    {
        if (useDefault)
        {
            tankPh = Manager.basePh;
            tankLight = Manager.baseLight;
            tankTemp = Manager.baseTemp;
            tankHardness = Manager.baseHardness;
        }
        Manager.Instance.onQuantityChange.AddListener(UpdateValue);
        this.onStatUpdate.AddListener(StatUpdate);
    }

    public void StatUpdate()
    {
        //update all the sliders
        tankDirtiness += FishManager.instance.GetFishPoo();
        Manager.Instance.dirtSlider.value = tankDirtiness;
        Manager.Instance.harmonySlider.value = FishManager.instance.GetFishHarmony();
        //glass.dirtiness = tankDirtiness; //do this for a dirtiness material
        //style handles itself when items are added or removed
        UpdateValue();
    }

    public void UpdateValue()//calculate the value of the tank (the value of the items within, then subtract any fish health issues and dirtiness, or add if fish are really happy and the tank is healthy
    {
        float updatedVal = 0;
        foreach (Placeable placeable in assignedPlaceables) //change the inventory bit to be like a held items list in here
        {
            updatedVal += placeable.purchasable.price * Manager.Instance.allPurchasables[placeable.purchasable];
            //add in a modifier for like tank quality too later
        }
        if (assignedSubstrate)
        {
            updatedVal += assignedSubstrate.price;
        }
        value = updatedVal;
        Manager.Instance.valueText.text = "£"+ value.ToString("#.00");
    }

    void UpdateStyle()
    {
        foreach(KeyValuePair<Tag,int> tag in tags)
        {
            if(mostCommonStyle == null)
            {
                mostCommonStyle = tag.Key;
            }
            if(tag.Value > tags[mostCommonStyle])
            {
                mostCommonStyle = tag.Key;
            }
        }

        if (mostCommonStyle != null)
        {
            Manager.Instance.styleIcon.sprite = mostCommonStyle.icon;
        }
    }

    public void AddModifiers(Item item)
    {
        tankPh += item.pHMod;
        tankTemp += item.tempMod;
        tankHardness += item.dGHMod;
        tankLight += item.lightMod;
        foreach (Tag tag in item.tags)
        {
            if (tags == null || !tags.ContainsKey(tag))
            {
                tags.Add(tag, 1);
            }
            else
            {
                tags[tag]++;
            }
        }
        UpdateStyle();
        onStatUpdate.Invoke();
    }
    public void RemoveModifiers(Item item)
    {
        tankPh -= item.pHMod;
        tankTemp -= item.tempMod;
        tankHardness -= item.dGHMod;
        tankLight -= item.lightMod;
        foreach (Tag tag in item.tags)
        {
            if (tags.ContainsKey(tag))
            {
                tags[tag]--;
            }
        }
        UpdateStyle();
        onStatUpdate.Invoke();
    }

    public void SwapSubstrate(Item item)
    {
        if (assignedSubstrate)
        {
            RemoveModifiers(assignedSubstrate);
        }
        assignedSubstrate = item;
        if (item == null)
        {
            assignedSubstrate = new Substrate();
        }
        AddModifiers(assignedSubstrate);
        substrateRenderer.material = assignedSubstrate.material;
        substrateMesh.mesh = assignedSubstrate.model;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= tickInterval)
        {
            //Debug.Log("Tick!");
            onTankTick.Invoke();
            onStatUpdate.Invoke();
            currentTime = 0;

            //temporary! need a global tick to stop people swapping tanks and being fine
            Manager.Instance.currentMoney += Manager.Instance.hourlyRate;
            //might make it so the tick is just based on realtime
        }
    }

    public void AddRottenFood()
    {
        if(tankDirtiness < Manager.Instance.dirtSlider.maxValue)
        {
            tankDirtiness++;
            StatUpdate();
        }
    }

    public void CleanTank()
    {
        Curtain.Instance.CurtainDown("Cleaning",1f);
        tankDirtiness = 0;
        StatUpdate();
    }

    public IEnumerator ResetTank()
    {
        //remove listeners as needed here
        Debug.Log("resetting...");
        Manager.Instance.onQuantityChange.RemoveListener(UpdateValue);
        value = 0;
        List<Placeable> allAssignedPlaceables = new List<Placeable>(); //they get removed as we iterate
        allAssignedPlaceables.AddRange(assignedPlaceables);
        assignedPlaceables.Clear();
        UnityEvent destroy = new UnityEvent();
        foreach (Placeable placeable in allAssignedPlaceables) //maybe have these as functionality in the behaviour? in ondestroy? works for now though
        {
            destroy.AddListener(placeable.SendOff);
        }
        destroy.Invoke();
        if (assignedSubstrate)
        {
            Manager.Instance.allPurchasables[assignedSubstrate]--;
            SwapSubstrate(nullSubstrate);
        }
        Manager.Instance.onQuantityChange.Invoke();
        Manager.Instance.onQuantityChange.AddListener(UpdateValue);
        tankPh = Manager.basePh;
        tankLight = Manager.baseLight;
        tankTemp = Manager.baseTemp;
        tankHardness = Manager.baseHardness;
        tankDirtiness = Manager.baseDirt;
        tankHarmony = 0;
        onStatUpdate.Invoke();
        yield return true;
    }
}
