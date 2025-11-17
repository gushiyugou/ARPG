using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Useable,
    Weapon,
    Arrow,
}
[CreateAssetMenu(fileName = "New Item",menuName = "KnapsackSystem/Item Data")]
public class ItemData_SO : ScriptableObject 
{
    public ItemType itemType;
    public string itemName;
    public int itmeCount;
    public Sprite itemIcon;
    public bool stackable;

    [TextArea]
    public string description = "";

    [Header("weapon")]
    public GameObject weaponPrefab;
    //TODO:ÎäÆ÷Êý¾Ý


}
