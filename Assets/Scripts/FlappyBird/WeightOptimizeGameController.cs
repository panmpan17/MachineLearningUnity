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

    public class WeightOptimizeGameController : AbstractMLGameControl
    {
        [Header("Machine Learning")]
        public int batchBirdCount;

        private WeightOptimize m_weightOptimizer;

        /// <summary>
        /// Record of the time when new batch start
        /// </summary>
        private float m_batchStartTime;
        public float GameStartTime
        {
            get { return m_batchStartTime; }
        }

        public int scoreRequire;
        public int maxnimumGenerationCount;
        private int m_generationCount;

        /// <summary>
        /// The minimum x of the ground become irrelevant
        /// </summary>
        public float x;

        public string genomeRecordFileName;
        public bool forShow;

        private void Start()
        {
            Physics2D.gravity = new Vector2(0, -9.8f);

            startText.gameObject.SetActive(false);

            // Only populate one bird if for show is on
            m_weightOptimizer = new WeightOptimize(this, forShow? 1: batchBirdCount);


            // Start new batch of birds, if record file can be load use record genome
            if (genomeRecordFileName != "")
            {
                try {
                    m_weightOptimizer.InsertGenome(SavingSystem.GetGenome<Genometype>(genomeRecordFileName));
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

        protected override void Update()
        {
            UpdateGround();

            // Chose the closest ground
            cloestGround = grounds[0].transform;
            float bestDistance = cloestGround.transform.position.x - x;
            for (int i = 0; i < grounds.Count; i++)
            {
                float distance = grounds[i].transform.position.x - x;
                if (bestDistance < 0 || (distance < bestDistance && distance > 0))
                {
                    bestDistance = distance;
                    cloestGround = grounds[i].transform;
                }
            }

            // Check is the score reach
            if (score >= scoreRequire)
                StopTraining();
        }

        /// <summary>
        /// Callback from bird
        /// </summary>
        /// <param name="bird"></param>
        public override void BirdOver(GenomeControlBird bird)
        {
            m_weightOptimizer.BirdOver(bird);
            if (m_weightOptimizer.AllDead)
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
                StopTraining();
        }

        public void StopTraining()
        {
            Genometype genome;
            if (m_weightOptimizer.FindAliveData(out genome))
            {
                SavingSystem.SaveData<Genometype>("best-data", genome);
            }

            // Quit the game
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}