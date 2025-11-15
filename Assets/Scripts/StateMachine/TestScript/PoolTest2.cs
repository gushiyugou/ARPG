using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolTest2 : MonoBehaviour
{
    private List<GameObject> activeObjects = new List<GameObject>();
    public string folderName;
    public string prefabName;

    private string path;
    private void Awake()
    {
        path = $"{folderName}/{prefabName}";
    }
    //private void OnEnable()
    //{
    //    Invoke("RemoveMe", 1);
    //}

    //public void RemoveMe()
    //{

    //}
    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            PoolManager.Instance.GetPoolObject(prefabName, folderName);
        }
    }
}