using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region Node and Connection
    public struct NodeGenes
    {
        public float value;
        public Types type;

        public enum Types {
            Sensor,
            Output,
            Hidden,
        }

        public NodeGenes(float _value, Types _type)
        {
            value = _value;
            type = _type;
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
