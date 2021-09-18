using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using NEAT;

namespace FlappyBird
{
    public abstract class AbstractMLGameControl : GameControl, ITraningGameControl
    {
        /// <summary>
        /// The closest ground ahead of the bird
        /// </summary>
        [System.NonSerialized]
        public Transform cloestGround;

        public GameObject birdPrefab;
        public PrefabPool<GenomeControlBird> birdPool;

        /// <summary>
        /// The minimum x of the ground become irrelevant
        /// </summary>
        public float groundRelevantMinmumX;

        public int scoreRequire;

        public float GameStartTime => m_gameStartTime;
        protected float m_gameStartTime;

        public abstract void BirdOver(GenomeControlBird bird);

        protected override void Awake() {
            birdPool = new PrefabPool<GenomeControlBird>(birdPrefab.GetComponent<GenomeControlBird>());
            Physics2D.gravity = new Vector2(0, -9.8f);

            base.Awake();
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

        public abstract void StopTraining();

        public IGenomeAgent GetGenomeControlFromPool()
        {
            return birdPool.Get();
        }

        public void OnDisable()
        {
            for (int i = 0; i < groundPools.Length; i++)
            {
                groundPools[i].ClearAliveObjs();
                groundPools[i].ClearPoolObjs();
            }


            for (int i = 0; i < birdPool.AliveObjs.Count; i++)
            {
                if (birdPool.AliveObjs[i] != null)
                    Destroy(birdPool.AliveObjs[i].gameObject);
            }
            for (int i = 0; i < birdPool.PoolObjs.Count; i++)
            {
                if (birdPool.PoolObjs[i] != null)
                    Destroy(birdPool.PoolObjs[i].gameObject);
            }
        }
    }
}