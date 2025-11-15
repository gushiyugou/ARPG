using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //TODO:物体的拾取,添加到背包

            other.gameObject.GetComponent<KeyInteraction>().CanPickUpGoods(this.gameObject);

            //

            //Destroy(gameObject);
        }
    }
}
