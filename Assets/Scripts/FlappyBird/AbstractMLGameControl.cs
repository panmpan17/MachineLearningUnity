using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace FlappyBird
{
    public abstract class AbstractMLGameControl : GameControl
    {
        /// <summary>
        /// The closest ground ahead of the bird
        /// </summary>
        [System.NonSerialized]
        public Transform cloestGround;

        public GameObject birdPrefab;
        public PrefabPool<GenomeControlBird> birdPool;

        /// <summary>
        /// Record of the time when new batch start
        /// </summary>
        [System.NonSerialized]
        public float gameStartTime;

        /// <summary>
        /// The minimum x of the ground become irrelevant
        /// </summary>
        public float groundRelevantMinmumX;

        public int scoreRequire;

        public abstract void BirdOver(GenomeControlBird bird);

        protected override void Awake() {
            birdPool = new PrefabPool<GenomeControlBird>(birdPrefab.GetComponent<GenomeControlBird>());
            Physics2D.gravity = new Vector2(0, -9.8f);
        }

        protected override void Update()
        {
            UpdateGround();

            // Chose the closest ground
            cloestGround = grounds[0].transform;
            float bestDistance = cloestGround.transform.position.x - groundRelevantMinmumX;
            for (int i = 0; i < grounds.Count; i++)
            {
                float distance = grounds[i].transform.position.x - groundRelevantMinmumX;
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

        public abstract void StopTraining(bool quitApplication=true);
    }
}