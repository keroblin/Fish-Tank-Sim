using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public List<GameObject> pool;

    public GameObject objToPool;
    public int poolLength;
    [ContextMenu("Create Pool")]
    void FillPool()
    {
        for (int i = transform.childCount; i < poolLength; i++)
        {
            GameObject instance = Instantiate(objToPool);
            instance.transform.parent = this.transform;
            instance.name = objToPool.name;
            instance.SetActive(false);
            Debug.Log("Made child " + (i + 1));
        }
    }
    void OnEnable()
    {
        foreach (Transform child in transform)
        {
            pool.Add(child.gameObject);
        }
    }

    public GameObject Pull()
    {
        if (pool.Count > 0)
        {
            GameObject objToReturn = pool[0];
            objToReturn.SetActive(true);
            Debug.Log("Pool amount left: " + pool.Count);
            pool.RemoveAt(0);
            return objToReturn;
        }
        else
        {
            Debug.Log("Pool empty!");
            return null;
        }
    }

    public void Return(GameObject objToReturn)
    {
        objToReturn.SetActive(false);
        objToReturn.transform.parent = this.transform;
        //reset its components basically here, but most of this should be done in the objects themselves that pull
        objToReturn.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
