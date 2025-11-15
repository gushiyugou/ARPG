using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class KeyInteraction:MonoBehaviour
{
    [SerializeField]private Image key_E;
    public void CanPickUpGoods(GameObject obj)
    {
        
        key_E.gameObject.SetActive(true);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Destroy(obj);
            key_E.gameObject.SetActive(false);
        }
    }
}
