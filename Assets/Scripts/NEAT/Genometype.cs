using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    [System.Serializable]
    public class Genometype
    {
        public static bool StructureIsSame(Genometype genome1, Genometype genome2)
        {
            if (genome1.connectionGenes == null || genome2.connectionGenes == null)
                return genome1.connectionGenes == null && genome2.connectionGenes == null;

            if (genome1.connectionGenes.Length != genome2.connectionGenes.Length)
                return false;

            for (int i = 0; i < genome1.connectionGenes.Length; i++)
            {
                bool matched = true;
                for (int e = 0; e < genome2.connectionGenes.Length; e++)
                {
                    if (genome1.connectionGenes[i].innovationUUID != genome2.connectionGenes[i].innovationUUID)
                    {
                        matched = false;
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
                data += string.Format("{0}: {1}\n", nodeGenes[i].type, nodeGenes[i].addOnType);
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
            /// The input or output index
            /// </summary>
            public int IOIndex;

            /// <summary>
            /// The value store in the node
            /// </summary>
            [System.NonSerialized]
            public float value;

            /// <summary>
            /// UUID of this node, use to line up genome when cross over
            /// </summary>
            public string UUID;

            /// <summary>
            /// How the number add on to the original number in this node
            /// </summary>
            public AddOnType addOnType;

            public NodeGenes(Types _type, int _IOIndex=0, AddOnType _addOnType=AddOnType.Plus)
            {
                type = _type;
                IOIndex = _IOIndex;
                value = 0;

                addOnType = _addOnType;

                UUID = System.Guid.NewGuid().ToString();
            }

            public enum Types
            {
                Input,
                Output,
                Hidden,
            }

            public enum AddOnType
            {
                Plus,
                Multiply,
            }
        }

        [System.Serializable]
        public struct ConnectionGenens
        {
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
            /// The innvation UUID of this connection, use to line up genome when cross over
            /// </summary>
            public string innovationUUID;

            public ConnectionGenens(int _inputNodeIndex, int _outputNodeIndex, float _weight, OperatorType _operatorType=OperatorType.Multiply, bool _enabled=true)
            {
                inputNodeIndex = _inputNodeIndex;
                outputNodeIndex = _outputNodeIndex;
                weight = _weight;
                operatorType = _operatorType;
                enabled = _enabled;

                innovationUUID = System.Guid.NewGuid().ToString();
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