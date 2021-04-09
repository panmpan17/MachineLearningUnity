using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public class WeightOptimize: AbstractTraninner
    {   
        public bool AllDead;

        public WeightOptimize(ITraningGameControl gameControl, int instanceCount) : base(gameControl, instanceCount)
        {
            m_gameControl =  gameControl;
            m_instanceCount = instanceCount;
        }

        public void InsertGenome(Genometype genome, bool fullyRandom=true)
        {
            m_currentGenome = genome;

            PopulateByEvolveFromGenome(fullyRandom: fullyRandom);
        }

        public void BirdOver(IGenomeAgent bird)
        {
            AllDead = true;

            for (int i = 0; i < m_instanceCount; i++)
            {
                if (m_instances[i] == bird)
                {
                    m_results[i] = Time.unscaledTime - m_gameControl.GameStartTime;
                }

                if (m_instances[i].gameObject.activeSelf)
                {
                    AllDead = false;
                }
            }
        }

        public void FindBestData()
        {
            Genometype bestData = m_instances[0].GenomeData;
            float bestTime = 0;

            for (int i = 0; i < m_instanceCount; i++)
            {
                if (m_results[i] > bestTime)
                {
                    bestData = m_instances[i].GenomeData;
                    bestTime = m_results[i];
                }
            }

            m_currentGenome = bestData;
        }


        public GenomeScore ExtractBestData()
        {
            GenomeScore genomeScore = new GenomeScore(m_instances[0].GenomeData, m_results[0]);

            for (int i = 1; i < m_instanceCount; i++)
            {
                if (m_results[i] > genomeScore.time)
                {
                    genomeScore = new GenomeScore(m_instances[i].GenomeData, m_results[i]);
                }
            }

            return genomeScore;
        }

        public bool FindAliveData(out Genometype genome)
        {
            for (int i = 0; i < m_instances.Length; i++)
            {
                if (m_instances[i].gameObject.activeSelf)
                {
                    genome = m_instances[i].GenomeData;
                    return true;
                }
            }

            genome = new Genometype(null, null);
            return false;
        }

        public struct GenomeScore
        {
            public Genometype genome;
            public float time;

            public GenomeScore(Genometype _genome, float _time)
            {
                genome = _genome;
                time = _time;
            }
        }
    }
}