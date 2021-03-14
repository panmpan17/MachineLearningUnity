using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public interface IBirdOverCallback
    {
        Transform CloestGround { get; }

        void BirdOver(GenomeControlBird bird);
    }

    public class WeightOptimizeGameController : GameControl, IBirdOverCallback
    {
        [Header("Machine Learning")]
        public Transform birdsCollection;
        public GenomeControlBird birdPrefab;
        public int batchBirdCount;

        /// <summary>
        /// Current batch of birds
        /// </summary>
        private GenomeControlBird[] m_birds;

        /// <summary>
        /// The result of current batch of birds
        /// </summary>
        private float[] m_birdResults;

        /// <summary>
        /// The closest ground ahead of the bird
        /// </summary>
        private Transform m_cloestGround;
        public Transform CloestGround { get { return m_cloestGround; } }

        /// <summary>
        /// Record of the time when new batch start
        /// </summary>
        private float m_batchStartTime;

        /// <summary>
        /// The minimum x of the ground become irrelevant
        /// </summary>
        public float x;

        private void Start()
        {
            startText.gameObject.SetActive(false);

            // Start new batch of birds
            m_birds = new GenomeControlBird[batchBirdCount];
            m_birdResults = new float[batchBirdCount];

            for (int i = 0; i < batchBirdCount; i++)
            {
                m_birds[i] = Instantiate<GenomeControlBird>(birdPrefab);
                m_birds[i].transform.SetParent(birdsCollection);

                // Setup the genome
                Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input),
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input),
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input),
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Hidden),
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output),
                };

                Genometype.ConnectionGenens[] connections = new Genometype.ConnectionGenens[] {
                    new Genometype.ConnectionGenens(0, 3, Random.Range(-10f, 10f)),
                    new Genometype.ConnectionGenens(1, 3, Random.Range(-10f, 10f)),
                    new Genometype.ConnectionGenens(2, 3, Random.Range(-10f, 10f)),
                    new Genometype.ConnectionGenens(3, 4, Random.Range(-10f, 10f), Genometype.ConnectionGenens.OperatorType.Plus),
                };

                m_birds[i].Prepare(this, new Genometype(nodes, connections));
            }

            m_batchStartTime = Time.unscaledTime;
            base.ResetGame();
        }

        protected override void Update()
        {
            UpdateGround();

            // Chose the close ground
            m_cloestGround = grounds[0].transform;
            float bestDistance = m_cloestGround.transform.position.x - x;
            for (int i = 0; i < grounds.Count; i++)
            {
                float distance = grounds[i].transform.position.x - x;
                if (bestDistance < 0 || (distance < bestDistance && distance > 0))
                {
                    bestDistance = distance;
                    m_cloestGround = grounds[i].transform;
                }
            }
        }

        /// <summary>
        /// Callback from bird
        /// </summary>
        /// <param name="bird"></param>
        public void BirdOver(GenomeControlBird bird)
        {
            bool allDead = true;
            for (int i = 0; i < batchBirdCount; i++)
            {
                if (m_birds[i] == bird)
                {
                    m_birdResults[i] = Time.unscaledTime - m_batchStartTime;
                }

                if (m_birds[i].gameObject.activeSelf) allDead = false;
            }

            if (allDead)
            {
                ResetGame();
            }
        }

        public override void ResetGame()
        {
            base.ResetGame();

            // Prepare new batch
            // Find best data from current batch of birds
            Genometype bestData = m_birds[0].GenomeData;
            float bestTime = 0;

            for (int i = 0; i < batchBirdCount; i++)
            {
                if (m_birdResults[i] > bestTime)
                {
                    bestData = m_birds[i].GenomeData;
                    bestTime = m_birdResults[i];
                }
                Destroy(m_birds[i].gameObject);
            }

            // if (m_generationCount % 5 == 0)
            //     SavingSystem.SaveData(bestData, string.Format("generation-{0}.json", m_generationCount));

            // Preserve the best of last batch to the next batch
            m_birds[0] = Instantiate<GenomeControlBird>(birdPrefab);
            m_birds[0].Prepare(this, bestData);
            m_birds[0].transform.SetParent(birdsCollection);

            for (int i = 1; i < batchBirdCount; i++)
            {
                m_birds[i] = Instantiate<GenomeControlBird>(birdPrefab);
                m_birds[i].Prepare(this, EvolveGenome(bestData));
                // Debug.Log(m_birds[i].GenomeData);
                m_birds[i].transform.SetParent(birdsCollection);
            }

            m_batchStartTime = Time.unscaledTime;
        }

        private Genometype EvolveGenome(Genometype genome)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                newGenome.connectionGenes[i].weight += Random.Range(-1f, 1f);
            }

            return newGenome;
        }
    }
}