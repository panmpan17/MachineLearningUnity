using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public interface IBirdOverCallback
    {
        Transform CloestGround { get; }
        GenomeControlBird BirdPrefab { get; }
        float GameStartTime { get; }

        void BirdOver(GenomeControlBird bird);
    }

    public class WeightOptimizeGameController : GameControl, IBirdOverCallback
    {
        [Header("Machine Learning")]
        public Transform birdsCollection;
        public GenomeControlBird birdPrefab;
        public GenomeControlBird BirdPrefab {
            get { return birdPrefab; }
        }

        public int batchBirdCount;

        private WeightOptimize m_weightOptimizer;

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

        public float GameStartTime {
            get { return m_batchStartTime; }
        }

        private void Start()
        {
            startText.gameObject.SetActive(false);

            // Only populate one bird if for show is on
            m_weightOptimizer = new WeightOptimize(this, forShow? 1: batchBirdCount);


            // Start new batch of birds, if record file can be load use record genome
            try {
                m_weightOptimizer.InsertGenome(SavingSystem.GetGenome(genomeRecordFileName));
            }
            catch (System.IO.FileNotFoundException)
            {
                StartFromScratch();
            }
            
            m_batchStartTime = Time.unscaledTime;
            base.ResetGame();
        }

        protected void StartFromScratch()
        {
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

            m_weightOptimizer.InsertGenome(new Genometype(nodes, connections));
        }

        protected void PopulateByEvolveFromGenome(Genometype data)
        {
            m_weightOptimizer.PopulateByEvolveFromGenome();
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
            if (m_weightOptimizer.BirdOver(bird))
                ResetGame();
        }

        public override void ResetGame()
        {
            base.ResetGame();

            // Prepare new batch
            m_weightOptimizer.FindBestData();
            m_weightOptimizer.PopulateByEvolveFromGenome();

            m_batchStartTime = Time.unscaledTime;

            // Check maximum generation is reach
            if (m_generationCount >= maxnimumGenerationCount)
                StopTheTraining();
        }

        public void StopTheTraining()
        {
            Genometype genome;
            if (m_weightOptimizer.FindAliveData(out genome))
            {
                SavingSystem.StoreGenome("best-data", genome);
            }

            // Quit the game
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}