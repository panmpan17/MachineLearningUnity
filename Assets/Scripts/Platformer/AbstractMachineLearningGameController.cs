using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public abstract class AbstractMachineLearningGameController : MonoBehaviour
    {
        public GameObject characterPrefab;
        public Vector3 characterSpawnPosition;
        public Transform characterCollection;

        [Range(10, 100)]
        public int instantCount = 20;

        public Transform endTransform;

        private int m_gernertaionCount;

        public abstract void CharacterReachEnd(CharacterController character);

        private void Awake() {
            Physics2D.gravity = new Vector2(0, -100f);
        }

        public float CalculateGenomeCharacterPoints(GenomeControlInput input)
        {
            Vector2 delta = input.transform.position - endTransform.position;

            return delta.sqrMagnitude;
        }
    }
}