using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolTest : MonoBehaviour
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

    //public void Update()
    //{
    //    if (Input.GetMouseButtonDown(1)) // 获取对象
    //    {
    //        GameObject obj = PoolManager.Instance.GetPoolObject("Cube", "Prefabs");
    //        if (obj != null)
    //        {
    //            activeObjects.Add(obj);
    //            // 设置随机位置，避免重叠
    //            obj.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space) && activeObjects.Count > 0) // 归还对象
    //    {
    //        GameObject objToReturn = activeObjects[0];
    //        activeObjects.RemoveAt(0);
    //        PoolManager.Instance.ReturnPoolObject("Cube", objToReturn);
    //    }

    //    if (Input.GetKeyDown(KeyCode.R)) // 显示状态信息
    //    {
    //        Debug.Log($"活跃对象数: {activeObjects.Count}");
    //        foreach (var pool in PoolManager.Instance.PoolDic)
    //        {
    //            Debug.Log($"对象池 '{pool.Key}': {pool.Value.Count} 个对象");
    //        }
    //    }
    //}
}