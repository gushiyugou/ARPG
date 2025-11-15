using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTest1 : MonoBehaviour
{
    public string folderName;
    public string prefabName;
    
    private void OnEnable()
    {
        Invoke("RemoveMe", 1);
    }

    private void RemoveMe()
    {
        //GameObjectPoolManager.Instance.ReplaceGameObject("Cube", this.gameObject);
        PoolManager.Instance.PutPoolObject(prefabName, this.gameObject);
    }
}
