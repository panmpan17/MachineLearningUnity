using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
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

        public void InputValueIntoNode(NodeGenes node, float value)
        {
            switch (node.operatorType)
            {
                case NodeGenes.OperatorType.Multiply:
                    node.value *= value;
                    break;
                case NodeGenes.OperatorType.Plus:
                    node.value += value;
                    break;
            }
        }
        public void InputValueIntoNode(int nodeIndex, float value)
        {
            switch (nodeGenes[nodeIndex].operatorType)
            {
                case NodeGenes.OperatorType.Multiply:
                    nodeGenes[nodeIndex].value *= value;
                    break;
                case NodeGenes.OperatorType.Plus:
                    nodeGenes[nodeIndex].value += value;
                    break;
            }
        }

        public float GetNodeValue(int nodeIndex)
        {
            return nodeGenes[nodeIndex].Value;
        }

        #region Node and Connection
        public struct NodeGenes
        {
            public Types type;
            public float value;
            public OperatorType operatorType;
            public OutputMode outputMode;

            public float Value
            {
                get {
                    return outputMode == OutputMode.Normal? value: - value;
                }
            }

            public NodeGenes(Types _type, float _value, OperatorType _operatorType=OperatorType.Multiply, OutputMode _outputMode=OutputMode.Normal)
            {
                value = _value;
                type = _type;
                operatorType = _operatorType;
                outputMode = _outputMode;
            }

            public enum Types
            {
                Input,
                Output,
                Hidden,
            }

            public enum OperatorType
            {
                Multiply,
                Plus,
            }

            public enum OutputMode
            {
                Normal,
                Reverse
            }
        }

        public struct ConnectionGenens
        {
            public int inputNodeIndex;
            public int outputNodeIndex;
            public float weight;
            public bool enabled;

            public ConnectionGenens(int _inputNodeIndex, int _outputNodeIndex, float _weight, bool _enabled=true)
            {
                inputNodeIndex = _inputNodeIndex;
                outputNodeIndex = _outputNodeIndex;
                weight = _weight;
                enabled = _enabled;
            }
        }
        #endregion
    }
}