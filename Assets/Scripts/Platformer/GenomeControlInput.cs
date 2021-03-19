using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace Platformer
{
    public class GenomeControlInput : AbstractCharacterInput
    {
        private AbstractMachineLearningGameController m_gameController;

        private GenomeController genomeController;
        public Genometype GenomeData => genomeController.genome;

        public override bool Left => m_left;
        public override bool Right => m_right;
        public override bool Jump => m_jump;

        private bool m_left;
        private bool m_right;
        private bool m_jump;

        private void Update()
        {
            genomeController.Reset();
            genomeController.Input(new float[] {
                transform.position.x,
                transform.position.y,
            });
            genomeController.StartProcess();

            m_left = genomeController.GetOutput(0) > 0;
            m_right = genomeController.GetOutput(1) > 0;
            m_jump = genomeController.GetOutput(2) > 0;
        }

        public void InsertGenome(AbstractMachineLearningGameController gameController, Genometype genome)
        {
            m_gameController = gameController;
            genomeController = new GenomeController(genome);
        }
    }
}