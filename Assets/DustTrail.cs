using UnityEngine;

public class DustTrail : MonoBehaviour
{
    public GameObject dustPrefab;
    public Rigidbody rb;
    public float movementThreshold = 0.3f;
    public float emitCooldown = 0.1f;

    private float emitTimer = 0f;

    void Update()
    {
        emitTimer += Time.deltaTime;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        bool isMoving = horizontalVelocity.magnitude > movementThreshold;

        if (isMoving && emitTimer >= emitCooldown)
        {
            // Instantiate efekt
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            Instantiate(dustPrefab, spawnPos, Quaternion.identity);
            emitTimer = 0f;
        }
    }
}
