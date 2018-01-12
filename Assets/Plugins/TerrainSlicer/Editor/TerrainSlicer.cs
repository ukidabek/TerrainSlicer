using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class TerrainSlicer : EditorWindow
{
    private int _fromGUIHorizontalSlices = 2;
    private int _fromGUIVerticalSlices = 2;

    private int _horizontalSlices = 2;
    private int _verticalSlices = 2;

    private int horizontalSize = 0;
    private int verticallSize = 0;

    private int _heightMapPreviewSize = 513;

    private Terrain _terrain = null;
    private TerrainData _terrainData
    {
        get
        {
            if (_terrain == null)
                return null;

            return _terrain.terrainData;
        }
    }

    private Texture2D _heightMap = null;
    private Texture2D[,] _slicedHeightMap = null;

    [MenuItem("Window/TerrainSlicer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TerrainSlicer window = (TerrainSlicer)GetWindow(typeof(TerrainSlicer));
        window.Show();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeObject == null)
        {
            _terrain = null;
            return;
        }

        _terrain = (Selection.activeObject as GameObject).GetComponent<Terrain>();
        GenerateHeightmapTexture();
        Repaint();
    }

    private void GenerateHeightmapTexture()
    {
        int heightmapWidth = _terrainData.heightmapWidth;
        int heightmapHeight = _terrainData.heightmapHeight;

        int _height = ((_terrainData.heightmapWidth - 1) / _horizontalSlices) + 1;

        float[,] hights = _terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
        _heightMap = new Texture2D(heightmapWidth, heightmapHeight);

        for (int x = 0; x < heightmapWidth; x++)
        {
            for (int y = 0; y < heightmapHeight; y++)
            {
                float height = hights[x, y];
                Color color = new Color(height, height, height);
                _heightMap.SetPixel(x, y, color);
            }
        }

        _heightMap.Apply();
    }

    public void Slice()
    {
        _horizontalSlices = _fromGUIHorizontalSlices;
        _verticalSlices = _fromGUIHorizontalSlices;

        _slicedHeightMap = new Texture2D[_horizontalSlices, _verticalSlices];

        int heightmapWidth = _terrainData.heightmapWidth;
        int heightmapHeight = _terrainData.heightmapHeight;

        horizontalSize = heightmapWidth / _horizontalSlices;
        verticallSize = heightmapHeight / _verticalSlices;


        for (int i = 0; i < _horizontalSlices; i++)
        {
            for (int j = 0; j < _verticalSlices; j++)
            {
                _slicedHeightMap[i,j] = new Texture2D(horizontalSize, verticallSize);
            }
        }

        int sliceHorizontal = 0;
        int sliceVertical = 0;

        for (int i = _horizontalSlices - 1; i >= 0; i--)
        {
            for (int j = 0; j < _verticalSlices; j++)
            {
                Color[] color = _heightMap.GetPixels(sliceHorizontal, sliceVertical, horizontalSize, verticallSize);
                _slicedHeightMap[i, j].SetPixels(0, 0, horizontalSize, verticallSize, color);

                sliceHorizontal += horizontalSize;
            }
            sliceVertical += verticallSize;
            sliceHorizontal = 0;
        }

        for (int i = 0; i < _horizontalSlices; i++)
        {
            for (int j = 0; j < _verticalSlices; j++)
            {
                _slicedHeightMap[i, j].Apply();
            }
        }
    }

    private void OnGUI()
    {
        if(_terrain == null)
        {
            EditorGUILayout.LabelField("Select terrain!");
            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.FloatField("Width", _terrainData.size.x);
                EditorGUILayout.FloatField("Length", _terrainData.size.z);
                EditorGUILayout.FloatField("Height", _terrainData.size.y);

                _fromGUIHorizontalSlices = EditorGUILayout.IntField("Horizontal Slices", _fromGUIHorizontalSlices);
                _fromGUIVerticalSlices = EditorGUILayout.IntField("Vertical Slices", _fromGUIVerticalSlices);
                if(GUILayout.Button("Slice"))
                {
                    Slice();
                }

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label(_heightMap, GUILayout.Width(_heightMapPreviewSize));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            {
                if(_slicedHeightMap != null)
                {
                    GUIStyle style = new GUIStyle();
                    style.padding = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.fixedWidth = _heightMapPreviewSize / _horizontalSlices;
                    style.fixedHeight = _heightMapPreviewSize / _verticalSlices;

                    EditorGUILayout.BeginVertical();
                    {
                        for (int i = 0; i < _horizontalSlices; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                for (int j = 0; j < _verticalSlices; j++)
                                {
                                    if(GUILayout.Button(_slicedHeightMap[i, j], style))
                                    {
                                        Debug.Log(i + " " + j);
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }
}
