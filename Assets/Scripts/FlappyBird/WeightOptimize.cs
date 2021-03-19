using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public class WeightOptimize
    {
        private int m_birdCount;

        /// <summary>
        /// Current batch of birds
        /// </summary>
        private GenomeControlBird[] m_birds;

        /// <summary>
        /// The result of current batch of birds
        /// </summary>
        private float[] m_birdResults;

        // private List<Genometype> m_historyGenomes;
        private Genometype m_currentGenome;

        private AbstractMLGameControl m_gameController;
        
        public bool AllDead;

        public WeightOptimize(AbstractMLGameControl gameController, int birdCount)
        {
            m_birdCount = birdCount;
            m_birds = new GenomeControlBird[birdCount];
            m_birdResults = new float[birdCount];

            m_gameController = gameController;
        }

        public void InsertGenome(Genometype genome, bool fullyRandom=true)
        {
            // m_historyGenomes.Add(genome);
            m_currentGenome = genome;

            PopulateByEvolveFromGenome(fullyRandom: fullyRandom);
        }

        public void PopulateByEvolveFromGenome(bool fullyRandom=false)
        {
            m_birds[0] = m_gameController.birdPool.Get();
            m_birds[0].Prepare(m_gameController, m_currentGenome);

            for (int i = 1; i < m_birdCount; i++)
            {
                m_birds[i] = m_gameController.birdPool.Get();
                m_birds[i].Prepare(m_gameController, ChangeWeightInGenome(m_currentGenome, fullyRandom: fullyRandom));
            }
        }

        private Genometype ChangeWeightInGenome(Genometype genome, bool fullyRandom=false)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                if (fullyRandom) newGenome.connectionGenes[i].weight = Random.Range(-10f, 10f);
                else newGenome.connectionGenes[i].weight += Random.Range(-0.6f, 0.6f);
            }

            return newGenome;
        }

        public void BirdOver(GenomeControlBird bird)
        {
            AllDead = true;

            for (int i = 0; i < m_birdCount; i++)
            {
                if (m_birds[i] == bird)
                {
                    m_birdResults[i] = Time.unscaledTime - m_gameController.gameStartTime;
                }

                if (m_birds[i].gameObject.activeSelf)
                {
                    AllDead = false;
                }
            }
        }

        public void FindBestData()
        {
            Genometype bestData = m_birds[0].GenomeData;
            float bestTime = 0;

            for (int i = 0; i < m_birdCount; i++)
            {
                if (m_birdResults[i] > bestTime)
                {
                    bestData = m_birds[i].GenomeData;
                    bestTime = m_birdResults[i];
                }
                // GameObject.Destroy(m_birds[i].gameObject);
            }

            // m_historyGenomes.Add(bestData);
            m_currentGenome = bestData;
        }


        public GenomeScore ExtractBestData()
        {
            GenomeScore genomeScore = new GenomeScore(m_birds[0].GenomeData, m_birdResults[0]);

            for (int i = 1; i < m_birdCount; i++)
            {
                if (m_birdResults[i] > genomeScore.time)
                {
                    genomeScore = new GenomeScore(m_birds[i].GenomeData, m_birdResults[i]);
                }
                // GameObject.Destroy(m_birds[i].gameObject);
            }

            return genomeScore;
        }

        public bool FindAliveData(out Genometype genome)
        {
            for (int i = 0; i < m_birds.Length; i++)
            {
                if (m_birds[i].gameObject.activeSelf)
                {
                    genome = m_birds[i].GenomeData;
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