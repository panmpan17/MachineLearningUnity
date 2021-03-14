using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;


namespace FlappyBird
{
    public class GenomeControlBird : BirdContoller
    {
        private IBirdOverCallback callbackController;
        private GenomeController genomeController;

        public Genometype GenomeData
        {
            get { return genomeController.genome; }
        }

        public void Prepare(IBirdOverCallback _callbackController, Genometype genome)
        {
            callbackController = _callbackController;
            genomeController = new GenomeController(genome);
        }

        void Update()
        {
            Transform upGround = callbackController.CloestGround.GetChild(0);
            Transform downGround = callbackController.CloestGround.GetChild(1);

            // Insert the inputs
            genomeController.Reset();
            genomeController.Input(new float[] {
                transform.position.y,
                upGround.position.y,
                downGround.position.y,
            });

            genomeController.StartProcess();

            // Process the outputs
            if (genomeController.GetOutput(0) > 0)
                Jump();
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            // Callback to the controller, let it know this bird hit ground
            gameObject.SetActive(false);
            callbackController?.BirdOver(this);
        }
    }
}