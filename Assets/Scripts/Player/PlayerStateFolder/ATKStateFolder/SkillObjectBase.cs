using System;
using System.Collections.Generic;
using UnityEngine;


public class SkillObjectBase : MonoBehaviour
{
    [SerializeField] private new Collider collider;
    private List<string> enemyTagList;
    private List<IHurt> enemyList = new List<IHurt>();
    private Action<IHurt, Vector3> onHitAction;

    //TODO WeaponCollider: 初始化函数，进行受伤时的通知
    public virtual void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        collider.enabled = false;
        this.enemyTagList = enemyTagList;
        this.onHitAction = onHitAction;
    }

    public virtual void StartSkillHit()
    {
        collider.enabled = true;
    }

    public virtual void StopSkillHit()
    {
        collider.enabled = false;
        enemyList.Clear();
    }

    /// <summary>
    /// 触发检测函数
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerStay(Collider other)
    {
        //检测打击对象的标签
        if (enemyTagList == null) return;
        if (enemyTagList.Contains(other.tag))
        {
            IHurt enemy = other.GetComponentInParent<IHurt>();
            if (enemy != null && !enemyList.Contains(enemy))
            {
                //TODO 武器击中敌人的通知
                Debug.Log("武器：攻击伤害触发");
                onHitAction?.Invoke(enemy, other.ClosestPoint(transform.position));
                enemyList.Add(enemy);
            }
        }
    }
}

