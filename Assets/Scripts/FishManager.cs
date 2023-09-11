using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static FishManager instance;
    public GameObject fishParent;
    public List<FishBehaviour> liveFish;

    private void Start()
    {
        instance = this;
    }

    public void AddFish(Fish fish)
    {
        FishBehaviour placeable = PlacementManager.Instance.Place(fish) as FishBehaviour;
        placeable.fish = fish;
        placeable.gameObject.transform.SetParent(fishParent.transform, false);
        placeable.Set(fish);
    }
    public void RemoveFish(Fish fish)
    {
        FishBehaviour behaviour = liveFish.Find(x => x.fish == fish);
        liveFish.Remove(behaviour);
    }
}
