using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static FishManager instance;
    public FishMonitor monitor;
    public GameObject fishParent;
    public List<FishBehaviour> liveFish;
    public delegate void FishAdded(Fish fish);
    public FishAdded onFishAdded;
    public delegate void FishRemoved(Fish fish);
    public FishRemoved onFishRemoved;
    public float overallHappiness;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Manager.Instance.currentTank.onTankTick.AddListener(FishTick);
    }

    public void AddFish(Fish fish)
    {
        FishBehaviour placeable = PlacementManager.Instance.Place(fish) as FishBehaviour;
        placeable.fish = fish;
        placeable.gameObject.transform.SetParent(fishParent.transform, false);
        placeable.placeableClicked.AddListener(delegate { monitor.Set(placeable); });
        placeable.Set(fish);
        
        foreach(FishBehaviour dweller in liveFish)
        {
            //check and set their harmony against the new fish
            if (dweller.fish.dislikedFish.Contains(fish))
            {
                dweller.harmony--;
                Debug.Log(dweller.name + " didn't like " + fish.name);
            }
            else if(dweller.fish.lovedFish.Contains(fish))
            {
                dweller.harmony++;
                Debug.Log(dweller.name + " liked " + fish.name);
            }
        }
        liveFish.Add(placeable);
    }
    public void RemoveFish(Fish fish)
    {
        FishBehaviour behaviour = liveFish.Find(x => x.fish == fish);
        liveFish.Remove(behaviour);
        foreach (FishBehaviour dweller in liveFish)
        {
            //undo modifiers basically
            if (dweller.fish.dislikedFish.Contains(fish))
            {
                dweller.harmony++;
            }
            else if (dweller.fish.lovedFish.Contains(fish))
            {
                dweller.harmony--;
            }
        }
    }

    public void FishTick()
    {
        overallHappiness = 0;
        foreach (FishBehaviour fishBehaviour in liveFish) //potentially use the tank's fish list we shall see
        {
            fishBehaviour.ageInTicks++;
            overallHappiness += fishBehaviour.GetHappiness();
        }
        overallHappiness /= liveFish.Count;
    }

    public int GetFishPoo()
    {
        int pooAmount = 0;
        foreach(FishBehaviour fish in liveFish)
        {
            if(fish.ageInTicks % 50 == 0) //every nth tick
            {
                pooAmount++;
                //do modifiers if its hungry or ill or whatever here
            }
        }
        return pooAmount;
    }
}
