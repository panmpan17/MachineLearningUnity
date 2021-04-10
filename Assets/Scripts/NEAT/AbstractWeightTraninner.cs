using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    public abstract class AbstractTraninner
    {
        protected int m_instanceCount;

        protected IGenomeAgent[] m_instances;

        protected float[] m_results;

        protected ITraningGameControl m_gameControl;

        // protected Genometype m_currentGenome;

        protected List<Genometype> m_currentGenomes;

        protected int m_surviveCount;

        public AbstractTraninner(ITraningGameControl gameControl, int instanceCount, int surviveCount)
        {
            m_gameControl = gameControl;

            m_instanceCount = instanceCount;
            m_instances = new IGenomeAgent[instanceCount];
            m_results = new float[instanceCount];

            m_currentGenomes = new List<Genometype>();
            m_surviveCount = surviveCount;
        }

        public void PopulateByEvolveFromGenome(bool fullyRandom=false, float weightRange=1f)
        {
            int instanceIndex = 0;
            for (int i = 0; i < m_currentGenomes.Count && instanceIndex < m_instances.Length; i++)
            {
                m_instances[instanceIndex] = m_gameControl.GetGenomeControlFromPool();
                m_instances[instanceIndex].Prepare(m_gameControl, m_currentGenomes[i]);

                instanceIndex++;
            }


            for (int i = 0; i < m_currentGenomes.Count && instanceIndex < m_instances.Length; i++)
            {
                m_instances[instanceIndex] = m_gameControl.GetGenomeControlFromPool();
                m_instances[instanceIndex].Prepare(
                    m_gameControl,
                    ChangeWeightInGenome(m_currentGenomes[i], fullyRandom: fullyRandom, weightRange: weightRange));

                instanceIndex++;
                if (i == m_currentGenomes.Count - 1) i = -1;
            }
        }

        protected Genometype ChangeWeightInGenome(Genometype genome, bool fullyRandom=false, float weightRange=1f)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                if (fullyRandom) newGenome.connectionGenes[i].weight = Random.Range(-weightRange, weightRange);
                else newGenome.connectionGenes[i].weight += Random.Range(-weightRange, weightRange);
            }

            return newGenome;
        }
    }
}