using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    [System.Serializable]
    public class Genometype
    {
        public NodeGenes[] nodeGenes;
        public ConnectionGenens[] connectionGenes;

        public Genometype(NodeGenes[] _nodeGenes, ConnectionGenens[] _connectionGenes)
        {
            nodeGenes = _nodeGenes;
            connectionGenes = _connectionGenes;
        }

        public Genometype Clone()
        {
            NodeGenes[] newNodeGenes = new NodeGenes[nodeGenes.Length];
            for (int i = 0; i < nodeGenes.Length; i++)
            {
                newNodeGenes[i] = nodeGenes[i];
            }

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
            for (int i = 0; i < nodeGenes.Length; i++)
            {
                data += string.Format("{0} {1}: {2}\n", nodeGenes[i].type, nodeGenes[i].outputMode, nodeGenes[i].value);
            }
            data += "\n";
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
            public Types type;
            public float value;
            public OutputMode outputMode;

            public float Value
            {
                get {
                    return outputMode == OutputMode.Normal? value: - value;
                }
            }

            public NodeGenes(Types _type, float _value=0, OutputMode _outputMode=OutputMode.Normal)
            {
                value = _value;
                type = _type;
                outputMode = _outputMode;
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
            public int inputNodeIndex;
            public int outputNodeIndex;
            public float weight;
            public OperatorType operatorType;
            public bool enabled;

            public ConnectionGenens(int _inputNodeIndex, int _outputNodeIndex, float _weight, OperatorType _operatorType=OperatorType.Multiply, bool _enabled=true)
            {
                inputNodeIndex = _inputNodeIndex;
                outputNodeIndex = _outputNodeIndex;
                weight = _weight;
                operatorType = _operatorType;
                enabled = _enabled;
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