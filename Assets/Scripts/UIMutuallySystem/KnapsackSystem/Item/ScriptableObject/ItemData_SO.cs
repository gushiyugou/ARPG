using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItmeType
{
    Useable,
    Weapon,
    Arrow,
}
[CreateAssetMenu(fileName = "New Item",menuName = "KnapsackSystem/Item Data")]
public class ItemData_SO : ScriptableObject 
{
    public ItmeType itmeType;
    public string itemName;
    public int itmeCount;
    public Sprite itemIcon;
    public bool stackable;

    [TextArea]
    public string description = "";

    [Header("weapon")]
    public GameObject weaponPrefab;

}
