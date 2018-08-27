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
                                                 

    //fold outs --------
    bool showRandom = false;
    bool showTexture = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlinNoise = false;
    bool showVoronoi = false;

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
    }

    // Updates the Editor GUI
    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        CustomTerrain terrain = (CustomTerrain) target;

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


        //Reset Terrain
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.PropertyField(resetTerrain);
   
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }

        serializedObject.ApplyModifiedProperties();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
