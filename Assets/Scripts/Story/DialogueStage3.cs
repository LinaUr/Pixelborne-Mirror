using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStage3 : MonoBehaviour
{
    [SerializeField]
    private int m_textPartDisplayTime = 3000;

    enum DialogueMode
    {
        NotStarted,
        Displaying,
        WaitingForTrigger
    }

    private bool m_enemiesKilled;
    private DialogueMode m_dialogueMode;
    private GameObject m_background;
    private GameObject m_dialogue;
    private GameObject m_nameTag;
    private int m_displayStartTime;
    private int m_textPart;
    private int m_dialoguePart;
    private string m_userName;
    private string[] m_dialogueText;
    private string[] m_dialogueTextPart0 = { "Knight! Is that you?",
                                             "You found me! Thank goodness.",
                                             "And I started to fear those vile demons might succeed.",
                                             "You must know, they believe the royal blood holds ancient power.",
                                             "Power they wanted to use to summon their Dark King from his -",
                                             "Hold on! Is that the Dark King's crown you have there?",
                                             "What a relief. We shall take it with us, so that they may never be able to use it.",
                                             "Come now, let us return to the castle at once!" };
    
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
        foreach (GameObject enemy in Singleplayer.Instance.ActiveEnemies)
        {
                allKilled = false;
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
        m_nameTag.GetComponent<TextMeshProUGUI>().text = "Princess";
        m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    public void ChangePart()
    {
        Singleplayer.Instance.ReachedEndOfStage();
    }
    
    public void GetName()
    {
        m_userName = Environment.UserName;
        m_dialogueTextPart0[0] = "Knight " + m_userName + "! Is that you?";
    }
}

