using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlacingMenu : MonoBehaviour
{
    //handles putting back and selling the selected placeable
    public GameObject visuals;
    public Button close;
    public Button putBack;
    public Button sell;
    public Placeable currentPlaceable;
    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    public Button moveLeft;
    public Button moveRight;
    public Button moveUp;
    public Button moveDown;
    public Slider red;
    public Slider green;
    public Slider blue;

    private bool sliderEnabled;

    Vector3 baseOffset = new Vector3(0, 1.3f, 0f);

    private void Start()
    {
        sell.onClick.AddListener(Unset);
        putBack.onClick.AddListener(Unset);
        close.onClick.AddListener(Unset);
    }

    public void Set(Placeable placeable)
    {
        red.onValueChanged.RemoveAllListeners();
        green.onValueChanged.RemoveAllListeners();
        blue.onValueChanged.RemoveAllListeners();
        moveLeft.onClick.RemoveAllListeners();
        moveRight.onClick.RemoveAllListeners();
        moveUp.onClick.RemoveAllListeners();
        moveDown.onClick.RemoveAllListeners();

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

        red.value = currentPlaceable.color.r;
        green.value = currentPlaceable.color.g;
        blue.value = currentPlaceable.color.b;

        red.onValueChanged.AddListener(delegate { currentPlaceable.SetColor(new Color(red.value, green.value, blue.value)); });
        green.onValueChanged.AddListener(delegate { currentPlaceable.SetColor(new Color(red.value, green.value, blue.value)); });
        blue.onValueChanged.AddListener(delegate { currentPlaceable.SetColor(new Color(red.value, green.value, blue.value)); });

        moveLeft.onClick.AddListener(delegate { currentPlaceable.SetInput(Vector2.left); });
        moveRight.onClick.AddListener(delegate { currentPlaceable.SetInput(Vector2.right); });
        moveUp.onClick.AddListener(delegate { currentPlaceable.SetInput(Vector2.up); });
        moveDown.onClick.AddListener(delegate { currentPlaceable.SetInput(Vector2.down); });

        visuals.SetActive(true);
        onSelect.Invoke();
    }

    public void Unset()
    {
        //transform.SetParent(null);
        if(currentPlaceable)
        {
            currentPlaceable.selected = false;
        }
        visuals.SetActive(false);
        onDeselect.Invoke();
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
