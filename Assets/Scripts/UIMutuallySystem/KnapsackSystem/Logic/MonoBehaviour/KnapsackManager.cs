using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnapsackManager : SingletonMono<KnapsackManager>
{
    public class DargData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }
    //TODO:最后添加模版用于保存数据
    [Header("Knapsack Data")]
    public KnapsackData_SO knapsackData;

    public KnapsackData_SO actionData;

    public KnapsackData_SO equipmentData;


    [Header("ContainerS")]
    public ContainerUI knapsackUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Darg Canvas")]
    public Canvas dargCanvas;
    public DargData currentDarg;

    private void Start()
    {
        knapsackUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    public bool CheckKnapsackUI(Vector3 position)
    {
        for (int i = 0; i < knapsackUI.slotHoders.Length; i++)
        {
            RectTransform tempRect = knapsackUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(tempRect, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHoders.Length; i++)
        {
            RectTransform tempRect = actionUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(tempRect, position))
            {
                return true;
            }
        }
        return false;
    }


    public bool CheckEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHoders.Length; i++)
        {
            RectTransform tempRect = equipmentUI.slotHoders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(tempRect, position))
            {
                return true;
            }
        }
        return false;
    }
}
