using UnityEngine;
using XNode;

namespace NEAT.Graph
{
    public class GenomeConnectionNode : Node
    {
        [Input]
        public float inputData;

        [Output]
        public float outputData;

        public float weight;
        public Genometype.ConnectionGenens.OperatorType operatorType;
        public bool enabled;
    }
}