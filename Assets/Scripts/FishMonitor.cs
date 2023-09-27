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
            fishComment.text = "Not sure how to implement this yet!";
        }
    }

    void GetComment()
    {
        //check which area is most impacting the fish, have that be the comment
        if(currentFish.hunger > currentFish.happiness)
        {

        }
    }

    /// <summary>
    /// OK SO
    /// FISH HAPPINESS IS ALREADY CONTEROLLED BY FACOURITE FOODS VS REGULAR FOODS
    /// I WANT HAPPINESS TO BE INFLUENCED BY LIKES AND DISLIKES FISH TOO
    /// AND HOW COMPATIBLE THE FISH IS IN THE TANK
    /// HAPPINESS CAN BE JUST THE READOUT FOR THE HARMONY AND IT CAN BE AN AVERAGE OF ALL THE FISH HAPPINESS
    /// THEY CAN CALCULATE
    /// 
    /// </summary>


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

    //fish comments logic here!
}
