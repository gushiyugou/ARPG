using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillHitEffectConfig")]
public class SkillHitEffectConfig:ScriptableObject
{
    public SkillSpawnObj skillSpawnObj;
    public AudioClip hitAudioClip;
}
