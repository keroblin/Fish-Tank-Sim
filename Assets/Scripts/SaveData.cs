using System;
using System.Collections;
using System.Collections.Generic;
using SerializeTools;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class SaveData
{
    //public List<TankData> tanks;
    public TankData tankData = null;
    public float money = Manager.Instance.currentMoney;
    public SerializeableDictionary<string, int> inventory = new(); //purchasables
    //public int lastAccessedTank;

    private static SaveData _instance;
    
    public static SaveData Instance
    {
        get 
        { 
            if(_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
    }
    [System.Serializable]
    public class TankData
    {
        public (string,float) assignedRequest = new();
        public List<PlaceableData> assignedPlaceables = new();
        public List<FishData> fishDatas = new();
        public List<ItemData> itemDatas = new();
        public float tankDirtiness = Manager.baseDirt;
        public float tankHappiness = 0;
        public float tankPh = Manager.basePh;
        [Range(0f, 1f)]
        public float tankLight = Manager.baseLight;
        public float tankTemp = Manager.baseTemp;
        public float tankHardness = Manager.baseHardness;
        public float value = 0.0f;
        public string assignedSubstrate;
        public float age;
        public TankData()
        {
            this.assignedRequest = ("",-1f);
            this.assignedPlaceables = new List<PlaceableData>();
            this.itemDatas = new();
            this.fishDatas = new();
            this.age = 0f;
            this.tankDirtiness = Manager.baseDirt;
            this.tankHappiness = 0;
            this.tankPh = Manager.basePh;
            this.tankLight = Manager.baseLight;
            this.tankTemp = Manager.baseTemp;
            this.tankHardness = Manager.baseHardness;
            this.value = 0.0f;
            this.assignedSubstrate = "Null";
        }
        public TankData(Tank tank)
        {
            if (!tank.assignedRequest)
            {
                this.assignedRequest = (tank.assignedRequest.name, tank.assignedRequestBehaviour.timeLeft.value);
            }
            assignedPlaceables = new List<PlaceableData>();
            foreach (Placeable placeable in tank.assignedPlaceables)
            {
                if(placeable is FishBehaviour)
                {
                    this.assignedPlaceables.Add(new FishData(placeable));
                    this.fishDatas.Add(new FishData(placeable)); //weird workaround for subclass serialisation issues
                    Debug.Log("Saved " + placeable.name + " as fish");
                }
                else if(placeable is ItemBehaviour)
                {
                    this.assignedPlaceables.Add(new ItemData(placeable));
                    this.itemDatas.Add(new ItemData(placeable));
                    Debug.Log("Saved " + placeable.name + " as item");
                }
            }
            this.age = tank.age;
            this.tankDirtiness = tank.tankDirtiness;
            this.tankHappiness = tank.tankHappiness;
            this.tankPh = tank.tankPh;
            this.tankLight = tank.tankLight;
            this.tankTemp = tank.tankTemp;
            this.tankHardness = tank.tankHardness;
            this.value = tank.value;
            if (tank.assignedSubstrate != null)
            {
                this.assignedSubstrate = tank.assignedSubstrate.name;
            }
            else { this.assignedSubstrate = null;}
        }
    }
    [System.Serializable]
    public class PlaceableData
    {
        public string purchasableName;
        public SerializeableTransform transform;
        public PlaceableData()
        {
            this.purchasableName = "";
            this.transform = null;
        }
        public PlaceableData(Placeable placeable)
        {
            this.purchasableName = placeable.purchasable.name;
            this.transform = new SerializeableTransform(placeable.gameObject.transform);
        }
    }
    [System.Serializable]
    public class FishData:PlaceableData
    {
        public float age;
        public float happiness;
        public float hunger;
        public float harmony;
        public SerializeableVec3 target;

        public FishData()
        {
            this.age = 0f;
            this.happiness = .5f;
            this.hunger = .5f;
            this.harmony = .5f;
            this.target = null;
        }
        public FishData(Placeable placeable):base(placeable)
        {
            FishBehaviour fish = placeable as FishBehaviour;
            this.age = fish.ageInTicks;
            this.happiness = fish.happiness;
            this.hunger = fish.hunger;
            this.harmony = fish.harmony;
            this.target = new SerializeableVec3(fish.currentTargetPosition);
        }
    }
    [System.Serializable]
    public class ItemData:PlaceableData
    {
        public Color color;
        public ItemData()
        {
            this.color = Color.white;
        }
        public ItemData(Placeable placeable):base(placeable)
        {
            ItemBehaviour item = placeable as ItemBehaviour;
            this.color = item.color;
        }
    }
}
