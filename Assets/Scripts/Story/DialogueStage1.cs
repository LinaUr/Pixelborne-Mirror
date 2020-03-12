using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStage1 : MonoBehaviour
{
    [SerializeField]
    private int m_textPartDisplayTime = 5000;

    enum DialogueMode
    {
        NotStarted,
        Displaying,
        WaitingForTrigger
    }

    private DialogueMode m_dialogueMode;
    private int m_displayStartTime;
    private int m_textPart;
    private int m_dialoguePart;
    bool m_enemiesKilled;
    GameObject m_background;
    GameObject m_dialogue;
    GameObject m_nameTag;
    string m_userName;
    string[] m_dialogueText;
    string[] m_dialogueTextPart0 = { "Knight! To me!" };
    string[] m_dialogueTextPart1 = { "It's terrible!",
                                     "The demons have found the shards of Dark Crystal in our dungeons.",
                                     "They have stolen them...\nAnd they took my daughter, the princess!",
                                     "I fear they plan to use her blood and the stones to summon their Dark King!",
                                     "Knight!",
                                     "Find them! Find my daughter and the stones or we are all doomed!",
                                     "Knight! You must hurry!" };
    
    public bool PlayerProgressed { get; set; }
 
    void Start()
    {
        m_background = GameObject.Find("Background");
        m_dialogue = GameObject.Find("Speech");
        m_nameTag = GameObject.Find("NameTag");
        m_dialoguePart = 0;
        GetName();
        m_dialogueText = m_dialogueTextPart0;
        PlayerProgressed = false;
        m_enemiesKilled = false;
        m_dialogueMode = DialogueMode.NotStarted;
    }

    void Update()
    {
        m_enemiesKilled = EnemiesKilled();

        if (m_dialogueMode == DialogueMode.Displaying && Input.GetKeyDown("space"))
        {
            m_displayStartTime -= m_textPartDisplayTime;
        }

        if (m_dialogueMode == DialogueMode.NotStarted && PlayerProgressed && m_enemiesKilled) 
        {
            ShowText();
        }
        else if (m_dialogueMode == DialogueMode.Displaying) 
        {
            m_dialogue.GetComponent<TextMeshProUGUI>().text = m_dialogueText[m_textPart];
            if (Toolkit.CurrentTimeMillisecondsToday() - m_displayStartTime >= m_textPartDisplayTime)
            {
                m_textPart++;
                if (m_textPart == m_dialogueText.Length)
                {
                    ChangePart();
                }
                m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
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
        m_background.GetComponent<Image>().color = Color.black;
        m_nameTag.GetComponent<TextMeshProUGUI>().text = "King";
        m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    public void ChangePart()
    {
        m_dialoguePart++;
        switch (m_dialoguePart)
        {
            case 1:
                m_background.GetComponent<Image>().color = Color.clear;
                m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
                m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
                PlayerProgressed = false;
                m_dialogueMode = DialogueMode.WaitingForTrigger;
                Singleplayer.Instance.LockPlayerInput(false);
                break;

            case 2:
                m_dialogueText = m_dialogueTextPart1;
                ShowText();
                break;

            case 3:
                m_dialogueMode = DialogueMode.WaitingForTrigger;
                m_background.GetComponent<Image>().color = Color.clear;
                m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
                m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
                PlayerProgressed = false;
                Singleplayer.Instance.LockPlayerInput(false);
                break;
        }
    }

    public void GetName()
    {
        m_userName = Environment.UserName;
        m_dialogueTextPart0[0] = "Knight " + m_userName + "! To me!";
        m_dialogueTextPart1[4] = "Knight " + m_userName + "!";
        m_dialogueTextPart1[6] = "Knight " + m_userName + "! You must hurry!";
    }
}

