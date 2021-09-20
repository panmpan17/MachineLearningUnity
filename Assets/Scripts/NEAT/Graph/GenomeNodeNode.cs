using UnityEngine;
using XNode;

namespace NEAT.Graph
{
    [NodeTint(91f / 255f, 100f / 255f, 168f / 255f)]
    public class GenomeNodeNode : Node
    {
        [Input]
        public float inputData;

        [Output]
        public float outputData;
        
        public Genometype.NodeGenes.Types type;
        public int IOIndex;
        public Genometype.NodeGenes.OutputMode outputMode;
    }
}