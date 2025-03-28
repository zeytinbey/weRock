using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalWithCollision : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
