using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Useable,
    Weapon,
    Armor,
}
[CreateAssetMenu(fileName = "New Item",menuName = "KnapsackSystem/Item Data")]
public class ItemData_SO : ScriptableObject 
{
    public ItemType itemType;
    public string itemName;
    public int itmeCount;
    public Sprite itemIcon;
    public bool stackable;

    public GameObject itemPrefab;
    [TextArea]
    public string description = "";

    [Header("Useble Item")]
    public UseableItemData_SO useableItemData;

    [Header("weapon")]
    //TODO:武器数据
    public WeaponItemData_SO weaponItemData;

    //防御数据
    [Header("armor")]
    public ArmorItmData_SO armorItemData;

    

}
