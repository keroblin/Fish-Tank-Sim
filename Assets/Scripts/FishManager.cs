using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public List<FishBehaviour> liveFish;

    //temp, may change
    IEnumerator HungerTick()
    {
        while (liveFish.Count > 0)
        {
            foreach(FishBehaviour fish in liveFish)
            {
                if(fish.happiness > 2)
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
    }
    /*IEnumerator HappinessTick()
    {
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
    }*/
}
