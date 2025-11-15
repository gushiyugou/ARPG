using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTime : MonoBehaviour
{
    public float skillDurationTime = 2;
    public string folderName;
    public string prefabName;

    private void OnEnable()
    {
        Invoke("RemoveSkill", skillDurationTime);
    }

    private void RemoveSkill()
    {
        if (prefabName != null && folderName != null)
            PoolManager.Instance.PutPoolObject(prefabName, this.gameObject);
    }
}
