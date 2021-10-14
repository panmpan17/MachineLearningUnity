using System.Collections;
using System.Collections.Generic;

namespace NEAT
{
    public class GenomeController
    {
        public Genometype genome;
        private Genometype m_genome;

        public GenomeController(Genometype _genome)
        {
            genome = _genome;
        }

        public void Reset()
        {
            m_genome = genome.Clone();
            // for (int i = 0; i < m_genome.connectionGenes.Length; i++)
            // m_genome.nodeGenes[i].value = 0;
        }


        #region Input
        public void Input(float[] inputDatas)
        {
            for (int i = 0; i < inputDatas.Length; i++)
            {
                for (int e = 0; e < m_genome.nodeGenes.Length; e++)
                {
                    if (m_genome.nodeGenes[e].type == Genometype.NodeGenes.Types.Input && m_genome.nodeGenes[e].IOIndex == i)
                    {
                        m_genome.nodeGenes[e].value = inputDatas[i];
                    }
                }
            }
        }
        #endregion


        #region Process Data
        public void StartProcess()
        {
            for (int i = 0; i < m_genome.connectionGenes.Length; i++)
            {
                Genometype.ConnectionGenens connection = m_genome.connectionGenes[i];
                if (connection.enabled)
                {
                    float value;
                    switch (connection.operatorType)
                    {
                        case Genometype.ConnectionGenens.OperatorType.Multiply:
                            value = m_genome.nodeGenes[connection.inputNodeIndex].value * connection.weight;
                            break;
                        case Genometype.ConnectionGenens.OperatorType.Plus:
                            value = m_genome.nodeGenes[connection.inputNodeIndex].value + connection.weight;
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }

                    switch (m_genome.nodeGenes[connection.outputNodeIndex].addOnType)
                    {
                        case Genometype.NodeGenes.AddOnType.Plus:
                            m_genome.nodeGenes[connection.outputNodeIndex].value += value;
                            break;
                        case Genometype.NodeGenes.AddOnType.Multiply:
                            if (m_genome.nodeGenes[connection.outputNodeIndex].value == 0)
                                m_genome.nodeGenes[connection.outputNodeIndex].value = value;
                            else
                                m_genome.nodeGenes[connection.outputNodeIndex].value *= value;
                            break;
                    }
                }
            }
        }
        #endregion


        #region Output
        /// <summary>
        /// Get ouput node value by index
        /// </summary>
        /// <param name="outputIndex">The index of the output node</param>
        /// <returns>The value of output node (proccessed data)</returns>
        public float GetOutput(int outputIndex)
        {
            int outputIndexCount = 0;
            for (int i = 0; i  < m_genome.nodeGenes.Length;  i++)
            {
                if (m_genome.nodeGenes[i].type == Genometype.NodeGenes.Types.Output)
                {
                    if (outputIndex == outputIndexCount)
                    {
                        return m_genome.nodeGenes[i].value;
                    }
                    outputIndexCount++;
                }
            }

            throw new System.IndexOutOfRangeException("There's not enough output node");
        }
        #endregion
    }
}