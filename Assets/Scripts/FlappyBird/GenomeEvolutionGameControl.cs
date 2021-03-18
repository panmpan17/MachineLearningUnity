using System.Collections.Generic;
using UnityEngine;
using NEAT;
using TMPro;

namespace FlappyBird
{
    public class GenomeEvolutionGameControl: GameControl, IBirdOverCallback
    {
        [Header("Genome Evolution")]
        public Transform birdsCollection;

        public GenomeControlBird birdPrefab;
        public GenomeControlBird BirdPrefab => birdPrefab;

        public int maxGenomeAtATime;

        public int genomeBirdCount;

        public int genomeMaxGeneration;

        private int m_weightGenerationCount;
        private int m_structureGenerationCount;

        public int scoreRequire;

        private List<Genometype> m_failedGenomes;
        // private List<Genometype> 
        private List<Genometype> m_aliveGenomes;
        private List<WeightOptimize> m_weightOptimizers;

        /// <summary>
        /// Record of the time when new batch start
        /// </summary>
        private float m_batchStartTime;
        public float GameStartTime => m_batchStartTime;

        /// <summary>
        /// The closest ground ahead of the bird
        /// </summary>
        private Transform m_cloestGround;
        public Transform CloestGround  => m_cloestGround;


        /// <summary>
        /// The minimum x of the ground become irrelevant
        /// </summary>
        public float x;

        public TextMeshProUGUI weightGenerationText;
        public TextMeshProUGUI structureGenerationText;

        public string recordFileName;

        private void Start()
        {
            m_failedGenomes = new  List<Genometype>();
            m_aliveGenomes = new List<Genometype>();
            m_weightOptimizers = new List<WeightOptimize>();

            startText.gameObject.SetActive(false);

            if (recordFileName != "")
            {
                GenomeStructEvolveData data;
                try
                {
                    data = SavingSystem.GetGenome<GenomeStructEvolveData>(recordFileName, recordFileName.EndsWith("json"));
                }
                catch (System.IO.FileNotFoundException)
                {
                    Application.Quit();
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    return;
                }

                m_failedGenomes.AddRange(data.failedGenomes);
                m_failedGenomes.AddRange(data.aliveGenomes);
                m_aliveGenomes.AddRange(data.aliveBirds);
                m_structureGenerationCount = data.structureGenerationCount;

                // for (int i = 0)
                for (int i = 0; i < m_aliveGenomes.Count; i++)
                {
                    m_weightOptimizers.Add(new WeightOptimize(this, genomeBirdCount));
                    m_weightOptimizers[i].InsertGenome(m_aliveGenomes[i], fullyRandom: false);
                }
            }
            else
            {
                StartFromScratch();
            }

            m_batchStartTime = Time.unscaledTime;

            base.ResetGame();
        }

        protected void StartFromScratch()
        {
            // Add basic genome input and output into alive genomes;
            Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 1),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 2),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 3),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 4),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 5),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 6),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 7),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output, _IOIndex: 0),
            };

            Genometype genome = new Genometype(nodes);

            GenomeMutationController mutationController = new GenomeMutationController(genome);
            mutationController.MutateByAddingConnection();
            m_aliveGenomes.AddRange(mutationController.mutations);

            for (int i = 0; i < m_aliveGenomes.Count; i++)
            {
                m_weightOptimizers.Add(new WeightOptimize(this, genomeBirdCount));
                m_weightOptimizers[i].InsertGenome(m_aliveGenomes[i]);
            }
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
                StopTraining();
        }

        public void BirdOver(GenomeControlBird bird)
        {
            bool allOver = true;
            for (int i = 0; i < m_weightOptimizers.Count; i++)
            {
                m_weightOptimizers[i].BirdOver(bird);
                if (!m_weightOptimizers[i].AllDead)
                    allOver = false;
            }

            if (allOver)
            {
                m_batchStartTime = Time.unscaledTime;
                ResetGame();
            }
        }

        public override void ResetGame()
        {
            base.ResetGame();

            if (m_weightGenerationCount >= genomeMaxGeneration)
            {
                DoGenomeMutation();
                m_weightGenerationCount = 0;
                m_structureGenerationCount++;

                structureGenerationText.text = m_structureGenerationCount.ToString();
                weightGenerationText.text = m_weightGenerationCount.ToString();
            }
            else
            {
                // Prepare new batch
                for (int i = 0; i < m_weightOptimizers.Count; i++)
                {
                    m_weightOptimizers[i].FindBestData();
                    m_weightOptimizers[i].PopulateByEvolveFromGenome();
                }

                m_weightGenerationCount++;
                weightGenerationText.text = m_weightGenerationCount.ToString();
            }

            m_batchStartTime = Time.unscaledTime;            
        }

        public void DoGenomeMutation()
        {
            // List<WeightOptimize.GenomeScore> bestScores = new List<WeightOptimize.GenomeScore>();
            WeightOptimize.GenomeScore bestScore = new WeightOptimize.GenomeScore(null, 0);

            // Find the best 3 genome from the weight optimizers
            for (int i = 0; i < m_weightOptimizers.Count; i++)
            {
                WeightOptimize.GenomeScore score = m_weightOptimizers[i].ExtractBestData();
                if (score.time > bestScore.time)
                {
                    if (bestScore.genome != null) m_failedGenomes.Add(bestScore.genome);
                    bestScore = score;
                }
                else
                {
                    m_failedGenomes.Add(score.genome);
                }
            }

            m_aliveGenomes.Clear();
            m_weightOptimizers.Clear();

            m_aliveGenomes.Add(bestScore.genome);

            GenomeMutationController mutateController = new GenomeMutationController(bestScore.genome);
            mutateController.MutateByAddingConnection();
            mutateController.MutateByInsertNodeBetweenConnection();

            List<Genometype> possibleMutations = new List<Genometype>();
            for (int i = 0; i < mutateController.mutations.Count; i++)
            {
                possibleMutations.Add(mutateController.mutations[i]);
            }

            if (possibleMutations.Count > maxGenomeAtATime - 3)
            {
                ShuffleList<Genometype>(possibleMutations);
                for (int i = 0; i < maxGenomeAtATime - 3; i++)
                {
                    m_aliveGenomes.Add(possibleMutations[i]);
                }
            }
            else
            {
                m_aliveGenomes.AddRange(possibleMutations);
            }

            for (int i = 0; i < m_aliveGenomes.Count; i++)
            {
                m_weightOptimizers.Add(new WeightOptimize(this, genomeBirdCount));
                m_weightOptimizers[i].InsertGenome(m_aliveGenomes[i], fullyRandom: false);
            }
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public void StopTraining(bool stopApplication=true)
        {
            GenomeStructEvolveData storeData = new GenomeStructEvolveData();

            storeData.structureGenerationCount = m_structureGenerationCount;
            storeData.failedGenomes = m_failedGenomes.ToArray();
            storeData.aliveGenomes = m_aliveGenomes.ToArray();

            GenomeControlBird[] birds = FindObjectsOfType<GenomeControlBird>(false);
            storeData.aliveBirds = new Genometype[birds.Length];

            for (int i = 0; i < birds.Length; i++)
            {
                storeData.aliveBirds[i] = birds[i].GenomeData;
            }

            SavingSystem.SaveData<GenomeStructEvolveData>("compound-data", storeData);

            if (stopApplication)
            {
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        }

        [System.Serializable]
        public struct GenomeStructEvolveData
        {
            public int structureGenerationCount;
            public Genometype[] failedGenomes;
            public Genometype[] aliveGenomes;
            public Genometype[] aliveBirds;
        }
    }
}