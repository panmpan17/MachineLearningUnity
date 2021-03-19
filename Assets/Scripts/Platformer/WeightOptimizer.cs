using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace Platformer
{
    public class WeightOptimizer
    {
        private AbstractMachineLearningGameController m_gameController;

        private int instanceCount => m_gameController.instantCount;

        private GenomeControlInput[] m_inputs;

        private Genometype m_currentGenome;

        public bool finished;

        public WeightOptimizer(AbstractMachineLearningGameController gameController)
        {
            m_gameController = gameController;

            m_inputs = new GenomeControlInput[instanceCount];
        }

        public void InsertGenome(Genometype genome, bool fullyRandom=true)
        {
            m_currentGenome = genome;
            Repopulate(fullyRandom: fullyRandom);
        }

        public void Repopulate(bool includeOrigin=true, bool fullyRandom=false)
        {
            finished = false;

            if (includeOrigin)
            {
                m_inputs[0] = GameObject.Instantiate(m_gameController.characterPrefab).GetComponent<GenomeControlInput>();
                m_inputs[0].InsertGenome(m_gameController, m_currentGenome);
                m_inputs[0].transform.SetParent(m_gameController.characterCollection);
                m_inputs[0].transform.position = m_gameController.characterSpawnPosition;
            }

            for (int i = includeOrigin? 1: 0; i < instanceCount; i++)
            {
                m_inputs[i] = GameObject.Instantiate(m_gameController.characterPrefab).GetComponent<GenomeControlInput>();
                m_inputs[i].InsertGenome(m_gameController, TweakWeightInGenome(m_currentGenome, fullyRandom: fullyRandom));
                m_inputs[i].transform.SetParent(m_gameController.characterCollection);
                m_inputs[i].transform.position = m_gameController.characterSpawnPosition;
            }
        }

        private Genometype TweakWeightInGenome(Genometype genome, bool fullyRandom=false)
        {
            Genometype newGenome = genome.Clone();

            for (int i = 0; i < newGenome.connectionGenes.Length; i++)
            {
                if (fullyRandom) newGenome.connectionGenes[i].weight = Random.Range(-10f, 10f);
                else newGenome.connectionGenes[i].weight += Random.Range(-1f, 1f);
            }

            // Debug.Log(newGenome);

            return newGenome;
        }

        public void FindBestData()
        {
            Genometype bestData = m_inputs[0].GenomeData;
            float bestScore = m_gameController.CalculateGenomeCharacterPoints(m_inputs[0]);

            for (int i = 1; i < instanceCount; i++)
            {
                float score = m_gameController.CalculateGenomeCharacterPoints(m_inputs[i]);
            
                if (score < bestScore)
                {
                    bestData = m_inputs[i].GenomeData;
                    bestScore = score;
                }

                GameObject.Destroy(m_inputs[i].gameObject);
            }
            GameObject.Destroy(m_inputs[0].gameObject);

            m_currentGenome = bestData;
        }

        public bool CharacterReachEnd(CharacterController character)
        {
            if (finished) return false;

            GenomeControlInput input = character.GetComponent<GenomeControlInput>();

            bool matched = false;
            for (int i = 0; i < instanceCount; i++)
            {
                if (input == m_inputs[i])
                {
                    matched = true;
                    break;
                }
            }

            if (!matched) return false;

            m_currentGenome = input.GenomeData;

            for (int i = 0; i < instanceCount; i++)
            {
                GameObject.Destroy(m_inputs[i].gameObject);
            }

            finished = true;

            return true;
        }
    }
}