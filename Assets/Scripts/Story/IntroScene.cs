using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class manages the displaying of images and text in the intro scene of the singleplayer mode.
public class IntroScene : MonoBehaviour
{
    [SerializeField]
    private float m_fadeTime = 3000;
    [SerializeField]
    private Image m_backgroundImage;
    [SerializeField]
    private TextMeshProUGUI m_story;

    private enum Mode
    {
        FadeImage,
        DisplayText
    }

    private Mode m_mode;
    private int m_storyPart = 0;
    private int m_textPart = 0;
    private Stopwatch m_stopwatch = new Stopwatch();
    private string[] m_imageHolder =
    {
        "IntroImages/peaceful",
        "IntroImages/war",
        "IntroImages/castle_gates"
    };
    private readonly string[][] m_storyHolder =
    {
        new string[] { "Prologue\n\nDarkness" },
        new string[] {
            "Once upon a time, there was a peaceful kingdom, full of light and happiness.",
            "The people lived content lives under the rule of a just king.",
            "And everything was bright and colorful."
        },
        new string[] {
            "Until one day, darkness erupted.",
            "A dark energy claimed the land, and with it came darker creatures, ancient and full of malice.",
            "They burnt the towns. They slaughtered the people."
        },
        new string[] {
            "And eventually, they reached the castle gates.",
            "The kingdom was weak, and the gates could not be held.",
            "But a few brave knights remained, and they fought back with everything they had."
        }
    };

    void Start()
    {
        m_mode = Mode.DisplayText;
        m_story.text = m_storyHolder[m_storyPart][m_textPart];
        m_stopwatch.Start();
    }

    void Update()
    {
        float elapsedTime = m_stopwatch.ElapsedMilliseconds * 1.0f;

        if (m_mode == Mode.FadeImage)
        {
            // Fade the colors darker.
            float percentage = elapsedTime / m_fadeTime;
            float colorValue = (1.0f - percentage) + 0.3f;
            m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);

            // Complete the fade when enough time has passed.
            if (elapsedTime >= m_fadeTime)
            {
                colorValue = 0.3f;
                m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);
                m_story.gameObject.SetActive(true);
                m_mode = Mode.DisplayText;
                m_stopwatch.Restart();
            }
        }
        else if (m_mode == Mode.DisplayText)
        {
            if (elapsedTime >= m_fadeTime)
            {
                m_textPart++;

                if (m_textPart == m_storyHolder[m_storyPart].Length)
                {
                    m_textPart = 0;
                    ChangeStoryPart();
                    m_mode = Mode.FadeImage;
                    m_stopwatch.Restart();
                    return;
                }

                m_story.text = m_storyHolder[m_storyPart][m_textPart];
                m_stopwatch.Restart();
            }
        }
    }

    private void ChangeStoryPart()
    {
        m_storyPart++;

        if (m_storyPart == m_storyHolder.Length)
        {
            Singleplayer.Instance.EndStage();
            return;
        }

        // Disable the text.
        m_story.gameObject.SetActive(false);

        // Change the background image.
        m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_imageHolder[m_storyPart]);
        m_backgroundImage.color = Color.white;
    }
}
