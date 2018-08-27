using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]

public class CustomTerrain : MonoBehaviour {

    // Random Height ---------
    public Vector2 randomHeightRange = new Vector2(0, 0.1f);

    //Texture Terrain --------
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    //Perlin Noise -----------
    public float perlinScaleX = 0.01f;
    public float perlinScaleY = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;
    public float perlinFrequency = 2f;

    //Multiple Noise ------------
    [System.Serializable]
    public class PerlinParameters
    {
        public float mPerlinScaleX = 0.01f;
        public float mPerlinScaleY = 0.01f;
        public int mPerlinOffsetX = 0;
        public int mPerlinOffsetY = 0;
        public int mPerlinOctaves = 3;
        public float mPerlinPersistance = 8;
        public float mPerlinHeightScale = 0.09f;
        public float mPerlinFrequency = 2f;
        public bool remove = false;
    }

    //Voronoi ----------
    public float voronoiFallOff = 1;
    public float voronoiDropOff = 1;
    public int voronoiPeakCount = 5;
    public float voronoiMinHeight = 0.1f;
    public float voronoiMaxHeight = 0.2f;
    public enum VoronoiType { Linear = 0, Power = 1, Combined = 2, WeirdShape = 3 }
    public VoronoiType voronoiType = VoronoiType.Linear;

    public List<PerlinParameters> perlinParamList = new List<PerlinParameters>()
    {
        new PerlinParameters()
    };

    //ResetTerrain ------------
    public bool resetTerrain = true;

    //Terrain Data -----------
    public Terrain terrain;
    public TerrainData terrainData;

    public float[,] GetHeightMap ()
    {
        if (resetTerrain)
        {
            return new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        }
        else
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        }
    }

    public void RandomTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                heightMap[x, y] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void TextureTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                heightMap[x, y] += heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(y * heightMapScale.y)).grayscale * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    } 

    public void PerlinNoiseTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                heightMap[x, y] += Utils.fBM((x + perlinOffsetX) * perlinScaleX, (y + perlinOffsetY) * perlinScaleY, perlinOctaves, perlinPersistance, perlinFrequency) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MultiplePerlinNoiseTerrain()
{
    float[,] heightMap = GetHeightMap();
    for (int y = 0; y < terrainData.heightmapHeight; y++)
    {
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            foreach (PerlinParameters p in perlinParamList)
            {
                heightMap[x, y] += Utils.fBM((x + perlinOffsetX) * perlinScaleX, (y + perlinOffsetY) * perlinScaleY, perlinOctaves, perlinPersistance, perlinFrequency) * perlinHeightScale;

            }
        }
    }
    terrainData.SetHeights(0, 0, heightMap);
}

    public void AddNewPerlin()
{
    perlinParamList.Add(new PerlinParameters());
}

    public void RemovePerlin()
    {
        List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
        for(int i = 0; i < perlinParamList.Count; i++)
        {
            if (!perlinParamList[i].remove)
            {
                keptPerlinParameters.Add(perlinParamList[i]);
            }
        }
        if (keptPerlinParameters.Count == 0)
        {
            keptPerlinParameters.Add(perlinParamList[0]);
        }
        perlinParamList = keptPerlinParameters;
    }

    public void VoronoiTerrain()
    {
        float[,] heightMap = GetHeightMap();

        for (int i = 0; i < voronoiPeakCount; i++)
        {
            Vector2 peakLocation = new Vector2(UnityEngine.Random.Range(0, terrainData.heightmapWidth), UnityEngine.Random.Range(0, terrainData.heightmapHeight));
            float peakHeight = UnityEngine.Random.Range(voronoiMinHeight, voronoiMaxHeight);
            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapWidth, terrainData.heightmapHeight));


            if (heightMap[(int)peakLocation.x, (int)peakLocation.y] < peakHeight)
            {
                heightMap[(int)peakLocation.x, (int)peakLocation.y] = peakHeight;
            }
            else
            {
                continue;
            }
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < terrainData.heightmapWidth; x++)
                {
                    if (!(x == peakLocation.x && y == peakLocation.y))
                    {
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                        float height;

                        if(voronoiType == VoronoiType.Combined)
                        {
                            height = peakHeight - distanceToPeak * voronoiFallOff - Mathf.Pow(distanceToPeak, voronoiDropOff);
                        }
                        else if (voronoiType == VoronoiType.Linear)
                        {
                            height = peakHeight - distanceToPeak * voronoiFallOff;
                        }
                        else if (voronoiType == VoronoiType.Power)
                        {
                            height = peakHeight - Mathf.Pow(distanceToPeak, voronoiDropOff);
                        } 
                        else
                        {
                            height = peakHeight - Mathf.Pow(distanceToPeak * 3, voronoiFallOff) - Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / voronoiDropOff;
                        }
                        if (heightMap[x, y] < height)
                        {
                            heightMap[x, y] = height;
                        }
                    }
                }
            }
        }


        terrainData.SetHeights(0, 0, heightMap);
        }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        terrainData.SetHeights(0, 0, heightMap);
    }

    void OnEnable()
    {
        terrain = this.GetComponent<Terrain>();
        terrainData =Terrain.activeTerrain.terrainData;
    }

    void Awake()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain");
        AddTag(tagsProp, "Cloud");
        AddTag(tagsProp, "Shore");

        tagManager.ApplyModifiedProperties();

        this.gameObject.tag = "Terrain";
    }

    void AddTag(SerializedProperty tagsProp, string newTag)
    {
        bool found = false;

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }
    }
}
