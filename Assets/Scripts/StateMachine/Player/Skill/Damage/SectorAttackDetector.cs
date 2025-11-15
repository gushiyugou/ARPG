using UnityEngine;
using System.Collections.Generic;

public class SectorAttackDetector : MonoBehaviour
{
    [Header("扇形攻击范围设置")]
    [SerializeField] private float attackRadius = 3f;
    [SerializeField] private float attackAngle = 90f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    //[SerializeField] private bool showGizmos = true;

    public float AttackRadius { get => attackRadius; set => attackRadius = value; }
    public float AttackAngle { get => attackAngle; set => attackAngle = Mathf.Clamp(value, 0, 360); }
    public LayerMask EnemyLayer { get => enemyLayer; set => enemyLayer = value; }
    public LayerMask ObstacleLayer { get => obstacleLayer; set => obstacleLayer = value; }

    /// <summary>
    /// 检测前方扇形区域内的所有敌人
    /// </summary>
    public List<GameObject> DetectEnemiesInSector()
    {
        List<GameObject> detectedEnemies = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        foreach (Collider collider in hitColliders)
        {
            if (IsTargetInSector(collider.transform.position) && !IsObstructed(collider.transform))
            {
                detectedEnemies.Add(collider.gameObject);
            }
        }

        return detectedEnemies;
    }

    /// <summary>
    /// 检测单个敌人是否在扇形区域内
    /// </summary>
    public bool IsTargetInSector(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle <= attackAngle * 0.5f;
    }

    /// <summary>
    /// 检查目标是否被障碍物阻挡
    /// </summary>
    private bool IsObstructed(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance, obstacleLayer))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 在Scene视图中绘制扇形区域
    /// </summary>
    //private void OnDrawGizmos()
    //{
    //    if (!showGizmos) return;

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, attackRadius);

    //    Gizmos.color = new Color(1, 0, 0, 0.3f);
    //    DrawSectorGizmo();

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(transform.position, transform.forward * 2f);
    //}

    private void DrawSectorGizmo()
    {
        int segments = 20;
        float halfAngle = attackAngle * 0.5f;

        Quaternion leftRotation = Quaternion.Euler(0, -halfAngle, 0);
        Quaternion rightRotation = Quaternion.Euler(0, halfAngle, 0);

        Vector3 leftDir = leftRotation * transform.forward;
        Vector3 rightDir = rightRotation * transform.forward;

        Gizmos.DrawRay(transform.position, leftDir * attackRadius);
        Gizmos.DrawRay(transform.position, rightDir * attackRadius);

        Vector3 previousPoint = transform.position + leftDir * attackRadius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + (attackAngle / segments) * i;
            Quaternion segmentRotation = Quaternion.Euler(0, angle, 0);
            Vector3 segmentDir = segmentRotation * transform.forward;
            Vector3 currentPoint = transform.position + segmentDir * attackRadius;

            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}