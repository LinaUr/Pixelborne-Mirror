using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour
{
    [SerializeField]
    private float m_fadeTime = 3000;
    [SerializeField]
    private Image m_backgroundImage;
    [SerializeField]
    private TextMeshProUGUI m_story;

    enum ImageMode
    {
        Fading,
        Displaying
    }

    private ImageMode m_imageMode;
    private int m_storyPart;
    private int m_textPart;
    private Stopwatch m_stopwatch = new Stopwatch();
    private string[] m_storyText;
    private string[] m_storyTextPart0 = { "Prologue\n\nDarkness" };
    private string[] m_storyTextPart1 = { "Once upon a time, there was a peaceful kingdom, full of light and happiness.",
                                          "The people lived content lives under the rule of a just king.",
                                          "And everything was bright and colorful." };
    private string[] m_storyTextPart2 = { "Until one day, darkness erupted.",
                                          "A dark energy claimed the land, and with it came darker creatures, ancient and full of malice.",
                                          "They burnt the towns. They slaughtered the people." };
    private string[] m_storyTextPart3 = { "And eventually, they reached the castle gates.",
                                          "The kingdom was weak, and the gates could not be held.",
                                          "But a few brave knights remained, and they fought back with everything they had." };

    void Start()
    {
        m_storyPart = 0;
        m_storyText = m_storyTextPart0;
        ShowText();
    }

    void Update()
    {
        long elapsedTime = m_stopwatch.ElapsedMilliseconds;

        if (m_imageMode == ImageMode.Fading)
        {
            // Fade the colors darker.
            Color color = m_backgroundImage.color;
            float percentage = elapsedTime / m_fadeTime;
            float colorValue = (1.0f - percentage) + 0.3f;
            m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);

            // Complete the fade when enough time has passed.
            if (elapsedTime >= m_fadeTime)
            {
                colorValue = 0.3f;
                m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);
                m_imageMode = 0;
                ShowText();
            }
        }
        else if (m_imageMode == ImageMode.Displaying)
        {
            m_story.text = m_storyText[m_textPart];
            if (elapsedTime >= m_fadeTime)
            {
                m_textPart++;
                if (m_textPart == m_storyText.Length)
                {
                    ChangePic();
                }
                m_stopwatch.Restart();
            }
        }
    }

    public void FadeOut()
    {
        m_stopwatch.Restart();
        m_imageMode = ImageMode.Fading;
    }

    public void ShowText()
    {
        m_imageMode = ImageMode.Displaying;
        m_textPart = 0;
        m_stopwatch.Restart();
    }

    public void ChangePic()
    {
        m_storyPart++;
        switch (m_storyPart)
        {
            case 1:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("IntroImages/peaceful");
                m_storyText = m_storyTextPart1;
                break;

            case 2:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("IntroImages/war");
                m_storyText = m_storyTextPart2;
                break;

            case 3:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("IntroImages/castle_gates");
                m_storyText = m_storyTextPart3;
                break;

            case 4:
                ChangeScene();
                break;
        }
        m_backgroundImage.color = Color.white;
        m_story.text = "";
        FadeOut();
    }

    public void ChangeScene()
    {
        Singleplayer.Instance.EndStage();
    }
}
