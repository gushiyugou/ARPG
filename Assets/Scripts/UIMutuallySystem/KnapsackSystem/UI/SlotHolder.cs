using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum SlotType
{
    Bag,
    Weapon,
    Armor,
    Action
}

public class SlotHolder : MonoBehaviour,IPointerClickHandler
{
    public SlotType slotType;
    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount % 2 == 0)
        {
            UseItme();
        }
        
    }

    public void UseItme()
    {
        if (itemUI.GetItem() == null) return;

        if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].itemCount > 0)
        {
            //CharacterStates.Instance.ApplyHealth(itemUI.GetItem().useableItemData.healthPoint);
            CharacterStates.Instance.UpdataCharacterInfo(itemUI.GetItem());
            itemUI.Bag.items[itemUI.Index].itemCount -= 1;
        }
        HpPanelManager.Instance.UpdateFillImage();
        UpdateItem();
    }

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
                {
                    gameObject.GetComponent<Image>().enabled = true;
                    
                } 
                else
                {
                    CharacterStates.Instance.EquipmentWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                    gameObject.GetComponent<Image>().enabled = false;
                }
                CharacterStates.Instance.UpdataCharacterInfo(itemUI.Bag.items[itemUI.Index].itemData);
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
