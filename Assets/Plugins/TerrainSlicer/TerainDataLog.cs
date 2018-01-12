using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerainDataLog : MonoBehaviour
{
    [SerializeField]
    Terrain _terrain = null;

    [SerializeField]
    Texture2D[] _map = null;

    [SerializeField]
    Texture2D _hightMap = null;

    [SerializeField]
    Texture2D[] _newHigntMaps = null;

    [SerializeField]
    float[,] _hights = null;

    private void Awake()
    {
        TerrainData data = _terrain.terrainData;
        _map = data.alphamapTextures;

        int xx = (((int)data.heightmapWidth - 1)/2)+1;
        int yy = (((int)data.heightmapHeight - 1)/2)+1;

        _newHigntMaps = new Texture2D[4];

        for (int i = 0; i < 4; i++)
        {
            _newHigntMaps[i] = new Texture2D(xx, yy);
        }

        _hightMap = new Texture2D((int)data.heightmapWidth, (int)data.heightmapHeight);
        _hights = data.GetHeights(0,0, (int)data.heightmapWidth, (int)data.heightmapHeight);
        for (int x = 0; x < data.heightmapWidth; x++)
        {
            for (int y = 0; y < data.heightmapHeight; y++)
            {
                float height = _hights[x, y];
                Color color = new Color(height, height, height);
                _hightMap.SetPixel(x, y, color);
            }
        }
        _hightMap.Apply();

        int xxx = 0;
        int yyy = 0;
        for (int x = 0; x < (int)data.heightmapWidth; x++)
        {
            float u = (float)x / (int)data.heightmapWidth;
            for (int y = 0; y < (int)data.heightmapHeight; y++)
            {
                float v = (float)y / (int)data.heightmapHeight;

                Color color = _hightMap.GetPixelBilinear(u, v);
                if(x <= xx && y <= xx)
                {
                    _newHigntMaps[0].SetPixel(x, y, color);
                }
            }
        }
        _newHigntMaps[0].Apply();


        Debug.Log(data.size.ToString());
        Debug.Log(_hights[90,24].ToString());

        //int x = 1;
        //for (int i = 0; i < 10; i++)
        //{
        //    x *= 2;
        //    Debug.Log(x);
        //}
    }
}
