using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Fish")]
public class Fish: ScriptableObject
{
    public float minPH = 6.0f,maxPH = 8.5f;
    public float minHardness = 5f,maxHardness = 30f;
    public float minTemp = 33f,maxTemp = 90f;
    public float minLight = 0f,maxLight = 1f;

    public Sprite fishIcon;
    public Mesh fishMesh;
    public string fishName;

    public Sprite GetHappinessIcon()
    {
        float happiness = CalculateHappiness();
        Debug.Log(happiness + " is " + fishName + "'s happiness");
        if(happiness < 0.5)
        {
            return Manager.Instance.sad;
        }
        else if (happiness < 0.2)
        {
            return Manager.Instance.ok;
        }
        else
        {
            return Manager.Instance.happy;
        }
    }

    float CalculateHappiness()
    {
        //do some averaging against current stats
        //check if the stats are in range and how close to range they are
        //use abs to get distance without knowing which is bigger
        float pHCompat = GetCompat(minPH,maxPH,Manager.Instance.tankPh);
        float hardnessCompat = GetCompat(minHardness, maxHardness, Manager.Instance.tankHardness);
        float tempCompat = GetCompat(minTemp, maxTemp, Manager.Instance.tankTemp);
        float lightCompat = GetCompat(minLight, maxLight, Manager.Instance.tankLight);

        //work out if stat is within risk range
        //work out how far from average stat is
        //if outside of range then its 0

        //get average of all percentages as happiness

        return 10* (pHCompat + hardnessCompat + tempCompat + lightCompat) / 4;
    }

    float GetCompat(float min, float max, float stat)
    {
        if (stat < max && stat > min)
        {
            float center = (min + max) / 2;
            return Mathf.Abs(center - stat) / (min+center);
        }
        else //out of range
        {
            return 0.0f;
        }
    }
}
