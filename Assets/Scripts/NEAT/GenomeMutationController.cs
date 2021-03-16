using UnityEngine;
using System.Collections.Generic;

namespace NEAT
{
    public class GenomeMutationController
    {
        private Genometype m_originGenome;

        private List<int> dataInputNodes;
        private List<int> dataOutputNodes;

        private List<NodeIndexPair> existPairs;

        public List<Genometype> mutations;

        // public bool ;
        public float randomWeightMin = -10f, randomWeightMax =  10f;

        public GenomeMutationController(Genometype genome)
        {
            m_originGenome = genome;


            dataInputNodes = new List<int>();
            dataOutputNodes = new List<int>();

            existPairs = new List<NodeIndexPair>();

            mutations = new List<Genometype>();

            AnalyzeGenome();
        }


        private void AnalyzeGenome()
        {
            // Loop thorught genome's node, store the index in the list
            for (int i = 0; i < m_originGenome.nodeGenes.Length; i++)
            {
                switch (m_originGenome.nodeGenes[i].type)
                {
                    case Genometype.NodeGenes.Types.Input:
                        dataInputNodes.Add(i);
                        break;
                    case Genometype.NodeGenes.Types.Hidden:
                        dataInputNodes.Add(i);
                        dataOutputNodes.Add(i);
                        break;
                    case Genometype.NodeGenes.Types.Output:
                        dataOutputNodes.Add(i);
                        break;
                }
            }

            // Loop throguht genome's connection, store pair index in the list
            for (int i = 0; i < m_originGenome.connectionGenes.Length; i++)
            {
                existPairs.Add(new NodeIndexPair(m_originGenome.connectionGenes[i]));
            }
        }

        public bool MutateByAddingConnection()
        {
            // Loop through the input node and output node, pair them up if they are new
            List<NodeIndexPair> possiblePairs = new List<NodeIndexPair>();

            for (int i = 0; i < dataInputNodes.Count; i++)
            {
                for (int e = 0; e < dataOutputNodes.Count; e++)
                {
                    if (dataInputNodes[i] != dataOutputNodes[e])
                    {
                        NodeIndexPair newPair = new NodeIndexPair(dataInputNodes[i], dataOutputNodes[e]);

                        bool overlap = false;

                        // Loop through exist pairs to check for overlap
                        for (int piarIndex = 0; piarIndex < existPairs.Count && !overlap; piarIndex++)
                        {
                            overlap = NodeIndexPair.Compare(newPair, existPairs[piarIndex]);
                        }
                        // Loop through possible pairs to check for overlap
                        for (int piarIndex = 0; piarIndex < possiblePairs.Count && !overlap; piarIndex++)
                        {
                            overlap = NodeIndexPair.Compare(newPair, possiblePairs[piarIndex]);
                        }

                        if (!overlap)
                        {
                            possiblePairs.Add(newPair);
                        }
                    }
                }
            }

            // Loop through possible pairs to generate mutations
            for (int i = 0; i < possiblePairs.Count; i++)
            {
                Genometype.ConnectionGenens newConnection = new Genometype.ConnectionGenens(
                    possiblePairs[i].nodeIndex1, possiblePairs[i].nodeIndex2,
                    Random.Range(randomWeightMin, randomWeightMax), Genometype.ConnectionGenens.OperatorType.Multiply);
                
                mutations.Add(MutateGenome(newConnection));

                newConnection = new Genometype.ConnectionGenens(
                    possiblePairs[i].nodeIndex1, possiblePairs[i].nodeIndex2,
                    Random.Range(randomWeightMin, randomWeightMax), Genometype.ConnectionGenens.OperatorType.Plus);

                mutations.Add(MutateGenome(newConnection));
            }

            return false;
        }

        public bool MutateByInsertNodeBetweenConnection()
        {
            for (int i = 0; i < m_originGenome.connectionGenes.Length; i++)
            {
                // The inbetween node
                int middleNodeIndex = m_originGenome.nodeGenes.Length;
                Genometype.NodeGenes newNode = new Genometype.NodeGenes(Genometype.NodeGenes.Types.Hidden);

                // The replacement connection
                Genometype.ConnectionGenens firstHalfConnection = new Genometype.ConnectionGenens(
                    m_originGenome.connectionGenes[i].inputNodeIndex, middleNodeIndex, 1);

                Genometype.ConnectionGenens secondHalfConnection = new Genometype.ConnectionGenens(
                    middleNodeIndex, m_originGenome.connectionGenes[i].outputNodeIndex,
                    m_originGenome.connectionGenes[i].weight);

                // Create the new genome, and disabled the original connection
                Genometype newGenome = MutateGenome(newNode, new Genometype.ConnectionGenens[] {
                    firstHalfConnection,
                    secondHalfConnection,
                });
                newGenome.connectionGenes[i].enabled = false;
                
                mutations.Add(newGenome);
            }

            return false;
        }



        /// <summary>
        /// Similar to clone, except add new connection while cloning
        /// </summary>
        /// <param name="newConnections">The new connections</param>
        /// <returns>The new genome</returns>
        private Genometype MutateGenome(Genometype.ConnectionGenens newConnection)
        {
            // Copy the nodes
            Genometype.NodeGenes[] newNodeGenes = new Genometype.NodeGenes[m_originGenome.nodeGenes.Length];
            for (int i = 0; i < m_originGenome.nodeGenes.Length; i++)
            {
                newNodeGenes[i] = m_originGenome.nodeGenes[i];
            }

            // Copy the connection
            Genometype.ConnectionGenens[] newConnectionGenes = new Genometype.ConnectionGenens[m_originGenome.connectionGenes.Length + 1];
            for (int i = 0; i < m_originGenome.connectionGenes.Length; i++)
            {
                newConnectionGenes[i] = m_originGenome.connectionGenes[i];
            }
            // Add new connection at the end
            newConnectionGenes[m_originGenome.connectionGenes.Length] = newConnection;

            return new Genometype(newNodeGenes, newConnectionGenes);
        }

        /// <summary>
        /// Similar to clone, except add new connection and new node while cloning
        /// </summary>
        /// <param name="newNode">The new node</param>
        /// <param name="newConnections">Array of the new connections</param>
        /// <returns>The new genome</returns>
        private Genometype MutateGenome(Genometype.NodeGenes newNode, Genometype.ConnectionGenens[] newConnections)
        {
            // Copy the nodes
            Genometype.NodeGenes[] newNodeGenes = new Genometype.NodeGenes[m_originGenome.nodeGenes.Length + 1];
            for (int i = 0; i < m_originGenome.nodeGenes.Length; i++)
            {
                newNodeGenes[i] = m_originGenome.nodeGenes[i];
            }
            newNodeGenes[m_originGenome.nodeGenes.Length] = newNode;

            // Copy the connection
            Genometype.ConnectionGenens[] newConnectionGenes = new Genometype.ConnectionGenens[m_originGenome.connectionGenes.Length + newConnections.Length];
            for (int i = 0; i < m_originGenome.connectionGenes.Length; i++)
            {
                newConnectionGenes[i] = m_originGenome.connectionGenes[i];
            }
            for (int i = 0; i < newConnections.Length; i++)
            {
                newConnectionGenes[i + m_originGenome.connectionGenes.Length] = newConnections[i];
            }

            return new Genometype(newNodeGenes, newConnectionGenes);
        }


        private struct NodeIndexPair
        {
            public static bool Compare(NodeIndexPair pair1, NodeIndexPair pair2)
            {
                // Check pair's node index 1 is same as pair's node index 1
                if (pair1.nodeIndex1 == pair2.nodeIndex1)
                {
                    return pair1.nodeIndex2 == pair2.nodeIndex2;
                }
                // Check pair's node index 1 is same as pair's node index 2
                else if (pair1.nodeIndex1 == pair2.nodeIndex2)
                {
                    return pair1.nodeIndex2 == pair2.nodeIndex1;
                }

                return false;
            }

            public int nodeIndex1;
            public int nodeIndex2;

            public NodeIndexPair(int index1, int index2)
            {
                nodeIndex1 = index1;
                nodeIndex2 = index2;
            }
            public NodeIndexPair(Genometype.ConnectionGenens connection)
            {
                nodeIndex1 = connection.inputNodeIndex;
                nodeIndex2 = connection.outputNodeIndex;
            }
        }
    }
}