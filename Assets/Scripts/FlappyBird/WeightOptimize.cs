using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public class WeightOptimize: AbstractTraninner
    {   
        public bool AllDead;

        public WeightOptimize(ITraningGameControl gameControl, int instanceCount, int surviveCount) : base(gameControl, instanceCount, surviveCount)
        {
            //
        }

        public void InsertGenome(Genometype genome, bool fullyRandom=true)
        {
            m_currentGenomes.Add(genome);

            PopulateByEvolveFromGenome(fullyRandom: fullyRandom, weightRange: 10f);
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
            List<GenomeScore> bestScores = new List<GenomeScore>();

            for (int i = 0; i < m_instanceCount; i++)
            {
                GenomeScore score = new GenomeScore(m_instances[i].GenomeData, m_results[i]);

                if (bestScores.Count < m_surviveCount)
                {
                    bestScores.Add(score);
                }
                else
                {
                    for (int e = 0; e < bestScores.Count; e++)
                    {
                        if (score.score > bestScores[e].score)
                        {
                            bestScores.Insert(e, score);
                            bestScores.RemoveAt(bestScores.Count - 1);
                            break;
                        }
                    }
                }
            }

            m_currentGenomes.Clear();

            for (int i = 0; i < bestScores.Count; i++)
                m_currentGenomes.Add(bestScores[i].genome);
        }


        public GenomeScore ExtractBestData()
        {
            GenomeScore genomeScore = new GenomeScore(m_instances[0].GenomeData, m_results[0]);

            for (int i = 1; i < m_instanceCount; i++)
            {
                if (m_results[i] > genomeScore.score)
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
            public float score;

            public GenomeScore(Genometype _genome, float _score)
            {
                genome = _genome;
                score = _score;
            }
        }
    }
}