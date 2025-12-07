using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpPanelManager : SingletonMono<HpPanelManager>
{
    [Header("fill Image")]
    public Image playerHpFillImage;
    public Image bossHpFillImage;

    [Header("state Info Data")]
    public CharacterBaseInfo_SO playerStateInfo;
    public CharacterBaseInfo_SO bossStateInfo;




    public void UpdateFillImage()
    {
        playerHpFillImage.fillAmount = playerStateInfo.currentHealth / playerStateInfo.maxHealth;
        bossHpFillImage.fillAmount = bossStateInfo.currentHealth / bossStateInfo.maxHealth;
    }
}
