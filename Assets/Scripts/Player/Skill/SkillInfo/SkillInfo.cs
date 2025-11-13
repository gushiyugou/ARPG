
using UnityEngine.UI;
using System;
using UnityEngine;

[Serializable]
public class SkillInfo
{
    public KeyCode skillKey;
    public SkillConfig skillConfig;
    public float cdTime;
    [NonSerialized]public float currentTime;
    public Image skillMaskImg;
}
