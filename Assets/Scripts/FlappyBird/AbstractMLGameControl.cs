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

        public abstract void BirdOver(GenomeControlBird bird);

        protected override void Awake() {
            birdPool = new PrefabPool<GenomeControlBird>(birdPrefab.GetComponent<GenomeControlBird>());
        }
    }
}