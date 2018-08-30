using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]

public class CustomTerrain : MonoBehaviour
{

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
    public List<PerlinParameters> perlinParamList = new List<PerlinParameters>() { new PerlinParameters() };

    //Voronoi ----------
    public float voronoiFallOff = 1;
    public float voronoiDropOff = 1;
    public int voronoiPeakCount = 5;
    public float voronoiMinHeight = 0.1f;
    public float voronoiMaxHeight = 0.2f;
    public enum VoronoiType { Linear = 0, Power = 1, Combined = 2, WeirdShape = 3 }
    public VoronoiType voronoiType = VoronoiType.Linear;

    //Mid Point Displacement ----
    public float mpdRoughness = 2.0f;
    public float mpdHeightMin = -10.0f;
    public float mpdHeightMax = 10.0f;
    public float mpdHeightDampener = 2.0f;

    //Smooth 
    public int smoothIterations = 3;

    //Splat Maps
    [System.Serializable]
    public class SplatHeights
    {
        public Texture2D texture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0f;
        public float maxSlope = 90f;
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 tileSize = new Vector2(50, 50);
        public bool remove = false;
    }
    public List<SplatHeights> splatHeights = new List<SplatHeights>() { new SplatHeights() };
    public float splatOffset = 0.01f;
    public float splatNoiseScale = 0.01f;
    public float splatNoiseX = 0.1f;
    public float splatNoiseY = 0.1f;

    //Water
    public float waterHeight = 0.2f;
    public GameObject waterObject;
    public Material shoreLineMaterial;

    //Erosion
    public enum ErosionType {Rain = 0, Thermal = 1, Tidal = 2, River = 3, Wind = 4}
    public ErosionType erosionType = ErosionType.Rain;
    public float erosionSolubility = 0.01f;
    public float erosionStrength = 0.01f;
    public int erosionDroplets = 1000;
    public int erosionSpringsPerDroplet = 2;



    //ResetTerrain ------------
    public bool resetTerrain = true;

    //Terrain Data -----------
    public Terrain terrain;
    public TerrainData terrainData;


    //Terrain Generation
    public float[,] GetHeightMap()
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
        for (int i = 0; i < perlinParamList.Count; i++)
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

                        if (voronoiType == VoronoiType.Combined)
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

    public void MPDTerrain()
    {
        float[,] heightMap = GetHeightMap();
        int width = terrainData.heightmapWidth - 1;
        int squaresize = width;
        float heightMin = mpdHeightMin;
        float heightMax = mpdHeightMax;
        float roughness = mpdRoughness;
        float heightDampener = Mathf.Pow(mpdHeightDampener, -roughness);

        int cornerX, cornerY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, pmidYD;

        while (squaresize > 0)
        {
            for (int x = 0; x < width; x += squaresize)
            {
                for (int y = 0; y < width; y += squaresize)
                {
                    cornerX = (x + squaresize);
                    cornerY = (y + squaresize);

                    midX = (int)(x + squaresize / 2.0);
                    midY = (int)(y + squaresize / 2.0);

                    heightMap[midX, midY] = ((heightMap[x, y] + heightMap[cornerX, y] + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4 + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }
            for (int x = 0; x < width; x += squaresize)
            {
                for (int y = 0; y < width; y += squaresize)
                {
                    cornerX = (x + squaresize);
                    cornerY = (y + squaresize);

                    midX = (int)(x + squaresize / 2.0);
                    midY = (int)(y + squaresize / 2.0);

                    pmidXR = (midX + squaresize);
                    pmidYU = (midY + squaresize);
                    pmidXL = (midX - squaresize);
                    pmidYD = (midY - squaresize);

                    if (pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1)
                    {
                        continue;
                    }

                    heightMap[midX, y] = ((heightMap[x, y] + heightMap[midX, midY] + heightMap[cornerX, y] + heightMap[midX, pmidYD]) / 4 + UnityEngine.Random.Range(heightMin, heightMax));
                    heightMap[x, midY] = ((heightMap[pmidXL, midY] + heightMap[x, cornerY] + heightMap[midX, midY] + heightMap[x, y]) / 4 + UnityEngine.Random.Range(heightMin, heightMax));
                    heightMap[midX, cornerY] = ((heightMap[x, cornerY] + heightMap[midX, pmidYU] + heightMap[cornerX, cornerY] + heightMap[midX, midY]) / 4 + UnityEngine.Random.Range(heightMin, heightMax));
                    heightMap[cornerX, midY] = ((heightMap[midX, midY] + heightMap[cornerX, cornerY] + heightMap[pmidXR, midY] + heightMap[cornerX, y]) / 4 + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }
            squaresize = (int)(squaresize / 2.0);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void SmoothTerrain()
    {
        float[,] heightMap = GetHeightMap();
        float averageHeight;
        float smoothProgress = 0;
        EditorUtility.DisplayProgressBar("Smoothing Terrain", "Iteration: " + smoothProgress + " of " + smoothIterations, smoothProgress);

        for (int s = 0; s < smoothIterations; s++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                for (int y = 0; y < terrainData.heightmapHeight; y++)
                {
                    averageHeight = heightMap[x, y];
                    List<Vector2> neighbours = GetNeighbours(new Vector2(x, y), terrainData.heightmapWidth, terrainData.heightmapHeight);

                    foreach (Vector2 n in neighbours)
                    {
                        averageHeight += heightMap[(int)n.x, (int)n.y];
                    }

                    heightMap[x, y] = averageHeight / ((float)neighbours.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothing Terrain", "Iteration: " + smoothProgress + " of " + smoothIterations, smoothProgress / smoothIterations);
        }
        EditorUtility.ClearProgressBar();
        terrainData.SetHeights(0, 0, heightMap);
    }

    public List<Vector2> GetNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1), Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                    {
                        neighbours.Add(nPos);
                    }
                }
            }
        }
        return neighbours;
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        terrainData.SetHeights(0, 0, heightMap);
    }

    //Texturing
    public void SplatMaps()
    {
        SplatPrototype[] newSplatPrototypes = new SplatPrototype[splatHeights.Count];
        int spindex = 0;
        foreach (SplatHeights sh in splatHeights)
        {
            newSplatPrototypes[spindex] = new SplatPrototype();
            newSplatPrototypes[spindex].texture = sh.texture;
            newSplatPrototypes[spindex].texture.Apply(true);
            spindex++;
        }
        terrainData.splatPrototypes = newSplatPrototypes;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float[] splat = new float[terrainData.alphamapLayers];
                for (int i = 0; i < splatHeights.Count; i++)
                {
                    float noise = Mathf.PerlinNoise(x * splatNoiseX, y * splatNoiseY) * splatNoiseScale;
                    float offset = splatOffset + noise;
                    float thisHeightStart = splatHeights[i].minHeight - offset;
                    float thisHeightStop = splatHeights[i].maxHeight + offset;
                    //float steepness = GetSteepness(heightMap, x, y, terrainData.heightmapWidth, terrainData.heightmapHeight);
                    float steepness = terrainData.GetSteepness(((float)y) / terrainData.alphamapHeight, ((float)x) / terrainData.alphamapWidth);

                    if ((heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop) && (steepness >= splatHeights[i].minSlope && steepness <= splatHeights[i].maxSlope))
                    {
                        splat[i] = 1;
                    }
                }
                NormalizeVector(splat);
                for (int j = 0; j < splatHeights.Count; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    public void NormalizeVector(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }

        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }

    public void AddNewSplatHeight()
    {
        splatHeights.Add(new SplatHeights());
    }

    public void RemoveSplatHeight()
    {
        List<SplatHeights> keptSplatHeights = new List<SplatHeights>();
        for (int i = 0; i < splatHeights.Count; i++)
        {
            if (!splatHeights[i].remove)
            {
                keptSplatHeights.Add(splatHeights[i]);
            }
        }
        if (keptSplatHeights.Count == 0)
        {
            keptSplatHeights.Add(splatHeights[0]);
        }
        splatHeights = keptSplatHeights;
    }

    public float GetSteepness(float[,] heightmap, int x, int y, int width, int height)
    {
        float h = heightmap[x, y];
        int nx = x + 1;
        int ny = y + 1;

        if (nx > width - 1) nx = x - 1;
        if (ny > height - 1) ny = y - 1;

        float dx = heightmap[nx, y] - h;
        float dy = heightmap[x, ny] - h;
        Vector2 gradient = new Vector2(dx, dy);

        float steep = gradient.magnitude;

        return steep;
    }

    //Details
    public void SetWater()
    {
        GameObject water = GameObject.Find("Water");
        if (!water)
        {
            water = Instantiate(waterObject, this.transform.position, this.transform.rotation);
            water.name = "Water";
        }
        water.transform.position = this.transform.position + new Vector3(terrainData.size.x / 2, waterHeight * terrainData.size.y, terrainData.size.z / 2);
        water.transform.localScale = new Vector3(terrainData.size.x / 10, 1, terrainData.size.z / 10);
    }

    public void SetShoreline()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        int quadCount = 0;
        //GameObject quads = new GameObject("Quads");

        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbours = GetNeighbours(thisLocation, terrainData.heightmapWidth, terrainData.heightmapHeight);

                foreach (Vector2 n in neighbours)
                {
                    if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                    {
                        if (quadCount > 10)
                        {
                            quadCount++;
                            GameObject ShorePiece = GameObject.CreatePrimitive(PrimitiveType.Quad);
                            ShorePiece.transform.localScale *= 25f;

                            ShorePiece.transform.position = this.transform.position + new Vector3(y / (float)terrainData.heightmapHeight * terrainData.size.z, waterHeight * terrainData.size.y, x / (float)terrainData.heightmapWidth * terrainData.size.x);
                            ShorePiece.transform.LookAt(new Vector3(n.y / (float)terrainData.heightmapHeight * terrainData.size.z, waterHeight * terrainData.size.y, n.x / (float)terrainData.heightmapWidth * terrainData.size.x));
                            ShorePiece.transform.Rotate(90, 0, 0);
                            ShorePiece.tag = "Shore";
                            //ShorePiece.transform.parent = quads.transform;
                            quadCount = 0;
                        }
                        quadCount++;
                    }
                }
            }
        }

        GameObject[] shoreQuads = GameObject.FindGameObjectsWithTag("Shore");
        MeshFilter[] meshFilters = new MeshFilter[shoreQuads.Length];
        for (int m = 0; m < shoreQuads.Length; m++)
        {
            meshFilters[m] = shoreQuads[m].GetComponent<MeshFilter>();
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.active = false;
            i++;
        }

        GameObject currentShoreLine = GameObject.Find("ShoreLine");
        if (currentShoreLine)
        {
            DestroyImmediate(currentShoreLine);
        }
        GameObject shoreLine = new GameObject();
        shoreLine.name = "ShoreLine";
        //shoreLine.AddComponent<WaveAnimation>();
        shoreLine.transform.position = this.transform.position;
        shoreLine.transform.rotation = this.transform.rotation;
        shoreLine.transform.position = new Vector3(0, 0.1f, 0);
        MeshFilter thisMF = shoreLine.AddComponent<MeshFilter>();
        thisMF.mesh = new Mesh();
        shoreLine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

        MeshRenderer r = shoreLine.AddComponent<MeshRenderer>();
        r.sharedMaterial = shoreLineMaterial;

        for (int sQ = 0; sQ < shoreQuads.Length; sQ++)
        {
            DestroyImmediate(shoreQuads[sQ]);
        }
    }

    public void Erode()
    {
        if (erosionType == ErosionType.Rain)
            Rain();
        else if (erosionType == ErosionType.Tidal)
            Tidal();
        else if (erosionType == ErosionType.Thermal)
            Thermal();
        else if (erosionType == ErosionType.River)
            River();
        else if (erosionType == ErosionType.Wind)
            Wind();
    }

    public void Rain()
    {

    }

    public void Tidal()
    {

    }

    public void Thermal()
    {

    }

    public void River()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        float[,] erosionMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];

        for (int i = 0; i < erosionDroplets; i++)
        {
            Vector2 dropletPosition = new Vector2(UnityEngine.Random.Range(0, terrainData.heightmapWidth), UnityEngine.Random.Range(0, terrainData.heightmapHeight));
            erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] = erosionStrength;
            for (int j = 0; j < erosionSpringsPerDroplet; j++)
            {
                erosionMap = RunRiver(dropletPosition, heightMap, erosionMap, terrainData.heightmapWidth, terrainData.heightmapHeight);
            }
        }
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                if (erosionMap[x, y] > 0)
                {
                    heightMap[x, y] -= erosionMap[x, y];
                    if (heightMap[x, y] < 0.1)
                    {
                        heightMap[x, y] = 0.1f;
                    }

                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public float[,] RunRiver(Vector3 dropletPosition, float[,] heightMap, float[,] erosionMap, int width, int height)
    {
        while(erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] > 0)
        {
            List<Vector2> neighbours = GetNeighbours(dropletPosition, width, height);
            neighbours.Shuffle();
            bool foundLower = false;
            foreach(Vector2 n in neighbours)
            {
                if(heightMap[(int)n.x, (int)n.y] < heightMap[(int)dropletPosition.x, (int)dropletPosition.y])
                {
                    erosionMap[(int)n.x, (int)n.y] = erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] - erosionSolubility;
                    dropletPosition = n;
                    foundLower = true;
                    break;
                }
            }
            if(!foundLower)
            {
                erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] -= erosionSolubility;
            }
        }
        return erosionMap;
    }

    public void Wind()
    {

    }

    //Utilities
    void OnEnable()
    {
        terrain = this.GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
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