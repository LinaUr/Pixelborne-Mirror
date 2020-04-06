using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// This class manages the displaying of the dialog in stage 1 of the singleplayer mode.
public class DialogueStage1 : Dialogue
{
    enum Mode
    {
        Displaying,
        WaitingForTrigger
    }

    private bool m_skipPart = false;
    private Mode m_mode = Mode.WaitingForTrigger;
    private int m_textPart = 0;
    private int m_dialoguePart = 0;
    private Stopwatch m_stopwatch = new Stopwatch();
    private readonly string[][] m_dialogueHolder =
    {
        new string[] { $"Knight {DEFAULT_KNIGHT}! To me!" },
        new string[] {
            "It's terrible!",
            "The demons have found the shards of Dark Crystal in our dungeons.",
            "They have stolen them...\nAnd they took my daughter, the princess!",
            "I fear they plan to use her blood and the stones to summon their Dark King!",
            $"Knight {DEFAULT_KNIGHT}!",
            "Find them! Find my daughter and the stones or we are all doomed!",
            $"Knight {DEFAULT_KNIGHT}! You must hurry!"
        }
    };
 
    void Start()
    {
        InsertName();
        m_nameTag.text = "King";
        m_background.GetComponent<Image>().color = Color.black;
        SetDialogueVisibility(false);
    }

    void Update()
    {
        bool enemiesKilled = AreFirstEnemiesKilled();

        if (m_mode == Mode.Displaying) 
        {
            m_dialogue.text = m_dialogueHolder[m_dialoguePart][m_textPart];

            if (m_stopwatch.ElapsedMilliseconds >= m_displayTime || Input.GetKeyDown("space"))
            {
                m_textPart++;

                if (m_textPart == m_dialogueHolder[m_dialoguePart].Length)
                {
                    ChangePart();
                    m_stopwatch.Stop();
                    return;
                }
                m_stopwatch.Restart();
            }
        }
        else if (m_mode == Mode.WaitingForTrigger && HasPlayerProgressed && enemiesKilled) 
        {
            Singleplayer.Instance.LockPlayerInput(true);
            m_mode = Mode.Displaying;
            SetDialogueVisibility(true);
            m_stopwatch.Start();
        }
    }

    private bool AreFirstEnemiesKilled()
    {
        foreach(GameObject enemy in Singleplayer.Instance.ActiveEnemies)
        {
            if (enemy.name == "EnemyStartRight" || enemy.name == "EnemyStartLeft")
            {
                return false;
            }
        }
        return true;
    }

    private void ChangePart()
    {
        Singleplayer.Instance.LockPlayerInput(false);
        SetDialogueVisibility(false);
        HasPlayerProgressed = false;
        m_mode = Mode.WaitingForTrigger;
        m_textPart = 0;
        m_dialoguePart++;
    }

    private void SetDialogueVisibility(bool isVisible)
    {
        m_background.SetActive(isVisible);
        m_dialogue.gameObject.SetActive(isVisible);
        m_nameTag.gameObject.SetActive(isVisible);
    }

    private void InsertName()
    {
        if (!string.IsNullOrWhiteSpace(Environment.UserName))
        {
            for (int i = 0; i < m_dialogueHolder.Length; i++)
            {
                for (int j = 0; j < m_dialogueHolder[i].Length; j++)
                {
                    m_dialogueHolder[i][j] = m_dialogueHolder[i][j].Replace(DEFAULT_KNIGHT, Environment.UserName);
                }
            }
        }
    }
}

