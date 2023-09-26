using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestSelector : MonoBehaviour
{
    List<Request> requests; //all available requests, discounting any already chosen ones
    Button left; //moves list to left
    Button right; //moves list to right
    Button select; //closes ui and starts the request up in a request behaviour, assigns it to the tank too?
    Pool requestChoicePool; //pool of prefabbed versions of this script
    int currentRequestIndex; //the index of the current request
    void Start()
    {
       
    }

    public void MoveList(int dir)
    {

    }

    
}
