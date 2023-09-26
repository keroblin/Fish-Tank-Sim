using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FishManager : MonoBehaviour
{
    public static FishManager instance;
    public FishMonitor monitor;
    public GameObject fishParent;
    public List<FishBehaviour> liveFish;

    private void Start()
    {
        instance = this;
        Manager.Instance.currentTank.onTankTick.AddListener(FishTick);
    }

    public void AddFish(Fish fish)
    {
        FishBehaviour placeable = PlacementManager.Instance.Place(fish) as FishBehaviour;
        placeable.fish = fish;
        placeable.gameObject.transform.SetParent(fishParent.transform, false);
        placeable.placeableClicked.AddListener(delegate { monitor.Set(placeable); });
        placeable.Set(fish);
        Manager.Instance.currentTank.assignedFish.Add(placeable);
    }
    public void RemoveFish(Fish fish)
    {
        FishBehaviour behaviour = liveFish.Find(x => x.fish == fish);
        liveFish.Remove(behaviour);
        Manager.Instance.currentTank.assignedFish.Remove(behaviour);
    }

    public void FishTick()
    {
        foreach (FishBehaviour fishBehaviour in liveFish) //potentially use the tank's fish list we shall see
        {
            //if the fish is really hungry, take down their happiness
            //if the fish is really unhappy, take down their hunger faster
            //if the fish dislikes other fish, take down their happiness a lot

            //sort out hygeine here based on number of fish

            fishBehaviour.ageInTicks++;

            if(fishBehaviour.hunger > 0)
            {
                if (fishBehaviour.happiness > 2)
                {
                    fishBehaviour.hunger -= .5f;
                }
                else
                {
                    fishBehaviour.hunger -= 1f;
                }
            }
            
            if(fishBehaviour.hunger < 2 && fishBehaviour.happiness > 0)
            {
                fishBehaviour.happiness -= .5f;
            }

            //check fishs dislikes
            //remove happiness
        }
    }

    public float GetFishHarmony()
    {
        float harmony = 5f;
        foreach(FishBehaviour fish in liveFish)
        {
            //calculate balance of liked and disliked fish
        }
        return harmony;
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
