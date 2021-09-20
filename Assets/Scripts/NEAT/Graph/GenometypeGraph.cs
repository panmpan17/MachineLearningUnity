using UnityEngine;
using XNode;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace NEAT.Graph
{
    public class GenometypeGraph : NodeGraph
    {
    #if UNITY_EDITOR
        [MenuItem("Assets/Create/Dialogue Graph", false, 0)]
        public static void CreateAsset()
        {
            GenometypeGraph asset = ScriptableObject.CreateInstance<GenometypeGraph>();

            AssetDatabase.CreateAsset(asset, "Assets/New Genometype Graph.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    #endif
    }
}