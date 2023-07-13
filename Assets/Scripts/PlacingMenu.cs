using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacingMenu : MonoBehaviour
{
    //handles putting back and selling the selected placeable
    public GameObject visuals;
    public Button move;
    public Button close;
    public Button putBack;
    public Button sell;
    public Placeable currentPlaceable;

    Vector3 baseOffset = new Vector3(0, 1.3f, 0f);

    private void Start()
    {
        sell.onClick.AddListener(Unset);
        putBack.onClick.AddListener(Unset);
        close.onClick.AddListener(Unset);
    }

    public void Set(Placeable placeable)
    {
        if (currentPlaceable)
        {
            currentPlaceable.selected = false;
        }
        currentPlaceable = placeable;
        placeable.selected = true;
        Vector3 offset;
        if (placeable.menuOffset == Vector3.zero)
        {
            offset = baseOffset;
        }
        else
        {
            offset = placeable.menuOffset;
        }
        //move to the placeable
        transform.SetParent(placeable.gameObject.transform, false);
        transform.localPosition = offset;
        visuals.SetActive(true);
    }
    public void Unset()
    {
        currentPlaceable.selected = false;
        transform.SetParent(null);
        visuals.SetActive(false);
    }

    public void Select(Placeable placeable)
    {
        if(currentPlaceable != placeable || (placeable.selected == false))
        {
            Set(placeable);
        }
        else
        {
            Unset();
        }
    }
}
