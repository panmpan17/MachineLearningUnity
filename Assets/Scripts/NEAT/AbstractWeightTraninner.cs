using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    public abstract class AbstractTraninner
    {
        protected int m_instanceCount;

        protected IGenomeAgent[] m_instances;

        protected float[] m_results;

        protected ITraningGameControl m_gameControl;

        // protected Genometype m_currentGenome;

        protected List<Genometype> m_currentGenomes;
        protected List<GenomeWeightRecordCollection> m_weightRecordCollection;

        protected int m_surviveCount;

        public AbstractTraninner(ITraningGameControl gameControl, int instanceCount, int surviveCount)
        {
            m_gameControl = gameControl;

            m_instanceCount = instanceCount;
            m_instances = new IGenomeAgent[instanceCount];
            m_results = new float[instanceCount];

            m_currentGenomes = new List<Genometype>();
            m_weightRecordCollection = new List<GenomeWeightRecordCollection>();

            m_surviveCount = surviveCount;
        }

        public void PopulateByEvolveFromGenome(bool fullyRandom=false, float weightRange=1f)
        {
            int instanceIndex = 0;
            for (int i = 0; i < m_currentGenomes.Count && instanceIndex < m_instances.Length; i++)
            {
                m_instances[instanceIndex] = m_gameControl.GetGenomeControlFromPool();
                m_instances[instanceIndex].Prepare(m_gameControl, m_currentGenomes[i]);

                instanceIndex++;
            }


            for (int i = 0; i < m_currentGenomes.Count && instanceIndex < m_instances.Length; i++)
            {
                m_instances[instanceIndex] = m_gameControl.GetGenomeControlFromPool();
                m_instances[instanceIndex].Prepare(
                    m_gameControl,
                    ChangeWeightInGenome(m_currentGenomes[i], fullyRandom: fullyRandom, weightRange: weightRange));

                instanceIndex++;
                if (i == m_currentGenomes.Count - 1) i = -1;
            }

            for (int i = 0; i < m_weightRecordCollection.Count; i++)
            {
                var collection = m_weightRecordCollection[i];
                collection.reweightedTime += 1;
                m_weightRecordCollection[i] = collection;
            }
        }

        protected Genometype ChangeWeightInGenome(Genometype genome, bool fullyRandom=false, float weightRange=1f)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                if (fullyRandom) newGenome.connectionGenes[i].weight = Random.Range(-weightRange, weightRange);
                else newGenome.connectionGenes[i].weight += Random.Range(-weightRange, weightRange);
            }

            return newGenome;
        }

        protected void AddWeightRecord(Genometype genometype, float score)
        {
            bool genomtypeFound = false;
            for (int i = 0; i < m_weightRecordCollection.Count; i++)
            {
                if (Genometype.StructureIsSame(m_weightRecordCollection[i].genometype, genometype))
                {
                    m_weightRecordCollection[i].records.Add(new GenomeWeightRecord {
                        weights = ExtractWeights(genometype),
                        score = score,
                    });

                    genomtypeFound = true;
                    break;
                }
            }

            if (!genomtypeFound)
            {
                m_weightRecordCollection.Add(new GenomeWeightRecordCollection {
                    genometype = genometype,
                    reweightedTime = 0,
                    records = new List<GenomeWeightRecord>(new GenomeWeightRecord[] {
                        new GenomeWeightRecord
                        {
                            weights = ExtractWeights(genometype),
                            score = score,
                        }
                    })
                });
            }
        }

        protected static float[] ExtractWeights(Genometype genometype)
        {
            float[] weights = new float[genometype.connectionGenes.Length];

            for (int i = 0; i < genometype.connectionGenes.Length; i++)
            {
                weights[i] = genometype.connectionGenes[i].weight;
            }

            return weights;
        }

        public void SaveRecord(string folder)
        {
            SavingSystem.CreateFolder(folder);
            for (int i = 0; i < m_weightRecordCollection.Count; i++)
            {
                string path = folder + "/" + i + ".json";
                SavingSystem.SaveData<GenomeWeightRecordCollection>(path, m_weightRecordCollection[i], true);
            }
        }
    }

    [System.Serializable]
    public struct GenomeWeightRecordCollection
    {
        public Genometype genometype;
        public int reweightedTime;
        public List<GenomeWeightRecord> records;
    }

    [System.Serializable]
    public struct GenomeWeightRecord
    {
        public float[] weights;
        public float score;
    }
}