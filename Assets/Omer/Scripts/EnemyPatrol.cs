using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // Hedef noktalar (sahneden atanacak)
    public float moveSpeed = 3f;     // Hareket hýzý
    public float arrivalThreshold = 0.2f; // Noktaya varma mesafesi

    private int currentPointIndex = 0;

    void Update()
    {
        if (patrolPoints.Length == 0) return;

        // Þu anki hedefe doðru yönel
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;

        // Hareket ettir
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Hedefe çok yaklaþtýysa sonraki noktaya geç
        float distance = Vector3.Distance(transform.position, targetPoint.position);
        if (distance < arrivalThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }

        // Düþmanýn yönünü hedefe çevir
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }
    }
}
