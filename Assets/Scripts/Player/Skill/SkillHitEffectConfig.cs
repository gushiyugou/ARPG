using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillHitEffectConfig")]
public class SkillHitEffectConfig:ScriptableObject
{
    public SkillSpawnObj skillSpawnObj;

    public AudioClip hitAudioClip;

    //技能失败时的配置
    public SkillSpawnObj failSkillSpawnObj;
}
