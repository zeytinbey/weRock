using UnityEngine;

public class ToplanabilirObje : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            // Manager'a haber ver
            CollectibleManager.Instance.CollectItem();

            // Objeyi yok et
            Destroy(other.gameObject);
        }
    }
    }
