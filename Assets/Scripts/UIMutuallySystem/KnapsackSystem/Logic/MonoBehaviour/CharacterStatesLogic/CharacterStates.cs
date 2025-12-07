using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class CharacterStates : SingletonMono<CharacterStates>
{
    Transform weaponSlot;


    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipmentWeapon(weapon);
        EquipmentWeapon(weapon);
    }

    public void EquipmentWeapon(ItemData_SO weapon)
    {
        if(weapon.itemPrefab != null)
            Instantiate(weapon.itemPrefab, weaponSlot);

        //DOTO:更新数据
    }

    public void UnEquipmentWeapon(ItemData_SO weapon)
    {
        CharacterBaseInfo_SO characterBaseInfo = KnapsackManager.Instance.characterBaseInfo;
        float maxHealth = characterBaseInfo.maxHealth;
        //if (weaponSlot.transform.childCount != 0)
        //{
        //    for(int i = 0;i < weaponSlot.transform.childCount;i++)
        //    {
        //        Destroy(weaponSlot.transform.GetChild(i).gameObject);
        //    }
        //}
        switch (weapon.itemType)
        {
            case ItemType.Weapon:
                if (weapon.weaponItemData != null)
                {
                    if(characterBaseInfo.attack - weapon.weaponItemData.attackNum >0)
                        characterBaseInfo.attack -= weapon.weaponItemData.attackNum;
                    else
                        characterBaseInfo.attack = 0;
                    if (characterBaseInfo.defense - weapon.weaponItemData.defenseNum > 0)
                        characterBaseInfo.defense -= weapon.weaponItemData.defenseNum;
                    else
                        characterBaseInfo.defense = 0;
                }
                break;
            case ItemType.Armor:
                if (weapon.armorItemData != null)
                {
                    if (characterBaseInfo.attack - weapon.weaponItemData.attackNum > 0)
                        characterBaseInfo.attack -= weapon.weaponItemData.attackNum;
                    else
                        characterBaseInfo.attack = 0;
                    if (characterBaseInfo.defense - weapon.weaponItemData.defenseNum > 0)
                        characterBaseInfo.defense -= weapon.weaponItemData.defenseNum;
                    else
                        characterBaseInfo.defense = 0;
                }
                break;
        }
    }
    #endregion


    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        CharacterBaseInfo_SO characterBaseInfo = KnapsackManager.Instance.characterBaseInfo;
        if (characterBaseInfo.currentHealth +  amount > characterBaseInfo.maxHealth)
            characterBaseInfo.currentHealth = characterBaseInfo.maxHealth;
        else
            characterBaseInfo.currentHealth += amount;
    }

    #endregion


    public void UpdataCharacterInfo(ItemData_SO item)
    {
        CharacterBaseInfo_SO characterBaseInfo = KnapsackManager.Instance.characterBaseInfo;
        float maxHealth = characterBaseInfo.maxHealth;
        if(item != null)
        {
            switch (item.itemType)
            {
                case ItemType.Useable:
                    if (item.useableItemData != null)
                    {
                        if (characterBaseInfo.currentHealth + item.useableItemData.healthPoint > maxHealth)
                            characterBaseInfo.currentHealth = maxHealth;
                        else
                            characterBaseInfo.currentHealth += item.useableItemData.healthPoint;
                    }
                    break;
                case ItemType.Weapon:
                    if (item.weaponItemData != null)
                    {
                        characterBaseInfo.attack += item.weaponItemData.attackNum;
                        characterBaseInfo.defense += item.weaponItemData.defenseNum;
                    }
                    break;
                case ItemType.Armor:
                    if (item.armorItemData != null)
                    {
                        characterBaseInfo.attack += item.armorItemData.attackNum;
                        characterBaseInfo.defense += item.armorItemData.defenseNum;
                    }
                    break;
            }
        }
       

    }
}
