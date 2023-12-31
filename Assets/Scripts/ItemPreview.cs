using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPreview : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();
    public MeshFilter mesh;
    public MeshRenderer meshRenderer;
    float meshViewDefaultDistance;

    private void Start()
    {
        meshViewDefaultDistance = mesh.transform.position.z;
    }

    public void RotLeft()
    {
        mesh.gameObject.transform.Rotate(0, -45, 0);
    }
    public void RotRight()
    {
        mesh.gameObject.transform.Rotate(0, 45, 0);
    }
    public void Set(Purchasable purchasable)
    {
        if(purchasable != null)
        {
            mesh.mesh = purchasable.model;
            meshRenderer.material = purchasable.material;
            Vector3 meshSize = mesh.mesh.bounds.size;
            meshSize.Scale(mesh.transform.localScale);
            mesh.transform.position = new Vector3(mesh.transform.position.x, mesh.transform.position.y, meshViewDefaultDistance + meshSize.magnitude);
        }
        else
        {
            mesh.mesh = null;
        }
    }
}
