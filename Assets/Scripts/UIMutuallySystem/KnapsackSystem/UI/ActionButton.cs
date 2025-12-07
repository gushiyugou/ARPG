using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode useKey;

    private SlotHolder currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(useKey) && currentSlotHolder.itemUI.GetItem() != null) 
        {
            currentSlotHolder.UseItme();
        }
    }
}
