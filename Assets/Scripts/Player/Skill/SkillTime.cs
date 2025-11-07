using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTime : MonoBehaviour
{
    public float skillDurationTime = 2;

    private void Updata()
    {
        gameObject.SetActive(true);
        Destroy(gameObject, skillDurationTime);
    }
}
