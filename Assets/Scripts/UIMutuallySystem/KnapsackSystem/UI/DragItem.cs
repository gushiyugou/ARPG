using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemUI currentItemUI;
    private SlotHolder currentSlotHolder;
    private SlotHolder targetSlotHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentSlotHolder = GetComponentInParent<SlotHolder>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        KnapsackManager.Instance.currentDarg = new KnapsackManager.DargData();
        KnapsackManager.Instance.currentDarg.originalHolder = currentSlotHolder;
        KnapsackManager.Instance.currentDarg.originalParent = (RectTransform)transform.parent;
        transform.SetParent(KnapsackManager.Instance.dargCanvas.transform,true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (KnapsackManager.Instance.CheckActionUI(eventData.position) ||
           KnapsackManager.Instance.CheckKnapsackUI(eventData.position) ||
           KnapsackManager.Instance.CheckEquipmentUI(eventData.position))
        {
            if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                targetSlotHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
            else
                targetSlotHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();

            switch (targetSlotHolder.slotType)
            {
                case SlotType.Bag:
                    SwapItem();
                    break;
                case SlotType.Weapon:
                    if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                        SwapItem();
                    break;
                case SlotType.Armor:
                    if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Arrow)
                        SwapItem();
                    break;
                case SlotType.Action:
                    if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                        SwapItem();
                    break;
            }

            currentSlotHolder.UpdateItem();
            targetSlotHolder.UpdateItem();
        }
        transform.SetParent(KnapsackManager.Instance.currentDarg.originalParent);
        RectTransform tempRect = transform as RectTransform;
        tempRect.offsetMax = -Vector2.one * 5;
        tempRect.offsetMin = Vector2.one * 5;
    }

    public void SwapItem()
    {
        var targetItem = targetSlotHolder.itemUI.Bag.items[targetSlotHolder.itemUI.Index];
        var darggingItem = currentSlotHolder.itemUI.Bag.items[currentSlotHolder.itemUI.Index];

        bool isSameItem = targetItem.itemData == darggingItem.itemData;

        if(isSameItem && targetItem.itemData.stackable)
        {
            targetItem.itemCount += darggingItem.itemCount;
            darggingItem.itemData = null;
            darggingItem.itemCount = 0;
        }
        else
        {
            currentSlotHolder.itemUI.Bag.items[currentSlotHolder.itemUI.Index] = targetItem;
            targetSlotHolder.itemUI.Bag.items[targetSlotHolder.itemUI.Index] = darggingItem;  

        }
    }
}
