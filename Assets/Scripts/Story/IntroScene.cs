﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroScene : MonoBehaviour
{
    [SerializeField]
    private int m_fadeTime = 5000;

    enum FadeMode
    {
        currentlyFading,
        currentlyDisplaying
    }

    private int m_fadeStartTime;
    private FadeMode m_fadeMode;
    private int m_textPart;
    private int m_storyPart;
    string[] m_storyText;

    string[] m_storyTextPart0 = { "Prologue\n\nDarkness" };
    string[] m_storyTextPart1 = {"Once upon a time, there was a peaceful kingdom, full of light and happiness.",
                                 "The people lived content lives under the rule of a just king.",
                                 "And everything was bright and colorful."};
    string[] m_storyTextPart2 = {"Until one day, darkness erupted.",
                                 "A dark energy claimed the land, and with it came darker creatures, ancient and full of malice.",
                                 "They burnt the towns. They slaughtered the people."};
    string[] m_storyTextPart3 = {"And eventually, they reached the castle gates.",
                                 "The kingdom was weak, and the gates could not be held.",
                                 "But a few brave knights remained, and they fought back with everything they had."};

    GameObject m_background;
    GameObject m_story;
   
    void Start()
    {
        m_background = GameObject.Find("Background");
        m_story = GameObject.Find("Story");
        m_storyPart = 0;
        m_storyText = m_storyTextPart0;
        ShowText();
    }

    void Update()
    {
        TextMeshProUGUI test = m_story.GetComponent<TextMeshProUGUI>();
        print(test.text);
        if (m_fadeMode == FadeMode.currentlyFading)
        {
            // Change the color to black.
            Color tmp = m_background.GetComponent<Image>().color;
            float takenTime = (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime) * 1.0f;
            float floatFadeTime = m_fadeTime * 1.0f;
            float percentage = takenTime / floatFadeTime;
            tmp.r = (1.0f - percentage) + 0.3f;
            tmp.g = (1.0f - percentage) + 0.3f;
            tmp.b = (1.0f - percentage) + 0.3f;
            m_background.GetComponent<Image>().color = tmp;
            // Complete the fade to black when enough time has passed.
            if (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime >= m_fadeTime)
            {
                tmp.r = 0.3f;
                tmp.g = 0.3f;
                tmp.b = 0.3f;
                m_background.GetComponent<Image>().color = tmp;
                m_fadeMode = 0;
                ShowText();
            }
        }
        else if (m_fadeMode == FadeMode.currentlyDisplaying)
        {
            m_story.GetComponent<TextMeshProUGUI>().text = m_storyText[m_textPart];
            if (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime >= m_fadeTime)
            {
                m_textPart++;
                if (m_textPart == m_storyText.Length)
                {
                    ChangePic();
                }
                m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
            }
        }
    }

    public void FadeOut()
    {
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
        m_fadeMode = FadeMode.currentlyFading;
    }

    public void ShowText()
    {
        m_fadeMode = FadeMode.currentlyDisplaying;
        m_textPart = 0;
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    public void ChangePic()
    {
        m_storyPart++;
        switch (m_storyPart)
        {
            case 1:
                m_background.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("IntroImages/peaceful");
                m_storyText = m_storyTextPart1;
                break;

            case 2:
                m_background.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("IntroImages/war");
                m_storyText = m_storyTextPart2;
                break;

            case 3:
                m_background.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("IntroImages/castle_gates");
                m_storyText = m_storyTextPart3;
                break;

            case 4:
                ChangeScene();
                break;
        }
        m_background.GetComponent<Image>().color = Color.white;
        m_story.GetComponent<TextMeshProUGUI>().text = "";
        FadeOut();
    }

    public void ChangeScene()
    {
        Singleplayer.Instance.ReachedEndOfStage();
    }
} 
