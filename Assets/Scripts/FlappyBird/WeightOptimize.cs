using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public class WeightOptimize
    {
        [SerializeField]
        private int m_birdCount;

        /// <summary>
        /// Current batch of birds
        /// </summary>
        private GenomeControlBird[] m_birds;

        /// <summary>
        /// The result of current batch of birds
        /// </summary>
        private float[] m_birdResults;

        private List<Genometype> m_historyGenomes;

        private IBirdOverCallback gameController;
        
        public bool AllDead;

        public WeightOptimize(IBirdOverCallback _gameController, int birdCount)
        {
            m_birdCount = birdCount;
            m_birds = new GenomeControlBird[birdCount];
            m_birdResults = new float[birdCount];
            m_historyGenomes = new List<Genometype>();

            gameController = _gameController;
        }

        public void InsertGenome(Genometype genome, bool fullyRandom=true)
        {
            m_historyGenomes.Add(genome);

            m_birds[0] = GameObject.Instantiate<GenomeControlBird>(gameController.BirdPrefab);
            // m_birds[i].transform.SetPraent(birdsCollection);
            m_birds[0].Prepare(gameController, genome);

            for (int i = 1; i < m_birdCount; i++)
            {
                m_birds[i] = GameObject.Instantiate<GenomeControlBird>(gameController.BirdPrefab);
                // m_birds[i].transform.SetPraent(birdsCollection);
                m_birds[i].Prepare(gameController, ChangeWeightInGenome(genome, fullyRandom: fullyRandom));
            }
        }

        public void PopulateByEvolveFromGenome()
        {
            Genometype data = m_historyGenomes[m_historyGenomes.Count - 1];

            m_birds[0] = GameObject.Instantiate<GenomeControlBird>(gameController.BirdPrefab);
            m_birds[0].Prepare(gameController, data);
            // m_birds[0].transform.SetParent(.birdsCollection);

            for (int i = 1; i < m_birdCount; i++)
            {
                m_birds[i] = GameObject.Instantiate<GenomeControlBird>(gameController.BirdPrefab);
                m_birds[i].Prepare(gameController, ChangeWeightInGenome(data));
                // m_birds[i].transform.SetParent(birdsCollection);
            }
        }

        private Genometype ChangeWeightInGenome(Genometype genome, bool fullyRandom=false)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                if (fullyRandom) newGenome.connectionGenes[i].weight = Random.Range(-10f, 10f);
                else newGenome.connectionGenes[i].weight += Random.Range(-1f, 1f);
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
                    m_birdResults[i] = Time.unscaledTime - gameController.GameStartTime;
                }

                if (m_birds[i].gameObject.activeSelf) AllDead = false;
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
                GameObject.Destroy(m_birds[i].gameObject);
            }

            m_historyGenomes.Add(bestData);
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
                GameObject.Destroy(m_birds[i].gameObject);
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