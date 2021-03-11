using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameControl : MonoBehaviour
{
    static public GameControl ins;

    [SerializeField]
    private GameObject[] groundSetPrefabs;

    [SerializeField]
    private Vector3 spawnPosition;
    [SerializeField]
    private float distroyX;

    [SerializeField]
    protected float scoreX;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float spawnGap;
    private float spawnTimer;

    [SerializeField]
    private float spawnMinY, spawnMaxY;

    [SerializeField]
    private BirdContoller birdContoller;

    [SerializeField]
    protected GameObject startText;

    [SerializeField]
    protected TextMeshProUGUI scoreText;
    private int score;

    protected List<GameObject> grounds = new List<GameObject>();

    protected virtual void Awake() {
        ins = this;
        birdContoller.enabled = false;
    }

    void Update()
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

        int index = Random.Range(0, groundSetPrefabs.Length);
        GameObject newGround = Instantiate(groundSetPrefabs[index]);

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
