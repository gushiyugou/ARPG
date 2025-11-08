using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Pool
{
    public GameObject poolObj;
    public List<GameObject> poolList;
    public Pool(GameObject parent, GameObject poolObj)
    {
        this.poolObj = poolObj;
        this.poolObj.transform.parent = parent.transform;
        poolList = new List<GameObject>();
    }

    public GameObject GetObject()
    {
        GameObject obj = poolList[0];
        poolList.Remove(obj);
        obj.SetActive(true);
        return obj;
    }

    public void ReplaceObject(GameObject poolIndividual)
    {
        poolList.Add(poolIndividual);
        poolIndividual.SetActive(false);
        poolIndividual.transform.parent = poolObj.transform;
    }
}
public class GameObjectPoolManager : SingletonMono<GameObjectPoolManager>
{
    public Dictionary<string, Pool> poolDic = new Dictionary<string, Pool>();
    private GameObject PoolParent;

    protected override void Awake()
    {
        base.Awake();
        PoolParent = new GameObject("Pool");
    }

    public GameObject GetPoolObject(string poolName, string folderName = null)
    {
        GameObject Obj = null;

        if (poolDic.ContainsKey(poolName))
        {
            if(poolDic[poolName].poolList.Count > 0)
            {
                Obj = poolDic[poolName].GetObject();
            }
            
        }
        else
        {
            GameObject poolGameObject = new GameObject(poolName+"Pool");
            Pool poolData = new Pool(PoolParent, poolGameObject);
            poolDic.Add(poolName, poolData);
            for (int i = 0; i < 10; i++)
            {
                GameObject poolIndividual = GameObject.Instantiate(Resources.Load<GameObject>(folderName + poolName));
                poolIndividual.name = poolName;
                poolData.ReplaceObject(poolIndividual);
            }
            Obj = poolDic[poolName].GetObject();
            //设置对象池的父对象
            //动态加载预制体
        }
        return Obj;
    }

    public void ReplaceGameObject(string poolName,GameObject poolIndividual)
    {
        if (!poolDic.ContainsKey(poolName))
        {
            GameObject newPool = new GameObject(poolName + "Pool");
            Pool poolData = new Pool(PoolParent, poolIndividual);
            poolDic.Add(poolName, poolData);
        }
        poolDic[poolName].ReplaceObject(poolIndividual);
    }
}
