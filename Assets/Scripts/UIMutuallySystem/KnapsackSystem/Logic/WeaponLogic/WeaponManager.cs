using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : SingletonMono<WeaponManager>
{
    Transform weaponSlot;



    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipmentWeapon();
        EquipmentWeapon(weapon);
    }

    public void EquipmentWeapon(ItemData_SO weapon)
    {
        Instantiate(weapon.weaponPrefab, weaponSlot);

        //DOTO:更新数据

    }

    public void UnEquipmentWeapon()
    {
        if(weaponSlot.transform.childCount != 0)
        {
            for(int i = 0;i < weaponSlot.transform.childCount;i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
    }
}
