using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    protected int m_displayTime = 3000;
    [SerializeField]
    protected GameObject m_background;
    [SerializeField]
    protected TextMeshProUGUI m_dialogue;
    [SerializeField]
    protected TextMeshProUGUI m_nameTag;

    protected int m_textPart = 0;
    protected int m_dialoguePart = 0;
    protected Stopwatch m_stopwatch = new Stopwatch();

    public bool HasPlayerProgressed { get; set; } = false;
    protected static readonly string DEFAULT_KNIGHT = "Ni";

    protected virtual string[][] DialogueHolder { get; set; }

    protected virtual void Start()
    {
        InsertName();
        m_background.GetComponent<Image>().color = Color.black;
        SetDialogueVisibility(false);
    }

    protected virtual void SetDialogueVisibility(bool isVisible)
    {
        m_background.SetActive(isVisible);
        m_dialogue.gameObject.SetActive(isVisible);
        m_nameTag.gameObject.SetActive(isVisible);
    }

    protected void InsertName()
    {
        if (!string.IsNullOrWhiteSpace(Environment.UserName))
        {
            for (int i = 0; i < DialogueHolder.Length; i++)
            {
                for (int j = 0; j < DialogueHolder[i].Length; j++)
                {
                    DialogueHolder[i][j] = DialogueHolder[i][j].Replace(DEFAULT_KNIGHT, Environment.UserName);
                }
            }
        }
    }

}
