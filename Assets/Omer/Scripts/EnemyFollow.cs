using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform target;         // Player referans�
    public float followSpeed = 3f;   // Takip h�z�
    public float followRange = 10f;  // Takip menzili

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= followRange)
        {
            // Hedefe y�nel ve ilerle
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;

            // Y�z�n� hedefe d�nd�r
            Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 290f, 0f);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
