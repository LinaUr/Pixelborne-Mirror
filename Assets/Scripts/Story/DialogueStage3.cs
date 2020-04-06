using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStage3 : Dialogue
{
    private enum Mode
    {
        NotStarted,
        Displaying,
        WaitingForTrigger
    }

    private Mode m_mode = Mode.WaitingForTrigger;
    private int m_textPart;
    private string[] m_dialogueText;
    

    private string[] m_dialogueTextPart0 = { $"Knight {DEFAULT_KNIGHT}! Is that you?",
                                             "You found me! Thank goodness.",
                                             "And I started to fear those vile demons might succeed.",
                                             "You must know, they believe the royal blood holds ancient power.",
                                             "Power they wanted to use to summon their Dark King from his -",
                                             "Hold on! Is that the Dark King's crown you have there?",
                                             "What a relief. We shall take it with us, so that they may never be able to use it.",
                                             "Come now, let us return to the castle at once!" };
    private bool m_skipPart;

    void Start()
    {
        InsertName();
        m_nameTag.text = "Princess";
        m_background.GetComponent<Image>().color = Color.black;
        SetDialogueVisibility(false);
        //m_dialogueText = m_dialogueTextPart0;
    }

    void Update()
    {
        bool enemiesKilled = Singleplayer.Instance.ActiveEnemies.Count == 0;


        switch (m_mode)
        {
            case Mode.WaitingForTrigger:
                if (HasPlayerProgressed && enemiesKilled)
                {
                    ShowText();
                }
                break;

            case Mode.Displaying:
                m_dialogue.GetComponent<TextMeshProUGUI>().text = m_dialogueText[m_textPart];

                if (m_stopwatch.ElapsedMilliseconds >= m_displayTime || Input.GetKeyDown("space"))
                {
                    m_textPart++;

                    if (m_textPart == m_dialogueText.Length)
                    {
                        m_stopwatch.Stop();
                        Singleplayer.Instance.LockPlayerInput(false);
                        Singleplayer.Instance.EndStage();
                        return;
                    }
                    m_stopwatch.Restart();
                }
                break;
        }
    }

    public void ShowText()
    {
        Singleplayer.Instance.LockPlayerInput(true);
        m_mode = Mode.Displaying;
        
        m_stopwatch.Restart();
    }

    private void SetDialogueVisibility(bool isVisible)
    {
        m_background.SetActive(isVisible);
        m_dialogue.gameObject.SetActive(isVisible);
        m_nameTag.gameObject.SetActive(isVisible);
    }

    public void InsertName()
    {
        m_dialogueTextPart0[0] = "Knight " + Environment.UserName + "! Is that you?";

        //if (!string.IsNullOrWhiteSpace(Environment.UserName))
        //{
        //    for (int i = 0; i < m_dialogueHolder.Length; i++)
        //    {
        //        for (int j = 0; j < m_dialogueHolder[i].Length; j++)
        //        {
        //            m_dialogueHolder[i][j] = m_dialogueHolder[i][j].Replace(DEFAULT_KNIGHT, Environment.UserName);
        //        }
        //    }
        //}
    }
}
