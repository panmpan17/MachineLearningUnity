using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;
using MPack;


namespace FlappyBird
{
    public class GenomeControlBird : BirdContoller, IPoolableObj, IGenomeAgent
    {
        private AbstractMLGameControl m_gameController;
        private GenomeController genomeController;

        public Genometype GenomeData
        {
            get { return genomeController.genome; }
        }

        public void Prepare(ITraningGameControl gameControl, Genometype genome)
        {
            m_gameController = (AbstractMLGameControl) gameControl;
            genomeController = new GenomeController(genome);
        }

        protected override void OnEnable()
        {
            transform.position = initialPosition;
            rigidbody2D.simulated = true;
            // rigidbody2D.velocity = new Vector2(0, jumpVelocity);
            rigidbody2D.angularVelocity = downwardAngularSpeed;
            transform.rotation = Quaternion.Euler(0, 0, upwardStopAngle);
        }

        void Update()
        {
            if (m_gameController == null) return;

            Transform upGround = m_gameController.cloestGround.GetChild(0);
            Transform downGround = m_gameController.cloestGround.GetChild(1);

            // Insert the inputs
            genomeController.Reset();
            genomeController.Input(new float[] {
                transform.position.y,
                rigidbody2D.velocity.y,
                upGround.position.x,
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
            m_gameController.birdPool.Put(this);
            m_gameController?.BirdOver(this);
        }

        public void Instantiate()
        {
            OnEnable();
        }

        public void DeactivateObj(Transform collectionTransform)
        {
            gameObject.SetActive(false);
        }

        public void Reinstantiate()
        {
            gameObject.SetActive(true);
            OnEnable();
        }
    }
}