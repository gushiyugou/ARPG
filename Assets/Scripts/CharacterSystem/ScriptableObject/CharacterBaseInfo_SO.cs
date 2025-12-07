using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new Character Base Data",menuName ="Character/Character Data")]
public class CharacterBaseInfo_SO : ScriptableObject
{
    [Header("Health")]
    public float currentHealth;
    public float maxHealth;

    [Header("Attack Info")]
    public float attack;
    public float defense;
}
