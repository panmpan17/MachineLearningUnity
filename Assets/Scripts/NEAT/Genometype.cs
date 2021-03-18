using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    [System.Serializable]
    public class Genometype
    {
        public static bool Compare(Genometype genome1, Genometype genome2)
        {
            if (genome1.connectionGenes == null || genome2.connectionGenes == null)
                return genome1.connectionGenes == null && genome2.connectionGenes == null;

            for (int i = 0; i < genome1.connectionGenes.Length; i++)
            {
                bool matched = true;
                for (int e = 0; e < genome2.connectionGenes.Length; e++)
                {
                    if (genome1.connectionGenes[i].innovationNumber == genome2.connectionGenes[i].innovationNumber)
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched) return false;
            }
            return true;
        }

        public NodeGenes[] nodeGenes;
        public ConnectionGenens[] connectionGenes;

        public Genometype(NodeGenes[] _nodeGenes)
        {
            nodeGenes = _nodeGenes;
            connectionGenes = new ConnectionGenens[0];
        }

        public Genometype(NodeGenes[] _nodeGenes, ConnectionGenens[] _connectionGenes)
        {
            nodeGenes = _nodeGenes;
            connectionGenes = _connectionGenes;
        }

        /// <summary>
        /// Make a clone of the genome
        /// </summary>
        /// <returns>The new genome</returns>
        public Genometype Clone()
        {
            // Copy the nodes
            NodeGenes[] newNodeGenes = new NodeGenes[nodeGenes.Length];
            for (int i = 0; i < nodeGenes.Length; i++)
            {
                newNodeGenes[i] = nodeGenes[i];
            }

            // Copy the connection
            ConnectionGenens[] newConnectionGenes = new ConnectionGenens[connectionGenes.Length];
            for (int i = 0; i < connectionGenes.Length; i++)
            {
                newConnectionGenes[i] = connectionGenes[i];
            }

            return new Genometype(newNodeGenes, newConnectionGenes);
        }

        public override string ToString()
        {
            string data = "";

            // Loop through the nodes, and turn its data into string
            for (int i = 0; i < nodeGenes.Length; i++)
            {
                data += string.Format("{0} {1}: {2}\n", nodeGenes[i].type, nodeGenes[i].outputMode, nodeGenes[i].value);
            }

            data += "\n";

            // Loop through the connections, and turn its data into string
            for (int i = 0; i < connectionGenes.Length; i++)
            {
                if (connectionGenes[i].enabled)
                    data += string.Format("{0} > {1}: {2} {3}\n", connectionGenes[i].inputNodeIndex, connectionGenes[i].outputNodeIndex, connectionGenes[i].weight, connectionGenes[i].operatorType);
            }
            return data;
        }

        #region Node and Connection
        [System.Serializable]
        public struct NodeGenes
        {
            /// <summary>
            /// The type of this node
            /// </summary>
            public Types type;

            /// <summary>
            /// How to output the value
            /// </summary>
            public OutputMode outputMode;

            /// <summary>
            /// The input or output index
            /// </summary>
            public int IOIndex;

            /// <summary>
            /// The value store in the node
            /// </summary>
            [System.NonSerialized]
            public float value;

            public float Value
            {
                get {
                    return outputMode == OutputMode.Normal? value: - value;
                }
            }

            public NodeGenes(Types _type, OutputMode _outputMode=OutputMode.Normal, int _IOIndex=0)
            {
                type = _type;
                outputMode = _outputMode;
                IOIndex = _IOIndex;
                value = 0;
            }

            public enum Types
            {
                Input,
                Output,
                Hidden,
            }

            public enum OutputMode
            {
                Normal,
                Reverse
            }
        }

        [System.Serializable]
        public struct ConnectionGenens
        {
            public static int InnovationNumber = 0;

            /// <summary>
            /// The index of the input node in the genome
            /// </summary>
            public int inputNodeIndex;

            /// <summary>
            /// The index of the output node in the genome
            /// </summary>
            public int outputNodeIndex;

            /// <summary>
            /// The weight of this connection
            /// </summary>
            public float weight;

            /// <summary>
            /// How to calculate the weight
            /// </summary>
            public OperatorType operatorType;

            /// <summary>
            /// Wether this connection is avalible
            /// </summary>
            public bool enabled;

            /// <summary>
            /// The innvation number of this connection, use to  line up genome when cross over
            /// </summary>
            public int innovationNumber;

            public ConnectionGenens(int _inputNodeIndex, int _outputNodeIndex, float _weight, OperatorType _operatorType=OperatorType.Multiply, bool _enabled=true)
            {
                inputNodeIndex = _inputNodeIndex;
                outputNodeIndex = _outputNodeIndex;
                weight = _weight;
                operatorType = _operatorType;
                enabled = _enabled;

                innovationNumber = InnovationNumber++;
            }

            public enum OperatorType
            {
                Multiply,
                Plus,
            }
        }
        #endregion
    }
}