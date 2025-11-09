using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*技能的配置主要配置技能的所有属性*/
[CreateAssetMenu(menuName = "SkillConfig")]
public class SkillConfig : ScriptableObject 
{
    public string AnimationName;
    public SkillReleaseData releaseData;
    //public SkillAttackData[] attackData = new SkillAttackData[1];
    public SkillAttackData attackData;
}

/// <summary>
/// 技能释放配置
/// </summary>
[Serializable,]
public class SkillReleaseData
{
    //播放粒子
    [Header("技能释放时的特效")]
    public SkillSpawnObj effectObj;
    //技能释放音效
    [Header("技能释放时的音效")]
    public AudioClip skillAudio;
}

/// <summary>
/// 技能攻击配置
/// </summary>
[Serializable]
public class SkillAttackData
{
    //播放粒子
    public SkillSpawnObj[] skillObj;

    //技能释放音效
    //public AudioClip[] attackAudio = new AudioClip[2];
    public AudioClip[] attackAudio;

    //TODO:命中数据
    //伤害值
    public float damageValue;

    //技能伤害持续时间（持续技能伤害特有）
    //敌人僵直
    public float stiffTime;

    //击退击飞时间
    public float repelTime;

    //技能击退击飞
    public Vector3 repelDegree;

    //技能击中效果
    public SkillHitEffectConfig hitEffect;

    //屏幕震动
    public float impulseValue;

    //后处理,目前有色差效果
    public float chromaticValue;

    //卡肉效果
    public float FreezeFrameTime;

    //时间停止
    public float FreezeGameTime;

    //范围检测
    public attackCheck attackcheck;

}

[Serializable]
public class SkillSpawnObj
{
    //直接生成预制体生成的预制体
    //public GameObject prefab;
    //使用对象池生成预制体
    [Header("预制体路径名")]
    public SkillPrefab prefab;
    //生成的音效
    public AudioClip spawnAudio;
    //位置
    public Vector3 position;
    //旋转
    public Vector3 rotation;

    public Vector3 scale = Vector3.one;
    
    //延迟时间
    public float Time;
}

[Serializable]
public class SkillPrefab
{
    public string prefabName;
    public string folderName;
}
[Serializable]
public class attackCheck
{
    public Vector3 checkPos;
    public Vector3 halfExtents;
    public Vector3 checkRot;
    public float checkRange;
}
