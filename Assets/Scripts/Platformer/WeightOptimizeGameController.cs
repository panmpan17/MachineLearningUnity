using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;
using MPack;


namespace Platformer
{
    public class WeightOptimizeGameController : AbstractMachineLearningGameController
    {
        private WeightOptimizer m_weightOptimizer;

        public Timer waitTimer;

        private void Start() {
            m_weightOptimizer = new WeightOptimizer(this);
            StartFromScratch();
        }

        private void Update()
        {
            if (waitTimer.UpdateEnd)
            {
                waitTimer.Reset();

                m_weightOptimizer.FindBestData();
                m_weightOptimizer.Repopulate();
            }
        }

        private void StartFromScratch()
        {
            Genometype.NodeGenes[] nodes = new Genometype.NodeGenes[] {
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 0),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Input, _IOIndex: 1),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output, _IOIndex: 0),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output, _IOIndex: 1),
                new Genometype.NodeGenes(Genometype.NodeGenes.Types.Output, _IOIndex: 2),
            };

            Genometype.ConnectionGenens[] connections = new Genometype.ConnectionGenens[] {
                new Genometype.ConnectionGenens(0, 2, Random.Range(-10f, 10f), _operatorType: Genometype.ConnectionGenens.OperatorType.Multiply),
                new Genometype.ConnectionGenens(0, 3, Random.Range(-10f, 10f), _operatorType: Genometype.ConnectionGenens.OperatorType.Multiply),
                new Genometype.ConnectionGenens(1, 4, Random.Range(-10f, 10f), _operatorType: Genometype.ConnectionGenens.OperatorType.Plus),
            };

            m_weightOptimizer.InsertGenome(new Genometype(nodes, connections));
        }

        public override void CharacterReachEnd(CharacterController character)
        {
            m_weightOptimizer.CharacterReachEnd(character);
            // m_weightOptimizer.Repopulate();
        }

        private void OnDrawGizmos() {
            Gizmos.color = new Color(0.1f, 0.3f, 0.6f, 0.4f);
            Gizmos.DrawCube(characterSpawnPosition, Vector3.one);
        }
    }
}