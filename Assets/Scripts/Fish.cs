using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Fish")]
public class Fish: Purchasable
{
    public float speed = 6f;
    public float maxSpeed = 10f;
    public Manager.FishPersonalities personality;

    public List<Fish> lovedFish;
    public List<Fish> dislikedFish;
    //remove the favourite stuff from the foods, do em on a per fish basis instead
    public List<Food> favouriteFoods;
    public List<Food> hatedFoods;

    public float minPH = 6.0f,maxPH = 8.5f;
    public float minHardness = 5f,maxHardness = 30f;
    public float minTemp = 33f,maxTemp = 90f;
    public float minLight = 0f,maxLight = 1f;

    public Sprite GetCompatIcon(float statOverride = 0.0f)
    {
        float compat;
        if (statOverride == 0.0f)
        {
            compat = CalculateCompat();
        }
        else
        {
            compat = statOverride;
        }

        if(compat > 0.55)
        {
            return Manager.Instance.happy;
        }
        else if(compat > 0.3)
        {
            return Manager.Instance.ok;
        }
        else
        {
            return Manager.Instance.sad;
        }
    }

    public float CalculateCompat()
    {
        //do some averaging against current stats
        //check if the stats are in range and how close to range they are
        //use abs to get distance without knowing which is bigger
        float pHCompat = GetCompat(minPH, maxPH, Manager.Instance.currentTank.tankPh);
        float hardnessCompat = GetCompat(minHardness, maxHardness, Manager.Instance.currentTank.tankHardness);
        float tempCompat = GetCompat(minTemp, maxTemp, Manager.Instance.currentTank.tankTemp);
        float lightCompat = GetCompat(minLight, maxLight, Manager.Instance.currentTank.tankLight);

        return (pHCompat + hardnessCompat + tempCompat + lightCompat) / 4;
    }

    public float GetCompat(float min, float max, float stat)
    {
        if (stat > max || stat < min) //out of range
        {
            return 0.0f;
        }
        else
        {
            if (min != max) //if within range
            {
                return 1f - Mathf.Abs(0.5f - (stat - min) / (max - min)) * 2;
            }
            else //if there's no range and the fish is very picky
            {
                return 1.0f;
            }

        }
    }

    public override void Buy()
    {
        base.Buy();
    }

    public override void Sell()
    {
        base.Sell();
    }

    public override void Place()
    {
        //spawn the fish in the tank
        FishManager.instance.AddFish(this);
    }
    public override void Remove()
    {
        base.Remove();
        FishManager.instance.RemoveFish(this);
    }

}
