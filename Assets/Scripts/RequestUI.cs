using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestBehaviour : MonoBehaviour
{
    public Request currentRequest;
    public Image portrait;
    public TextMeshProUGUI name;
    public TextMeshProUGUI request;
    public Button submit;
    public Button cancel;
    public GameObject visuals;

    public GameObject reviewScreen;

    public void Set(Request request)
    {
        submit.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
        currentRequest = request;
        portrait.sprite = request.portrait;
        name.text = request.characterName;
        submit.onClick.AddListener(Manager.Instance.SubmitTankRequest);
        cancel.onClick.AddListener(Manager.Instance.CancelTankRequest);
    }
}
