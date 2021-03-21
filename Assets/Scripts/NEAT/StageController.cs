using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StageController : MonoBehaviour
{
    public static StageController ins;

    public GameObject[] stages;
    private int m_stageIndex;

    public GameObject stageButtonPrefab;

    public Button autoToggleButton;
    private bool m_autoMode = false;
    private GameObject[] buttons;
    
    private void Awake()
    {
        ins = this;
        autoToggleButton.onClick.AddListener(ToggleAutoMode);

        Vector3 position = autoToggleButton.transform.position;
        float height = autoToggleButton.GetComponent<RectTransform>().sizeDelta.y;
        Canvas canvas = FindObjectOfType<Canvas>();

        buttons = new GameObject[stages.Length];

        for (int i = 0; i < stages.Length; i++)
        {
            position.y -= 10 + height;

            GameObject button = Instantiate(stageButtonPrefab);
            button.transform.SetParent(canvas.transform);
            button.transform.position = position;
            button.GetComponent<Button>().onClick.AddListener(delegate {
                ChangeStage(button);
            });

            button.name = stages[i].name;
            button.GetComponentInChildren<TextMeshProUGUI>().text = stages[i].name;

            buttons[i] = button;
        }
    }

    public void ChangeStage(GameObject button)
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (button.name == stages[i].name)
            {
                m_stageIndex = i;
                stages[i].SetActive(true);
                break;
            }
        }

        autoToggleButton.gameObject.SetActive(false);
        for (int i = 0; i < buttons.Length; i++) buttons[i].SetActive(false);
    }

    public void StageTraningEnd()
    {
        stages[m_stageIndex].gameObject.SetActive(false);

        if (m_autoMode)
        {
            if (++m_stageIndex >= stages.Length)
            {
                Quit();
            }
            else
            {
                stages[m_stageIndex].gameObject.SetActive(true);
            }
        }
        else
        {
            autoToggleButton.gameObject.SetActive(true);
            for (int i = 0; i < buttons.Length; i++) buttons[i].SetActive(true);
        }
    }

    public void SaveProgress()
    {
        stages[m_stageIndex].gameObject.SendMessage("SaveTraining", SendMessageOptions.DontRequireReceiver);
    }

    public void ToggleAutoMode()
    {
        if (m_autoMode)
        {
            m_autoMode = false;
            autoToggleButton.targetGraphic.color = Color.white;
        }
        else
        {
            m_autoMode = true;
            autoToggleButton.targetGraphic.color = new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }

    public void Quit()
    {
        // Quit the game
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
