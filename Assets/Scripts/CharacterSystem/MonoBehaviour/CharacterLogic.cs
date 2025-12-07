using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterLogic : SingletonMono<KnapsackManager>
{
    [Header("character State Data")]
    public CharacterBaseInfo_SO characterData;
    [Header("TextMeshPro")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI armorText;
    
    private void Update()
    {
       
    }
}
