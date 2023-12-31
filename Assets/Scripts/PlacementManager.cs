using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Playables;

public class PlacementManager:MonoBehaviour
{
    public GameObject placementParent;
    public List<Placeable> instances;
    public Pool placeablePool;

    public delegate void OnSelect(Placeable placeable);
    public delegate void OnPlace(Placeable placeable);
    public delegate void OnPutBack(Placeable placeable);
    public event OnSelect PlaceableSelected;
    public event OnPlace PlaceablePlaced;
    public event OnPutBack PlaceableReturned;

    public static PlacementManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Placeable Place(Purchasable purchasable, Pool pool = null)
    {
        Placeable placeable;
        if(purchasable.prefab != null)
        {
            placeable = Instantiate(purchasable.prefab).GetComponent<Placeable>();
        }
        else
        {
            if(pool == null)
            {
                pool = placeablePool;
            }
            GameObject obj = pool.Pull();
            Debug.Log("Placeable spawned as " +  obj.name);
            placeable = obj.GetComponent<Placeable>();
        }
        placeable.Set(purchasable);
        Debug.Log(placeable.GetType());
        instances.Add(placeable);
        placeable.gameObject.transform.SetParent(placementParent.transform, false);
        if(placeable != null && PlaceablePlaced != null)
        {
            PlaceablePlaced.Invoke(placeable);
        }
        Manager.Instance.currentTank.assignedPlaceables.Add(placeable);
        return placeable;
    }

    public void Select(Placeable placeable) //when the placeable menu selects an item
    {
        if(placeable != null && PlaceableSelected != null)
        {
            PlaceableSelected.Invoke(placeable);
        }
    }

    public void PutBackPlaceable(Placeable placeable)
    {
        instances.Remove(placeable);
        Manager.Instance.currentTank.assignedPlaceables.Remove(placeable);
        if (placeable.purchasable.prefab == null)
        {
            placeablePool.StartCoroutine("ReturnRigidbody", placeable.gameObject);
        }
        else
        {
            Destroy(placeable.gameObject);
        }
        if(placeable != null && PlaceableReturned != null)
        {
            PlaceableReturned.Invoke(placeable);
        }
    }

    public Placeable GetMostRecentPlaceable(Purchasable purchasable)
    {
        Placeable placeable;
        placeable = instances.FindLast(x=>x.purchasable == purchasable);
        return placeable;
    }

    public int GetAmountPlaced(Purchasable purchasable)
    {
        int count = instances.FindAll(x => x.purchasable == purchasable).Count;
        return count;
    }
}
