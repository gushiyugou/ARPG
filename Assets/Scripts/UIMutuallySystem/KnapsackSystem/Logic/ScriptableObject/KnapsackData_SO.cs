using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New KnapsackData",menuName = "KnapsackSystem/Knapsack Data")]
public class KnapsackData_SO : ScriptableObject
{
    public List<KnapsackItem> items = new List<KnapsackItem>();

    public void AddItem(ItemData_SO newItemData,int itemAmount)
    {
        bool found = false;
        if (newItemData.stackable)
        {
            foreach (KnapsackItem item in items)
            {
                if(item.itemData == newItemData)
                {
                    item.itemCount += itemAmount;
                    found = true;
                    break;
                }
            }
        }

        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].itemData == null && !found)
            {
                items[i].itemData = newItemData;
                items[i].itemCount = itemAmount;
                break;
            }
        }
    }
}


[Serializable]
public class KnapsackItem
{
    public ItemData_SO itemData;
    public int itemCount;
}
