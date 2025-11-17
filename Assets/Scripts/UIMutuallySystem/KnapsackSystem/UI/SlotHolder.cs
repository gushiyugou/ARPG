using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
                itemUI.Bag = KnapsackManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData == null)
                    gameObject.GetComponent<Image>().enabled = true;
                else
                    gameObject.GetComponent<Image>().enabled = false;
                //ÇÐ»»ÎäÆ÷
                //if (itemUI.Bag.items[itemUI.Index].itemData != null)
                //    WeaponManager.Instance.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                //else
                //    WeaponManager.Instance.UnEquipmentWeapon();
                break;
            case SlotType.Armor:
                itemUI.Bag = KnapsackManager.Instance.equipmentData;
                break;
            case SlotType.Action:
                itemUI.Bag = KnapsackManager.Instance.actionData;
                break;
        }

        KnapsackItem item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.itemCount);
    }
    
}
