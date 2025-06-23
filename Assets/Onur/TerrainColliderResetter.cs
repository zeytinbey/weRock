using UnityEngine;

public class TerrainColliderResetter : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TerrainCollider>().enabled = false;

        GetComponent<TerrainCollider>().enabled = true;
    }
}
