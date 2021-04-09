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

        protected Genometype m_currentGenome;

        public AbstractTraninner(ITraningGameControl gameControl, int instanceCount)
        {
            m_gameControl = gameControl;

            m_instanceCount = instanceCount;
            m_instances = new IGenomeAgent[instanceCount];
            m_results = new float[instanceCount];
        }

        public void PopulateByEvolveFromGenome(bool fullyRandom=false)
        {
            m_instances[0] = m_gameControl.GetGenomeControlFromPool();
            m_instances[0].Prepare(m_gameControl, m_currentGenome);

            for (int i = 1; i < m_instanceCount; i++)
            {
                m_instances[i] = m_gameControl.GetGenomeControlFromPool();
                m_instances[i].Prepare(m_gameControl, ChangeWeightInGenome(m_currentGenome, fullyRandom: fullyRandom));
            }
        }

        protected Genometype ChangeWeightInGenome(Genometype genome, bool fullyRandom = false)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                if (fullyRandom) newGenome.connectionGenes[i].weight = Random.Range(-10f, 10f);
                else newGenome.connectionGenes[i].weight += Random.Range(-0.6f, 0.6f);
            }

            return newGenome;
        }
    }
}