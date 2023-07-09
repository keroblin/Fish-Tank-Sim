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

        if(happiness > 0.55)
        {
            Debug.Log(fishName + " is happy ");
            return Manager.Instance.happy;
        }
        else if(happiness > 0.3)
        {
            Debug.Log(fishName + " is ok");
            return Manager.Instance.ok;
        }
        else
        {
            Debug.Log(fishName + " is sad");
            return Manager.Instance.sad;
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

        Debug.Log("ph compat on " + fishName + " is " + pHCompat.ToString());
        Debug.Log("temp compat on " + fishName + " is " + tempCompat.ToString());
        Debug.Log("light compat on " + fishName + " is " + lightCompat.ToString());
        Debug.Log("hardness compat on " + fishName + " is " + hardnessCompat.ToString());

        return (pHCompat + hardnessCompat + tempCompat + lightCompat) / 4;
    }

    float GetCompat(float min, float max, float stat)
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
}
