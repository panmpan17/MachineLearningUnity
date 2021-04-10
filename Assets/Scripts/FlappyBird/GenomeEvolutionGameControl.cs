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

        public int genomeSurviveCount;

        public int genomeBirdCount;

        public int genomeBirdSurviveCount;

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
                    m_gameStartTime = Time.unscaledTime;
                    base.ResetGame();
                    return;
                }

                m_aliveGenomes.AddRange(data.aliveBirds);
                m_structureGenerationCount = data.structureGenerationCount;

                for (int i = 0; i < m_aliveGenomes.Count; i++)
                {
                    m_weightOptimizers.Add(new WeightOptimize(this, genomeBirdCount, genomeBirdSurviveCount));
                    m_weightOptimizers[i].InsertGenome(m_aliveGenomes[i], fullyRandom: false);
                }
            }
            else
            {
                StartFromScratch();
            }

            m_gameStartTime = Time.unscaledTime;

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
                m_weightOptimizers.Add(new WeightOptimize(this, genomeBirdCount, genomeBirdSurviveCount));
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
                m_gameStartTime = Time.unscaledTime;
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
                    m_weightOptimizers[i].PopulateByEvolveFromGenome(weightRange: 0.6f);
                }

                m_weightGenerationCount++;
                weightGenerationText.text = m_weightGenerationCount.ToString();
            }

            m_gameStartTime = Time.unscaledTime;
            FindObjectOfType<ToggleRenderers>().ForceEnabled(true);
        }

        public void DoGenomeMutation()
        {
            List<WeightOptimize.GenomeScore> bestScores = new List<WeightOptimize.GenomeScore>();
            // WeightOptimize.GenomeScore bestScore = new WeightOptimize.GenomeScore(null, 0);

            // Find the best 3 genome from the weight optimizers
            for (int i = 0; i < m_weightOptimizers.Count; i++)
            {
                WeightOptimize.GenomeScore score = m_weightOptimizers[i].ExtractBestData();

                if (bestScores.Count < genomeSurviveCount)
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


            List<Genometype> surviveGenomes = new List<Genometype>();
            for (int i = 0; i < bestScores.Count; i++)
                surviveGenomes.Add(bestScores[i].genome);

            m_aliveGenomes.Clear();
            m_weightOptimizers.Clear();

            m_aliveGenomes.AddRange(surviveGenomes);

            List<Genometype> possibleMutations = new List<Genometype>();

            int genomePerGenome = (maxGenomeAtATime - m_aliveGenomes.Count) / m_aliveGenomes.Count;

            for (int i = 0; i < surviveGenomes.Count; i++)
            {
                GenomeMutationController mutateController = new GenomeMutationController(surviveGenomes[i]);
                mutateController.MutateByAddingConnection();
                mutateController.MutateByInsertNodeBetweenConnection();

                if (mutateController.mutations.Count > genomePerGenome)
                {
                    ShuffleList<Genometype>(mutateController.mutations);
                    for (int e = 0; e < genomePerGenome; e++)
                        m_aliveGenomes.Add(mutateController.mutations[e]);
                }
                else
                {
                    m_aliveGenomes.AddRange(mutateController.mutations);
                }
            }

            for (int i = 0; i < m_aliveGenomes.Count; i++)
            {
                m_weightOptimizers.Add(new WeightOptimize(this, genomeBirdCount, genomeBirdSurviveCount));
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