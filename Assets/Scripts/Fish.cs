using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Fish")]
public class Fish: ScriptableObject
{
    public float minPH = 6.0f,maxPH = 8.5f;
    public float minHardness,maxHardness;
    public float minTemp,maxTemp;
    public float minLight,maxLight;
    [Range(0, 1)]
    public float happiness;

    public Mesh fishMesh;
    public string fishName;

    float CalculateHappiness()
    {
        //do some averaging against current stats
        return happiness;
    }
}
