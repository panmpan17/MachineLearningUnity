﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MPack;


namespace FlappyBird
{
    public class GameControl : MonoBehaviour
    {
        [SerializeField]
        protected BirdContoller birdContoller;

        [SerializeField]
        protected float scoreX;

        [Header("Ground control")]
        [SerializeField]
        protected GameObjectPrefabPool[] groundPools;

        public int spawnCount;
        private int spawnCounting;
        private int groundIndex;

        [SerializeField]
        protected Vector3 spawnPosition;
        [SerializeField]
        protected float distroyX;

        [SerializeField]
        protected float moveSpeed;

        [SerializeField]
        protected float spawnGap;
        protected float spawnTimer;

        [SerializeField]
        protected float spawnMinY, spawnMaxY;

        [Header("UI display")]
        [SerializeField]
        protected GameObject startText;

        [SerializeField]
        protected TextMeshProUGUI scoreText;
        protected int score;

        protected List<GameObject> grounds = new List<GameObject>();

        protected virtual void Awake() {
            if (birdContoller != null) birdContoller.enabled = false;

            for (int i = 0; i < groundPools.Length; i++)
                groundPools[i].Initial();
        }

        protected virtual void Update()
        {
            if (!birdContoller.enabled)
            {
                if (Input.GetKeyDown(KeyCode.Space)) ResetGame();
            }
            else UpdateGround();
        }

        protected void UpdateGround()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnGap)
                SpawnGround();

            for (int i = 0; i < grounds.Count; i++)
            {
                Vector3 position = grounds[i].transform.position;
                bool cross = position.x <= scoreX;

                position.x += moveSpeed * Time.deltaTime;

                if (!cross && position.x <= scoreX)
                {
                    score += 1;
                    scoreText.text = score.ToString();
                }

                if (position.x <= distroyX)
                {
                    Destroy(grounds[i]);
                    grounds.RemoveAt(i);
                    i--;
                    break;
                }

                grounds[i].transform.position = position;
            }
        }

        protected void SpawnGround()
        {
            spawnTimer = 0;

            if (++spawnCounting > spawnCount)
            {
                spawnCounting = 0;
                if (++groundIndex >= groundPools.Length)
                {
                    groundIndex = 0;
                }
            }

            // int index = Random.Range(0, groundSetPrefabs.Length);
            GameObject newGround = groundPools[groundIndex].Get();
            newGround.SetActive(true);

            Vector3 position = spawnPosition;
            position.y = Random.Range(spawnMinY, spawnMaxY);
            newGround.transform.position = position;

            grounds.Add(newGround);
        }

        public virtual void GameOver()
        {
            startText.SetActive(true);
        }

        public virtual void ResetGame()
        {
            spawnCounting = groundIndex = 0;
            
            startText.SetActive(false);
            if (birdContoller != null) birdContoller.enabled = true;
            score = 0;
            scoreText.text = "0";

            while (grounds.Count > 0)
            {
                Destroy(grounds[0]);
                grounds.RemoveAt(0);
            }

            SpawnGround();
        }
    }
}