using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStage1 : MonoBehaviour
{
    [SerializeField]
    private int m_textPartDisplayTime = 3000;
    [SerializeField]
    private GameObject m_background;
    [SerializeField]
    private TextMeshProUGUI m_dialogue;
    [SerializeField]
    private TextMeshProUGUI m_nameTag;

    enum DialogueMode
    {
        NotStarted,
        Displaying,
        WaitingForTrigger
    }

    private bool m_enemiesKilled = false;
    private bool m_skipPart = false;
    private DialogueMode m_dialogueMode = DialogueMode.NotStarted;
    private int m_textPart;
    private int m_dialoguePart;
    private Stopwatch m_stopwatch = new Stopwatch();
    private string m_userName;
    private string[] m_dialogueText;
    private string[] m_dialogueTextPart0 = { "Knight! To me!" };
    private string[] m_dialogueTextPart1 = { "It's terrible!",
                                             "The demons have found the shards of Dark Crystal in our dungeons.",
                                             "They have stolen them...\nAnd they took my daughter, the princess!",
                                             "I fear they plan to use her blood and the stones to summon their Dark King!",
                                             "Knight!",
                                             "Find them! Find my daughter and the stones or we are all doomed!",
                                             "Knight! You must hurry!" };

    public bool PlayerProgressed { get; set; } = false;
 
    void Start()
    {
        GetName();
        m_background.GetComponent<Image>().color = Color.black;
        m_dialoguePart = 0;
        m_dialogueText = m_dialogueTextPart0;
        SetDialogueVisibility(false);
    }

    void Update()
    {
        m_enemiesKilled = EnemiesKilled();

        if (m_dialogueMode == DialogueMode.Displaying && Input.GetKeyDown("space"))
        {
            m_skipPart = true;
        }

        if (m_dialogueMode == DialogueMode.NotStarted && PlayerProgressed && m_enemiesKilled) 
        {
            ShowText();
        }
        else if (m_dialogueMode == DialogueMode.Displaying) 
        {
            m_dialogue.text = m_dialogueText[m_textPart];
            if (m_stopwatch.ElapsedMilliseconds >= m_textPartDisplayTime || m_skipPart)
            {
                m_textPart++;
                if (m_textPart == m_dialogueText.Length)
                {
                    ChangePart();
                }
                m_stopwatch.Restart();
                m_skipPart = false;
            }
        }
        else if (m_dialogueMode == DialogueMode.WaitingForTrigger && PlayerProgressed && m_enemiesKilled) 
        {
            ChangePart();
        }
    }

    public bool EnemiesKilled()
    {
        bool allKilled = true;
        foreach(GameObject enemy in Singleplayer.Instance.ActiveEnemies)
        {
            if (enemy.name == "EnemyStartRight" || enemy.name == "EnemyStartLeft")
            {
                allKilled = false;
            }
        }
        return allKilled;
    }

    public void ShowText()
    {
        Singleplayer.Instance.LockPlayerInput(true);
        Singleplayer.Instance.Player.GetComponent<PlayerMovement>().ResetEntityAnimations();
        m_dialogueMode = DialogueMode.Displaying;
        m_textPart = 0;
        m_nameTag.text = "King";
        SetDialogueVisibility(true);
        m_stopwatch.Restart();
    }

    public void ChangePart()
    {
        switch (m_dialoguePart)
        {
            case 0:
                Singleplayer.Instance.LockPlayerInput(false);
                SetDialogueVisibility(false);
                PlayerProgressed = false;
                m_dialogueMode = DialogueMode.WaitingForTrigger;
                break;

            case 1:
                m_dialogueText = m_dialogueTextPart1;
                ShowText();
                break;

            case 2:
                Singleplayer.Instance.LockPlayerInput(false);
                SetDialogueVisibility(false);
                PlayerProgressed = false;
                m_dialogueMode = DialogueMode.WaitingForTrigger;
                break;
        }

        m_dialoguePart++;
    }

    private void SetDialogueVisibility(bool isVisible)
    {
        m_background.SetActive(isVisible);
        m_dialogue.gameObject.SetActive(isVisible);
        m_nameTag.gameObject.SetActive(isVisible);
    }

    public void GetName()
    {
        m_userName = Environment.UserName;
        m_dialogueTextPart0[0] = "Knight " + m_userName + "! To me!";
        m_dialogueTextPart1[4] = "Knight " + m_userName + "!";
        m_dialogueTextPart1[6] = "Knight " + m_userName + "! You must hurry!";
    }
}

