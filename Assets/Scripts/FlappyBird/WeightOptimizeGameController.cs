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

        public int scoreRequire;
        public float maxnimumGenerationCount;
        private float m_generationCount;

        /// <summary>
        /// The minimum x of the ground become irrelevant
        /// </summary>
        public float x;

        public string genomeRecordFileName;
        public bool forShow;

        private void Start()
        {
            m_birds = new GenomeControlBird[batchBirdCount];
            m_birdResults = new float[batchBirdCount];

            startText.gameObject.SetActive(false);

            // Start new batch of birds
            if (genomeRecordFileName != "")
            {
                try {
                    Genometype genome = SavingSystem.GetGenome(genomeRecordFileName);

                    if (forShow)
                    {
                        // Only populate one bird
                        batchBirdCount = 1;
                    }

                    PopulateByEvolveFromGenome(SavingSystem.GetGenome(genomeRecordFileName));
                }
                catch (System.IO.FileNotFoundException)
                {
                    StartFromScratch();
                }
            }
            else StartFromScratch();
            

            m_batchStartTime = Time.unscaledTime;
            base.ResetGame();
        }

        protected void StartFromScratch()
        {
            for (int i = 0; i < batchBirdCount; i++)
            {
                m_birds[i] = Instantiate<GenomeControlBird>(birdPrefab);
                m_birds[i].transform.SetParent(birdsCollection);

                // Setup the genome
                Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 1),
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 5),
                    new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 7),
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

            m_generationCount++;
        }

        protected void PopulateByEvolveFromGenome(Genometype data)
        {
            m_birds[0] = Instantiate<GenomeControlBird>(birdPrefab);
            m_birds[0].Prepare(this, data);
            m_birds[0].transform.SetParent(birdsCollection);

            for (int i = 1; i < batchBirdCount; i++)
            {
                m_birds[i] = Instantiate<GenomeControlBird>(birdPrefab);
                m_birds[i].Prepare(this, ChangeWeightInGenome(data));
                m_birds[i].transform.SetParent(birdsCollection);
            }

            m_generationCount++;
        }

        protected override void Update()
        {
            UpdateGround();

            // Chose the closest ground
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

            // Check is the score reach
            if (score >= scoreRequire)
                StopTheTraining();
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

            PopulateByEvolveFromGenome(bestData);

            m_batchStartTime = Time.unscaledTime;

            // Check maximum generation is reach
            if (m_generationCount >= maxnimumGenerationCount)
                StopTheTraining();
        }

        private Genometype ChangeWeightInGenome(Genometype genome)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                newGenome.connectionGenes[i].weight += Random.Range(-1f, 1f);
            }

            return newGenome;
        }

        public void StopTheTraining()
        {
            for (int i = 0; i < m_birds.Length; i++)
            {
                if (m_birds[i].gameObject.activeSelf)
                {
                    SavingSystem.StoreGenome("result.data", m_birds[i].GenomeData);
                    Debug.Log(Application.persistentDataPath);
                    break;
                }
            }

            // Quit the game
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}