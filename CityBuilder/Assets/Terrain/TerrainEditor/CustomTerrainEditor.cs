using UnityEngine;
using UnityEditor;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]


public class CustomTerrainEditor : Editor {


    //Properties -------
    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty perlinScaleX;
    SerializedProperty perlinScaleY;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty perlinFrequency;
    SerializedProperty resetTerrain;
    SerializedProperty perlinParameters;
    GUITableState perlinParameterTable;
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiPeakCount;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiType;
    SerializedProperty mpdRoughness;
    SerializedProperty mpdHeightMin;
    SerializedProperty mpdHeightMax;
    SerializedProperty mpdHeightDampener;
    SerializedProperty smoothIterations;
    GUITableState splatMapTable;
    SerializedProperty splatHeights;
    SerializedProperty splatOffset;
    SerializedProperty splatNoiseScale;
    SerializedProperty splatNoiseX;
    SerializedProperty splatNoiseY;
    SerializedProperty waterHeight;
    SerializedProperty waterObject;
    SerializedProperty shoreLineMaterial;
    SerializedProperty erosionSolubility;
    SerializedProperty erosionStrength;
    SerializedProperty erosionDroplets;
    SerializedProperty erosionSpringsPerDroplet;
    SerializedProperty erosionType;
    GUITableState vegetationRules;
    SerializedProperty vegetationMaxTrees;
    SerializedProperty vegetationTreeSpacing;

    //fold outs --------
    bool showRandom = false;
    bool showTexture = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlinNoise = false;
    bool showVoronoi = false;
    bool showmMPD = false;
    bool showSmooth = false;
    bool showSplatMaps = false;
    bool showWater = false;
    bool showErosion = false;
    bool showVegetation = false;

    void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        perlinScaleX = serializedObject.FindProperty("perlinScaleX");
        perlinScaleY = serializedObject.FindProperty("perlinScaleY");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        perlinFrequency = serializedObject.FindProperty("perlinFrequency");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiPeakCount = serializedObject.FindProperty("voronoiPeakCount");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiType = serializedObject.FindProperty("voronoiType");
        mpdRoughness = serializedObject.FindProperty("mpdRoughness");
        mpdHeightMin = serializedObject.FindProperty("mpdHeightMin");
        mpdHeightMax = serializedObject.FindProperty("mpdHeightMax");
        mpdHeightDampener = serializedObject.FindProperty("mpdHeightDampener");
        smoothIterations = serializedObject.FindProperty("smoothIterations");
        splatHeights = serializedObject.FindProperty("splatHeights");
        splatOffset = serializedObject.FindProperty("splatOffset");
        splatNoiseScale = serializedObject.FindProperty("splatNoiseScale");
        splatNoiseX = serializedObject.FindProperty("splatNoiseX");
        splatNoiseY = serializedObject.FindProperty("splatNoiseY");
        waterHeight = serializedObject.FindProperty("waterHeight");
        waterObject = serializedObject.FindProperty("waterObject");
        shoreLineMaterial = serializedObject.FindProperty("shoreLineMaterial");
        erosionSolubility = serializedObject.FindProperty("erosionSolubility");
        erosionStrength = serializedObject.FindProperty("erosionStrength");
        erosionDroplets = serializedObject.FindProperty("erosionDroplets");
        erosionSpringsPerDroplet = serializedObject.FindProperty("erosionSpringsPerDroplet");
        erosionType = serializedObject.FindProperty("erosionType");
        vegetationMaxTrees = serializedObject.FindProperty("vegetationMaxTrees");
        vegetationTreeSpacing = serializedObject.FindProperty("vegetationTreeSpacing");
    }

    // Updates the Editor GUI
    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        CustomTerrain terrain = (CustomTerrain) target;

        GUILayout.Label("Terrain generation", EditorStyles.boldLabel);

        //Random Terrain
        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights by Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if (GUILayout.Button("Random Heights"))
            {
                terrain.RandomTerrain();
            }
        }

        //Texture Terrain
        showTexture = EditorGUILayout.Foldout(showTexture, "Texture");
        if (showTexture)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights by Texture", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            if (GUILayout.Button("Load Texture"))
            {
                terrain.TextureTerrain();
            }

        }

        //PerlinNoise Terrain
        showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Perlin Noise");
        if(showPerlinNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights by Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinScaleX, 0, 0.05f, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinScaleY, 0, 0.05f, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 1000, new GUIContent("X Offset"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 1000, new GUIContent("Y Offset"));
            EditorGUILayout.IntSlider(perlinOctaves, 0, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("HeightScale"));
            EditorGUILayout.Slider(perlinFrequency, 0, 3, new GUIContent("Frequency"));

            if (GUILayout.Button("Generate"))
            {
                terrain.PerlinNoiseTerrain();
            }
        }

        //Multiple PerlinNoise Terrain
        showMultiplePerlinNoise = EditorGUILayout.Foldout(showMultiplePerlinNoise, "Multiple Perlin Noise");
        if (showMultiplePerlinNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights by Multiple Perlin Noises", EditorStyles.boldLabel);

            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, serializedObject.FindProperty("perlinParamList"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                terrain.AddNewPerlin();
            }
            if (GUILayout.Button("Remove"))
            {
                terrain.RemovePerlin();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Generate"))
            {
                terrain.MultiplePerlinNoiseTerrain();
            }
        }

        //Voronoi Terrain
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        if (showVoronoi)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set New Voronoi Point", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(voronoiPeakCount, 1, 10, new GUIContent("Peaks"));
            EditorGUILayout.Slider(voronoiFallOff, 0.1f, 10, new GUIContent("Fall off"));
            EditorGUILayout.Slider(voronoiDropOff, 0.1f, 10, new GUIContent("Drop off"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
            EditorGUILayout.PropertyField(voronoiType);
            if (GUILayout.Button("Generate"))
            {
                terrain.VoronoiTerrain();
            }
        }

        //MidPointDisplacement Terrain
        showmMPD = EditorGUILayout.Foldout(showmMPD, "Mid Point Displacement");
        if(showmMPD)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Mid Point Displacement", EditorStyles.boldLabel);
            EditorGUILayout.Slider(mpdHeightMin, -10, 5, "Min Height");
            EditorGUILayout.Slider(mpdHeightMax, -5, 10, "Max Height");
            EditorGUILayout.Slider(mpdHeightDampener, 0, 5, "Dampener");
            EditorGUILayout.Slider(mpdRoughness, 0, 5, "Roughness");
            if (GUILayout.Button("Generate"))
            {
                terrain.MPDTerrain();
            }
        }

        //MidPointDisplacement Terrain
        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth");
        if (showSmooth)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Smooth Terrain", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(smoothIterations, 1, 10, "Iterations");
            if (GUILayout.Button("Smooth"))
            {
                terrain.SmoothTerrain();
            }
        }

        //Erosion
        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if(showErosion)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Erode Terrain", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.IntSlider(erosionDroplets, 100, 6000, "Droplets");
            EditorGUILayout.IntSlider(erosionSpringsPerDroplet, 1, 5, "Springs");
            EditorGUILayout.Slider(erosionStrength, 0.001f, 0.1f, "Strength");
            EditorGUILayout.Slider(erosionSolubility, 0.0001f, 0.1f, "Solubility");
            if (GUILayout.Button("Erode"))
            {
                terrain.Erode();
            }
        }

        GUILayout.Label("Terrain texturing", EditorStyles.boldLabel);

        // SplatMaps
        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Splat Maps");
        if (showSplatMaps)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Splat Maps", EditorStyles.boldLabel);

            EditorGUILayout.Slider(splatOffset, 0f, 0.1f, new GUIContent("Offset"));
            EditorGUILayout.Slider(splatNoiseScale, 0f, 0.1f, new GUIContent("Noise Scale"));
            EditorGUILayout.Slider(splatNoiseX, 0f, 2f, new GUIContent("Noise X"));
            EditorGUILayout.Slider(splatNoiseY, 0f, 2f, new GUIContent("Noise Y"));

            splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                terrain.AddNewSplatHeight();
            }
            if (GUILayout.Button("Remove"))
            {
                terrain.RemoveSplatHeight();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Splatmaps"))
            {
                terrain.SplatMaps();
            }
        }

        GUILayout.Label("Details", EditorStyles.boldLabel);
        showWater = EditorGUILayout.Foldout(showWater, "Water");
        if (showWater)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Water", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(waterObject);
            EditorGUILayout.PropertyField(shoreLineMaterial);
            EditorGUILayout.Slider(waterHeight, 0, 1, "Water Height");
            if(GUILayout.Button("River"))
            {
                terrain.River();
            }
            if (GUILayout.Button("Set Water"))
            {
                terrain.SetWater();
                //terrain.SetShoreline();
            }


        }

        showVegetation = EditorGUILayout.Foldout(showVegetation, "Vegetation");
        if (showVegetation)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Vegetation", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(vegetationMaxTrees, 100, 10000, "Max Trees");
            EditorGUILayout.IntSlider(vegetationTreeSpacing, 1, 50, "Tree Spacing");

            vegetationRules = GUITableLayout.DrawTable(vegetationRules, serializedObject.FindProperty("vegetationRules"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                terrain.AddNewVegetationMesh();
            }
            if (GUILayout.Button("Remove"))
            {
                terrain.RemoveVegetationMesh();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Vegetation"))
            {
                terrain.SetVegetation();
            }
        }

        //Reset Terrain
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.PropertyField(resetTerrain);
   
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }
        if (GUILayout.Button("Generate Random Terrain"))
        {
            terrain.GenerateRandomTerrain();    
        }

        serializedObject.ApplyModifiedProperties();
    }
}
