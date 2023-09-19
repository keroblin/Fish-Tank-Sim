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

    public Item currentSubstrate;
    public Item nullSubstrate;
    public MeshFilter substrateMesh;
    public MeshRenderer substrateRenderer;
    public Material glass;

    public Slider harmonySlider;
    public Slider dirtSlider;
    public Image styleIcon;
    public Slider valueSlider;
    public TextMeshProUGUI valueText;

    public UnityEvent onStatUpdate;
    public UnityEvent onTankTick;

    public Request assignedRequest;

    public List<Placeable> assignedItems;
    public List<FishBehaviour> assignedFish;

    public Dictionary<Tag, int> tags = new Dictionary<Tag, int>();
    public Tag mostCommonStyle = null;

    public Animator animator;
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
        tankDirtiness += FishManager.instance.GetFishPoo();
        dirtSlider.value = tankDirtiness;
        harmonySlider.value = FishManager.instance.GetFishHarmony();
        //glass.dirtiness = tankDirtiness; //do this for a dirtiness material
        //style handles itself when items are added or removed
        UpdateValue();
    }

    public void UpdateValue()//calculate the value of the tank (the value of the items within, then subtract any fish health issues and dirtiness, or add if fish are really happy and the tank is healthy
    {
        foreach (Purchasable purchasable in Manager.Instance.inventory) //change the inventory bit to be like a held items list in here
        {
            value += purchasable.price * Manager.Instance.allPurchasables[purchasable];
            //add in a modifier for like tank quality too later
        } 
        valueText.text = "£"+ value.ToString("#.00");
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
            styleIcon.sprite = mostCommonStyle.icon;
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
            //Debug.Log("Tick!");
            onTankTick.Invoke();
            onStatUpdate.Invoke();
            currentTime = 0;
        }
    }

    public void AddRottenFood()
    {
        if(tankDirtiness < dirtSlider.maxValue)
        {
            tankDirtiness++;
            StatUpdate();
        }
    }

    public void CleanTank()
    {
        animator.Play("CurtainDown");
        tankDirtiness = 0;
        StatUpdate();
    }
}
