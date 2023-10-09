using SerializeTools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tank : MonoBehaviour, ISaving
{
    private void OnEnable()
    {
        Saving.savers.Add(this);
    }
    private void OnDestroy()
    {
        Saving.savers.Remove(this);
    }
    public void Save()
    {
        Saving.currentSave.tankData = new SaveData.TankData(this);
        //need to figure out getting and saving request age
    }
    public void Load()
    {
        SaveData.TankData tankData = Saving.currentSave.tankData;
        if(tankData != null )
        {
            foreach (SaveData.PlaceableData placeableData in tankData.assignedPlaceables)
            {
                Debug.Log("Current placeable: " + placeableData.purchasableName);
                if (assignedPlaceables.Find(x => x.purchasable.name == placeableData.purchasableName && x.transform.position == placeableData.transform.position.GetAsVector3() && x.transform.rotation == placeableData.transform.rotation.GetAsQuaternion()))
                {
                    Debug.Log(placeableData.purchasableName + " already assigned");
                    //if the saved placeable is already here
                    continue;
                }
                //do removing the placeable if its not present in the save here
                else
                {
                    Purchasable purchasable = Manager.Instance.allPurchasableSOs.Find(x => x.name == placeableData.purchasableName);
                    Placeable placedPlaceable = null;
                    if (purchasable is Fish)
                    {
                        Debug.Log("Fish loaded: " + placeableData.purchasableName);
                        Fish fish = purchasable as Fish;
                        fish.Place();
                        placedPlaceable = assignedPlaceables.Find(x => x.purchasable == fish);
                        FishBehaviour fishB = placedPlaceable as FishBehaviour;
                        SaveData.FishData fishData = Saving.currentSave.tankData.fishDatas.Find(x => x.purchasableName == fish.name);
                        fishB.ageInTicks = fishData.age;
                        fishB.happiness = fishData.happiness;
                        fishB.hunger = fishData.hunger;
                        fishB.harmony = fishData.harmony;
                        fishB.currentTargetPosition = fishData.target.GetAsVector3();
                        assignedPlaceables.Add(placedPlaceable);

                    }
                    else if (purchasable is Item)
                    {
                        Debug.Log("Item loaded: " + placeableData.purchasableName);
                        Item item = purchasable as Item;
                        item.Place();
                        placedPlaceable = assignedPlaceables.Find(x => x.purchasable == item);
                        ItemBehaviour placedItem = placedPlaceable as ItemBehaviour;
                        SaveData.ItemData itemData = Saving.currentSave.tankData.itemDatas.Find(x => x.purchasableName == item.name);
                        placedItem.SetColor(itemData.color);
                        assignedPlaceables.Add(placedPlaceable);
                    }
                    placedPlaceable.transform.position = placeableData.transform.position.GetAsVector3();
                    placedPlaceable.transform.rotation = placeableData.transform.rotation.GetAsQuaternion();
                }
            }
            if(tankData.assignedRequestName != "null")
            {
                Request request = Manager.Instance.allRequestSOs.Find(x => x.name == tankData.assignedRequestName);
                assignedRequest = request;
                RequestBehaviour rqBehaviour = FindFirstObjectByType(typeof(RequestBehaviour)).GetComponent<RequestBehaviour>(); 
                rqBehaviour.Set(request);
                rqBehaviour.timeLeft.value = tankData.assignedRequestTime;
            }
            tankDirtiness = tankData.tankDirtiness;
            tankHappiness = tankData.tankHappiness;
            tankPh = tankData.tankPh;
            tankLight = tankData.tankLight;
            tankTemp = tankData.tankTemp;
            tankHardness = tankData.tankHardness;
            value = tankData.value;
            SwapSubstrate(Manager.Instance.allPurchasableSOs.Find(x => x.name == tankData.assignedSubstrate) as Item);
        }
        Manager.Instance.onQuantityChange.Invoke();
        onStatUpdate.Invoke();
    }
    public Bounds tankBounds;
    public bool drawBounds;

    public float tickInterval = 5f;
    public float age;
    public float currentTime;

    bool useDefault = true;
    public float tankDirtiness = Manager.baseDirt;
    public float tankHappiness = 0;
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
    public RequestBehaviour assignedRequestBehaviour;

    public List<Placeable> assignedPlaceables;

    public Dictionary<Tag, int> tags = new Dictionary<Tag, int>();
    public Tag mostCommonStyle = null;
    public Fish mostCommonFishType = null;

    public Slider happinessSlider;
    public Slider dirtSlider;
    public Image styleIcon;
    public Slider valueSlider;
    public TextMeshProUGUI valueText;

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
        dirtSlider.value = tankDirtiness;
        happinessSlider.value = FishManager.instance.overallHappiness;
        //glass.dirtiness = tankDirtiness; //do this for a dirtiness material
        //style handles itself when items are added or removed
        UpdateValue();
    }

    public void UpdateValue()//calculate the value of the tank (the value of the items within, then subtract any fish health issues and dirtiness, or add if fish are really happy and the tank is healthy
    {
        float updatedVal = 0;
        foreach (Placeable placeable in assignedPlaceables) //change the inventory bit to be like a held items list in here
        {
            int val;
            updatedVal += placeable.purchasable.price * Manager.Instance.allPurchasables[placeable.purchasable];
            //add in a modifier for like tank quality too later
        }
        if (assignedSubstrate)
        {
            updatedVal += assignedSubstrate.price;
        }
        value = updatedVal;
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
        if(onStatUpdate != null)
        {
            onStatUpdate.Invoke();
        }
    }
    public void RemoveModifiers(Item item)
    {
        tankPh -= item.pHMod;
        tankTemp -= item.tempMod;
        tankHardness -= item.dGHMod;
        tankLight -= item.lightMod;
        if(item.tags != null)
        {
            foreach (Tag tag in item.tags)
            {
                if (tags.ContainsKey(tag))
                {
                    tags[tag]--;
                }
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
            assignedSubstrate = nullSubstrate;
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
        if(tankDirtiness < dirtSlider.maxValue)
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
        tankHappiness = 0;
        onStatUpdate.Invoke();
        yield return true;
    }
}
