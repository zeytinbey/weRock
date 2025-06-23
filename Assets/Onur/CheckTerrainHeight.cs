using UnityEngine;

public class CheckTerrainHeight : MonoBehaviour
{
    public Terrain terrain;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain atanmamýþ!");
            return;
        }

        TerrainData data = terrain.terrainData;
        Debug.Log($"Terrain Size Y: {data.size.y}");
        Debug.Log($"Heightmap Resolution: {data.heightmapResolution}");

        // 5 farklý noktada yükseklik oku
        for (int i = 0; i <= 4; i++)
        {
            float normX = i / 4f;
            float normY = i / 4f;
            float height = data.GetInterpolatedHeight(normX, normY);
            Debug.Log($"Height at ({normX}, {normY}) = {height}");
        }
    }
}
