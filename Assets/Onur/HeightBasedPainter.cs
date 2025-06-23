using UnityEngine;

public class TerrainHeightPainter : MonoBehaviour
{
    Terrain terrain;
    TerrainData terrainData;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        PaintByHeight();
    }

    void PaintByHeight()
    {
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;

        float[,,] splatmapData = new float[width, height, 3]; // 3 layer

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normX = (float)x / (width - 1);
                float normY = (float)y / (height - 1);

                float worldHeight = terrainData.GetInterpolatedHeight(normX, normY) / terrainData.size.y;

                float[] splat = new float[3];

                if (worldHeight < 0.3f)
                    splat[0] = 1f;  // layer 0: çimen
                else if (worldHeight < 0.6f)
                    splat[1] = 1f;  // layer 1: kaya
                else
                    splat[2] = 1f;  // layer 2: kar

                for (int i = 0; i < 3; i++)
                {
                    splatmapData[x, y, i] = splat[i];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("Terrain yüksekliðe göre boyandý.");
    }
}
