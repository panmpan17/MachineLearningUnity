using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NEAT;
using NEAT.Graph;
using XNode;


namespace FlappyBird
{
    public class SavingBrowseWindow : EditorWindow
    {
        [MenuItem("Window/Saving Browse Window")]
        public static void OpenWindow()
        {
            GetWindow<SavingBrowseWindow>();
        }

        private string[] m_files;
        private int m_index;
        private GenomeEvolutionGameControl.GenomeStructEvolveData m_data;

        void OnEnable()
        {
            FindAvalibleFiles();
            GetFileData();
        }

        public void FindAvalibleFiles()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath);
            List<string> fileList = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i].Replace(Application.persistentDataPath, "").Substring(1);
                if (fileName.IndexOf(".") >= 0)
                    continue;

                fileList.Add(fileName);
            }

            m_files = fileList.ToArray();
        }

        public void GetFileData()
        {
            string filePath = Path.Combine(Application.persistentDataPath, m_files[m_index]);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);
                m_data = (GenomeEvolutionGameControl.GenomeStructEvolveData)binaryFormatter.Deserialize(stream);
                stream.Close();
            }
        }

        void OnGUI()
        {
            for (int i = 0; i < m_files.Length; i++)
            {
                EditorGUILayout.LabelField(m_files[i]);
            }
            GUILayout.Space(5);

            for (int i = 0; i < m_data.aliveGenomes.Length; i++)
            {
                EditorGUILayout.LabelField(i.ToString());
                if (GUILayout.Button("Open graph"))
                {
                    GenometypeGraphVisualizer visualizer = new GenometypeGraphVisualizer(m_data.aliveGenomes[i]);
                    visualizer.Start();
                }
            }
        }
    }

    public class GenometypeGraphVisualizer
    {
        private Genometype m_genometype;

        private GenometypeGraph m_graph;

        public GenometypeGraphVisualizer(Genometype genometype)
        {
            m_genometype = genometype;
        }

        public void Start()
        {
            // Initialize graph
            m_graph = ScriptableObject.CreateInstance<GenometypeGraph>();
            AssetDatabase.CreateAsset(m_graph, "Assets/New Genometype Graph.asset");
            AssetDatabase.SaveAssets();

            GenerateNodeGenes();
            GenerateConnectionGenes();

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = m_graph;
        }

        void GenerateNodeGenes()
        {
            float y = 0;
            for (int i = 0; i < m_genometype.nodeGenes.Length; i++)
            {
                Node.graphHotfix = m_graph;

                GenomeNodeNode node = ScriptableObject.CreateInstance<GenomeNodeNode>();
                node.name = "NodeGene-" + i;
                node.graph = m_graph;

                node.type = m_genometype.nodeGenes[i].type;
                node.IOIndex = m_genometype.nodeGenes[i].IOIndex;
                node.outputMode = m_genometype.nodeGenes[i].outputMode;

                node.position = new Vector2(0, y);
                y += 150;

                m_graph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(m_graph)))
                    AssetDatabase.AddObjectToAsset(node, m_graph);
            }
        }

        void GenerateConnectionGenes()
        {
            for (int i = 0; i < m_genometype.connectionGenes.Length; i++)
            {
                Node.graphHotfix = m_graph;

                GenomeConnectionNode node = ScriptableObject.CreateInstance<GenomeConnectionNode>();
                node.name = "ConnectionGene-" + i;
                node.graph = m_graph;

                Genometype.ConnectionGenens genens = m_genometype.connectionGenes[i];

                node.weight = genens.weight;
                node.operatorType = genens.operatorType;
                node.enabled = genens.enabled;

                Vector2 position = new Vector2(250, 0);

                if (genens.inputNodeIndex != -1)
                {
                    Node otherNode = m_graph.nodes[genens.inputNodeIndex];
                    node.GetInputPort("inputData").Connect(otherNode.GetOutputPort("outputData"));
                    position.x = otherNode.position.x + 250;
                    position.y = otherNode.position.y;
                }
                if (genens.outputNodeIndex != -1)
                {
                    Node otherNode = m_graph.nodes[genens.outputNodeIndex];
                    node.GetOutputPort("outputData").Connect(otherNode.GetInputPort("inputData"));

                    otherNode.position = new Vector2(position.x + 250, position.y);
                }

                node.position = position;

                m_graph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(m_graph)))
                    AssetDatabase.AddObjectToAsset(node, m_graph);
            }
        }
    }
}