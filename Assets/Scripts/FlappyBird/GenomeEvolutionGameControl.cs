using System.Collections.Generic;
using UnityEngine;
using NEAT;
using TMPro;

namespace FlappyBird
{
    public class GenomeEvolutionGameControl: AbstractMLGameControl
    {
        [Header("Genome Evolution")]
        public int maxGenomeAtATime;

        public int genomeBirdCount;

        public int genomeMaxGeneration;

        private int m_weightGenerationCount;
        private int m_structureGenerationCount;

        // private List<Genometype> m_failedGenomes;
        private List<Genometype> m_aliveGenomes;
        private List<WeightOptimize> m_weightOptimizers;

        public TextMeshProUGUI weightGenerationText;
        public TextMeshProUGUI structureGenerationText;

        public string readFileName;
        public string saveFileName;

        private void Start()
        {
            m_aliveGenomes = new List<Genometype>();
            m_weightOptimizers = new List<WeightOptimize>();

            startText.gameObject.SetActive(false);

            if (SavingSystem.DataFileExist(saveFileName)) readFileName = saveFileName;

            if (readFileName != "")
            {
                GenomeStructEvolveData data;
                try
                {
                    data = SavingSystem.GetGenome<GenomeStructEvolveData>(readFileName, readFileName.EndsWith("json"));
                }
                catch (System.IO.FileNotFoundException) {
                    StartFromScratch();
                    gameStartTime = Time.unscaledTime;
                    base.ResetGame();
                    return;
                }

                m_aliveGenomes.AddRange(data.aliveBirds);
                m_structureGenerationCount = data.structureGenerationCount;

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

            gameStartTime = Time.unscaledTime;

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

        public override void BirdOver(GenomeControlBird bird)
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
                gameStartTime = Time.unscaledTime;
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

            gameStartTime = Time.unscaledTime;
            FindObjectOfType<ToggleRenderers>().ForceEnabled(true);
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
                    // if (bestScore.genome != null) m_failedGenomes.Add(bestScore.genome);
                    bestScore = score;
                }
                // else
                // {
                    // m_failedGenomes.Add(score.genome);
                // }
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

            if (possibleMutations.Count > maxGenomeAtATime - 1)
            {
                ShuffleList<Genometype>(possibleMutations);
                for (int i = 0; i < maxGenomeAtATime - 1; i++)
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

        public override void StopTraining()
        {
            SaveTraining();
            if (StageController.ins != null) StageController.ins?.StageTraningEnd();
            else
            {
                // Quit the game
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        }

        private void SaveTraining()
        {
            GenomeStructEvolveData storeData = new GenomeStructEvolveData();

            storeData.structureGenerationCount = m_structureGenerationCount;

            GenomeControlBird[] birds = FindObjectsOfType<GenomeControlBird>(false);
            storeData.aliveBirds = new Genometype[birds.Length];

            for (int i = 0; i < birds.Length; i++)
            {
                storeData.aliveBirds[i] = birds[i].GenomeData;
            }

            SavingSystem.SaveData<GenomeStructEvolveData>(saveFileName, storeData);
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