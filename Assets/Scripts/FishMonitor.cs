using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishMonitor : MonoBehaviour
{
    public GameObject visuals;
    public static FishMonitor instance;
    public FishStatView statView;
    public Camera fishCam;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishComment;
    public Slider happiness;
    public Slider hunger;
    public FishBehaviour currentFish;

    bool set;
    void Start()
    {
        instance = this;
        Manager.Instance.currentTank.onTankTick.AddListener(UpdateUI);
    }

    public void Set(FishBehaviour fishB)
    {
        visuals.SetActive(true);
        currentFish = fishB;
        fishName.text = fishB.fish.name;
        UpdateUI();
        fishCam.transform.SetParent(fishB.gameObject.transform, false);
        fishCam.transform.localPosition = Vector3.zero;
        fishCam.transform.localPosition = new Vector3(-(fishB.meshFilter.mesh.bounds.size.x * fishB.meshFilter.gameObject.transform.localScale.x) * 4, 0f, 0f);
        set = true;
    }

    public void UpdateUI()
    {
        if (currentFish)
        {
            happiness.value = currentFish.happiness; //maybe do this in an event or something so its always up to date
            hunger.value = currentFish.hunger;
            statView.Set(currentFish.fish);
            fishComment.text = GetComment();
        }
    }

    string GetComment()
    {
        //check which area is most impacting the fish, have that be the comment
        float compat = currentFish.fish.CalculateCompat()*10;
        List<(float,string)> fishStats = new List<(float, string)>();
        fishStats.Add((compat, "compat"));
        fishStats.Add((currentFish.harmony, "harmony"));
        fishStats.Add((1-currentFish.hunger, "fullness")); //take away from 1 to invert it and make it fullness
        (float,string) worstStat = (-100,"null");
        foreach((float,string) stat in fishStats)
        {
            if(worstStat.Item2 == "null")
            {
                worstStat = stat;
                continue;
            }
            if(stat.Item1 < worstStat.Item1)
            {
                worstStat = stat;
            }
        }
        string comment = "";
        Debug.Log("Worst was " + worstStat.ToString());
        if (worstStat.Item2 == "harmony") //on its own to cover if there are no bad fish actually
        {
            List<Fish> badFish = new List<Fish>();
            foreach (Fish dislikedFish in currentFish.fish.dislikedFish)
            {
                if (FishManager.instance.liveFish.Find(x => x.fish == dislikedFish))
                {
                    badFish.Add(dislikedFish);
                }
            }

            if (badFish.Count > 1)
            {
                comment = badFish.Count.ToString() + " fish in here are not my vibe, " + badFish[Random.Range(0, badFish.Count - 1)].name + " is a lil.. you know..";
                return comment;
            }
            else if (badFish.Count > 0)
            {
                comment = badFish[0] + " is harshing my vibe!";

                return comment;
            }
        }

        if (worstStat.Item2 == "fullness")
        {
            if (currentFish.fish.favouriteFoods.Count > 0)
            {
                comment = "I am absolutely STARVING for some " + currentFish.fish.favouriteFoods[Random.Range(0, currentFish.fish.favouriteFoods.Count - 1)];
                return comment;
            }
            else
            {
                comment = "I am absolutely STARVING!";
                return comment;
            }
        }

        if (worstStat.Item2 == "compat")
        {
            comment = "Something about the items in here is making me queasy";
        }

        return comment;
    }

    public void Unset()
    {
        visuals.SetActive(false);
        set = false;
        frameWaiter = 0;
    }

    int frameWaiter = 0; //just waits a frame, but allows us to check input in update and not have weirdness, again I'm sure theres a better way to do this but it works for now!
    private void Update() //i am sure there's a better way to do this in like a universal input handler but this will do for now
    {
        if (visuals.activeSelf && set)
        {
            frameWaiter++;
            if(Input.GetMouseButtonDown(0) && frameWaiter > 1)
            {
                Unset();
            }
        }
    }
}
