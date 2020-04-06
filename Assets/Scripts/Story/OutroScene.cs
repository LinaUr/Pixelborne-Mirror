using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutroScene : MonoBehaviour
{
    [SerializeField]
    private float m_fadeTime = 3000;
    [SerializeField]
    private float m_animationTime = 500;
    [SerializeField]
    private Image m_backgroundImage;
    [SerializeField]
    private TextMeshProUGUI m_story;

    private enum Mode
    {
        FadeImage,
        DisplayText,
        AnimateImages,
        Done
    }

    private Mode m_mode;
    private int m_storyPart = 0;
    private int m_textPart = 0;
    private int m_animationPart = 0;
    private Stopwatch m_stopwatch = new Stopwatch();
    private string[] m_animationPictures = { "OutroImages/possessed_land",
                                              "OutroImages/retreating_shadows",
                                              "OutroImages/shadows_almost_gone",
                                              "OutroImages/free_once_more"};
    //private string[] m_storyText;
    //private string[] m_storyTextPart0 = { "And as the crown splintered, a terrible screech rang out as its Dark Crystal cracked and dulled.",
    //                                      "A cold wind filled the world, a whisper of hate and ancient darkness.",
    //                                      "And then... silence."};
    //private string[] m_storyTextPart1 = { "As one, the demons all over the land froze.",
    //                                      "As one, they started wailing and screeching and turning on one another.",
    //                                      "And then, they disappeared as if sucked through a crack in the world, leaving not a trace." };
    //private string[] m_storyTextPart2 = { "And what remained was a beautiful kingdom, battered, but unbroken.",
    //                                      "What remained were triumphant people under the wise rule of a just queen.",
    //                                      "What remained was light and warmth and hope." };
    //private string[] m_storyTextPart3 = { "THE END" };
    //private string[] m_storyTextPart4 = { "THE END\n\nThank you for playing Pixelborne.\nPress Space tu return to Main Menu." };

    private readonly string[][] m_storyHolder =
    {
        new string[] {
            "And as the crown splintered, a terrible screech rang out as its Dark Crystal cracked and dulled.",
            "A cold wind filled the world, a whisper of hate and ancient darkness.",
            "And then... silence."
        },
        new string[] {
            "As one, the demons all over the land froze.",
            "As one, they started wailing and screeching and turning on one another.",
            "And then, they disappeared as if sucked through a crack in the world, leaving not a trace."
        },
        new string[] {
            "And what remained was a beautiful kingdom, battered, but unbroken.",
            "What remained were triumphant people under the wise rule of a just queen.",
            "What remained was light and warmth and hope."
        },
        new string[] { "THE END" },
        new string[] { "THE END\n\nThank you for playing Pixelborne.\nPress space to return to the main menu." }
    };

    void Start()
    {
        m_storyPart = 0;
        m_backgroundImage.overrideSprite = Resources.Load<Sprite>("OutroImages/dark_crown_destroyed");
        FadeOut();
    }

    void Update()
    {
        long elapsedTime = m_stopwatch.ElapsedMilliseconds;

        if (m_mode == Mode.FadeImage)
        {
            // Fade the colors darker.
            float percentage = elapsedTime / m_fadeTime;
            float colorValue = (1.0f - percentage) + 0.3f;
            m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);

            // Complete the fade to black when enough time has passed.
            if (elapsedTime >= m_fadeTime)
            {
                colorValue = 0.3f;
                m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);
                m_mode = Mode.DisplayText;
                ShowText();
            }
        }
        else if (m_mode == Mode.DisplayText)
        {
            m_story.text = m_storyHolder[m_storyPart][m_textPart];
            if (m_stopwatch.ElapsedMilliseconds >= m_fadeTime)
            {
                m_textPart++;
                if (m_textPart == m_storyHolder[m_storyPart].Length)
                {
                    ChangeStoryPart();
                }
                m_stopwatch.Restart();
            }
        }
        else if (m_mode == Mode.AnimateImages)
        {
            if (m_stopwatch.ElapsedMilliseconds >= m_animationTime || m_animationPart < 0)
            {
                m_animationPart++;
                if (m_animationPart == m_animationPictures.Length)
                {
                    ChangeStoryPart();
                    return;
                }
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_animationPictures[m_animationPart]);
                m_stopwatch.Restart();
            }
        }
        else if (m_mode == Mode.Done && Input.GetKeyDown("space"))
        {
            Singleplayer.Instance.EndStage();
        }
    }

    public void FadeOut()
    {
        m_stopwatch.Restart();
        m_mode = Mode.FadeImage;
    }

    public void ShowText()
    {
        m_mode = Mode.DisplayText;
        m_textPart = 0;
        m_stopwatch.Restart();
    }

    public void Animate()
    {
        m_animationPart = -1;
        m_mode = Mode.AnimateImages;
        m_stopwatch.Restart();
    }

    public void ChangeStoryPart()
    {
        m_storyPart++;
        switch (m_storyPart)
        {
            case 1:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("OutroImages/possessed_land");
                //m_storyText = m_storyTextPart1;
                break;

            case 2:
                Animate();
                break;

            case 3:
                //m_storyText = m_storyTextPart2;
                break;

            case 4:
                m_backgroundImage.color = Color.black;
                //m_storyText = m_storyTextPart3;
                ShowText();
                return;

            case 5:
                //m_story.text = m_storyTextPart4[0];
                m_mode = Mode.Done;
                return;
        }
        m_backgroundImage.color = Color.white;
        m_story.text = "";
        if (m_storyPart != 2)
        {
            FadeOut();
        }
    }
}
