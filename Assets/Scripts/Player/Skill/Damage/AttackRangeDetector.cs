using UnityEngine;

public class AttackRangeDetector : MonoBehaviour
{
    [Header("攻击范围设置")]
    public float attackRadius = 2f;        // 攻击半径
    public float attackAngle = 90f;         // 攻击角度（扇形）
    public float attackDistance = 3f;       // 攻击距离
    public LayerMask enemyLayer;            // 敌人层级
    public LayerMask obstacleLayer;         // 障碍物层级

    /// <summary>
    /// 检测前方扇形区域内的敌人
    /// </summary>
    public Collider[] DetectEnemiesInFront()
    {
        // 获取角色位置和前方方向
        Vector3 center = transform.position + transform.forward * (attackDistance / 2);
        Vector3 forward = transform.forward;

        // 使用球形检测获取所有可能的碰撞体
        Collider[] hitColliders = Physics.OverlapSphere(center, attackRadius, enemyLayer);

        // 过滤出在扇形区域内的敌人
        System.Collections.Generic.List<Collider> enemiesInSector = new System.Collections.Generic.List<Collider>();

        foreach (Collider col in hitColliders)
        {
            Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(forward, directionToTarget);

            // 检查是否在扇形角度内
            if (angleToTarget < attackAngle / 2)
            {
                // 检查是否有障碍物阻挡
                if (!IsObstructed(transform.position, col.transform.position))
                {
                    enemiesInSector.Add(col);
                }
            }
        }

        return enemiesInSector.ToArray();
    }

    /// <summary>
    /// 检查两点之间是否有障碍物
    /// </summary>
    private bool IsObstructed(Vector3 from, Vector3 to)
    {
        RaycastHit hit;
        Vector3 direction = to - from;
        float distance = direction.magnitude;

        if (Physics.Raycast(from, direction, out hit, distance, obstacleLayer))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 在Scene视图中绘制攻击范围（调试用）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 绘制攻击距离
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * attackDistance);

        // 绘制扇形区域
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        DrawSectorGizmo(transform.position, transform.forward, attackAngle, attackDistance);
    }

    /// <summary>
    /// 绘制扇形Gizmo
    /// </summary>
    private void DrawSectorGizmo(Vector3 center, Vector3 forward, float angle, float radius)
    {
        int segments = 20;
        float deltaAngle = angle / segments;
        Quaternion leftRayRotation = Quaternion.Euler(0, -angle / 2, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, angle / 2, 0);

        Vector3 leftDir = leftRayRotation * forward;
        Vector3 rightDir = rightRayRotation * forward;

        // 绘制扇形边界
        Gizmos.DrawRay(center, leftDir * radius);
        Gizmos.DrawRay(center, rightDir * radius);

        // 绘制扇形弧线
        Vector3 prevPoint = center + leftDir * radius;
        for (int i = 1; i <= segments; i++)
        {
            float segmentAngle = -angle / 2 + deltaAngle * i;
            Quaternion segmentRotation = Quaternion.Euler(0, segmentAngle, 0);
            Vector3 segmentDir = segmentRotation * forward;
            Vector3 currentPoint = center + segmentDir * radius;

            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }

    
}