using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RequestSelector : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI requestUI;
    public Button select;
    public TextMeshProUGUI timeLabel;

    Request centerItem;
    public List<Request> requests; //all available requests, discounting any already chosen ones
    int currentRequestIndex; //the index of the current request

    public delegate void RequestSelected(Request request);
    public RequestSelected OnRequestSelected;

    private void Start()
    {
        Set(requests[0]);
        select.onClick.AddListener(Select);
    }

    void Set(Request request)
    {
        centerItem = request;
        portrait.sprite = request.portrait;
        charName.text = request.characterName;
        requestUI.text = request.GenerateRequest(request.requestText);
        
        timeLabel.text = request.lengthInTicks + " ticks long"; //defo wanna change to realtime so I can use mins, hours. secs
    }

    public void Select()
    {
        OnRequestSelected.Invoke(centerItem);
        Manager.Instance.currentTank.assignedRequest = centerItem;
        gameObject.SetActive(false);
    }

    public void MoveList(int dir)
    {
        int target = currentRequestIndex + dir;
        if(target > requests.Count-1)
        {
            currentRequestIndex = 0;
        }
        else if(target < 0)
        {
            currentRequestIndex = requests.Count - 1;
        }
        else
        {
            currentRequestIndex = target;
        }
        Set(requests[currentRequestIndex]);
    }

    
}
