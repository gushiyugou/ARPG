using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnapsackManager : SingletonMono<KnapsackManager>
{
    //TODO:最后添加模版用于保存数据
    [Header("Knapsack Data")]
    public KnapsackData_SO knapsackData;


    [Header("ContainerS")]
    public ContainerUI knapsackUI;

    private void Start()
    {
        knapsackUI.RefreshUI();
    }
}
