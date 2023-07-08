using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlacingMenu : MonoBehaviour
{
    public Button move;
    public Button rotate;
    public Button putBack;
    public Button sell;

    Vector3 baseOffset = new Vector3(0, 2, -2);
    public void Set(Placeable placeable)
    {
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
        //connect up all events to the placeable
        move.onClick.AddListener(placeable.StartMove);
        rotate.onClick.AddListener(placeable.StartRotate);
        putBack.onClick.AddListener(placeable.PutBack);
        sell.onClick.AddListener(placeable.Sell);
    }
    public void Unset()
    {
        move.onClick.RemoveAllListeners();
        rotate.onClick.RemoveAllListeners();
        putBack.onClick.RemoveAllListeners();
        sell.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
        transform.SetParent(null);
    }
}
