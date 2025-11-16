using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SlotType
{
    Bag,
    Weapon,
    Armor,
    Action
}
public class SlotHolder : MonoBehaviour
{
    public SlotType slotType;
    public ItemUI itemUI;


    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.Bag:
                itemUI.Bag = KnapsackManager.Instance.knapsackData;
                break;
            case SlotType.Weapon:
                break;
            case SlotType.Armor:
                break;
            case SlotType.Action:
                break;
        }


        KnapsackItem item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.itemCount);
    }
    
}
