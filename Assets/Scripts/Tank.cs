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

    public float tickInterval = 50f;
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

    public Slider harmonySlider;
    public Slider hygeineSlider;
    public Image styleIcon;
    public Slider valueSlider;
    public TextMeshProUGUI valueText;

    public UnityEvent onStatUpdate;
    public UnityEvent onTankTick;
    bool useDefault = true;

    public List<Placeable> assignedItems;
    public List<FishBehaviour> assignedFish;

    public Dictionary<Tag, int> tags = new Dictionary<Tag, int>();
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
        Manager.Instance.onQuantityChange.AddListener(UpdateValue);
        this.onStatUpdate.AddListener(StatUpdate);
    }

    public void StatUpdate()
    {
        //update all the sliders
        tankHygeine -= FishManager.instance.GetFishPoo();
        hygeineSlider.value = tankHygeine;
        harmonySlider.value = FishManager.instance.GetFishHarmony();
        //style handles itself when items are added or removed
        UpdateValue();
    }

    public void UpdateValue()//calculate the value of the tank (the value of the items within, then subtract any fish health issues and dirtiness, or add if fish are really happy and the tank is healthy)

    {
        float value = 0.0f;
        foreach (Purchasable purchasable in Manager.Instance.inventory) //change the inventory bit to be like a held items list in here
        {
            value += purchasable.price * Manager.Instance.allPurchasables[purchasable];
            //add in a modifier for like tank quality too later
        } 
        valueText.text = "£"+ value.ToString("#.00");
    }

    void UpdateStyle()
    {
        Tag mostCommon = null;
        foreach(KeyValuePair<Tag,int> tag in tags)
        {
            if(mostCommon == null)
            {
                mostCommon = tag.Key;
            }
            if(tag.Value > tags[mostCommon])
            {
                mostCommon = tag.Key;
            }
        }

        if (mostCommon != null)
        {
            styleIcon.sprite = mostCommon.icon;
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
        currentTime += Time.deltaTime;

        if(currentTime >= tickInterval)
        {
            onTankTick.Invoke();
            onStatUpdate.Invoke();
            currentTime = 0;
        }
    }

    public void AddRottenFood()
    {
        if(tankHygeine > 0)
        {
            tankHygeine -= 10;
        }
    }
}
