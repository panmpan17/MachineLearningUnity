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
        private int m_index = -1;
        private GenomeEvolutionGameControl.GenomeStructEvolveData m_data;

        void OnEnable()
        {
            FindAvalibleFiles();
        }

        public void FindAvalibleFiles()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath);
            List<string> fileList = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i].Replace(Application.persistentDataPath, "").Substring(1);
                fileList.Add(fileName);
            }

            m_files = fileList.ToArray();
        }

        public void GetFileData()
        {
            string filePath = Path.Combine(Application.persistentDataPath, m_files[m_index]);

            try {
                m_data = SavingSystem.ReadData<GenomeEvolutionGameControl.GenomeStructEvolveData>(filePath, m_files[m_index].EndsWith(".json"));
            }
            catch (System.Exception)
            {
                m_index = -1;
            }
        }

        public void ConvertToJson(int index)
        {
            string filePath = Path.Combine(Application.persistentDataPath, m_files[index]);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            if (File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);
                m_data = (GenomeEvolutionGameControl.GenomeStructEvolveData)binaryFormatter.Deserialize(stream);
                stream.Close();

                SavingSystem.SaveData<GenomeEvolutionGameControl.GenomeStructEvolveData>(m_files[index] + ".json", m_data, true);
            }
        }

        void OnGUI()
        {
            for (int i = 0; i < m_files.Length; i++)
            {
                EditorGUILayout.LabelField(m_files[i]);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Open"))
                {
                    m_index = i;
                    GetFileData();
                }

                if (GUILayout.Button("Convert To JSON"))
                {
                    ConvertToJson(i);
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);

            if (m_data.aliveGenomes != null)
            {
                for (int i = 0; i < m_data.aliveGenomes.Length; i++)
                {
                    EditorGUILayout.LabelField(i.ToString());
                    if (GUILayout.Button("Open graph"))
                    {
                        GenometypeGraphVisualizer visualizer = new GenometypeGraphVisualizer(m_data.aliveGenomes[i]);
                        visualizer.Export("Genome-" + i, true);
                    }
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

        public void Export(string fileName, bool withoutConnectionNode=false)
        {
            // Initialize graph
            m_graph = ScriptableObject.CreateInstance<GenometypeGraph>();
            AssetDatabase.CreateAsset(m_graph, "Assets/" + fileName + ".asset");
            AssetDatabase.SaveAssets();

            GenerateNodeGenes();
            if (withoutConnectionNode)
                GenerateConnectionGenes();
            else
                GenerateConnectionGenesWithoutConnectionNode();

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

        void GenerateConnectionGenesWithoutConnectionNode()
        {
            for (int i = 0; i < m_genometype.connectionGenes.Length; i++)
            {
                Node.graphHotfix = m_graph;
                Genometype.ConnectionGenens genens = m_genometype.connectionGenes[i];

                Vector2 position = new Vector2(250, 0);

                if (genens.inputNodeIndex != -1 && genens.outputNodeIndex != -1)
                {
                    Node firstNode = m_graph.nodes[genens.inputNodeIndex];
                    Node secondNode = m_graph.nodes[genens.outputNodeIndex];
                    firstNode.GetOutputPort("outputData").Connect(secondNode.GetInputPort("inputData"));

                    secondNode.position.x += 250;
                }
            }
        }
    }
}