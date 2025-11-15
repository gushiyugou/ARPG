

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill2 : SkillObjectBase
{
    public AudioSource audioSource;
    public override void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        base.Init(enemyTagList, onHitAction);
        StartSkillHit();
        PlayAudio();
    }

    private void PlayAudio()
    {
        audioSource.Play();
    }

}
