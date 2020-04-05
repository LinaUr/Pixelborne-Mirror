using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutroScene : MonoBehaviour
{
    [SerializeField]
    private int m_fadeTime = 5000;

    [SerializeField]
    private int m_animationTime = 500;

    enum FadeMode
    {
        currentlyFading,
        currentlyDisplaying,
        currentlyAnimated,
        done
    }

    private FadeMode m_fadeMode;
    private GameObject m_background;
    private GameObject m_story;
    private int m_storyPart;
    private int m_textPart;
    private int m_animationPart;
    private Stopwatch m_textStopwatch = new Stopwatch();
    private string[] m_animationPictures;
    private string[] m_animationPictures0 = { "OutroImages/possessed_land",
                                              "OutroImages/retreating_shadows",
                                              "OutroImages/shadows_almost_gone",
                                              "OutroImages/free_once_more"};
    private string[] m_storyText;
    private string[] m_storyTextPart0 = { "And as the crown splintered, a terrible screech rang out as its Dark Crystal cracked and dulled.",
                                          "A cold wind filled the world, a whisper of hate and ancient darkness.",
                                          "And then... silence."};
    private string[] m_storyTextPart1 = { "As one, the demons all over the land froze.",
                                          "As one, they started wailing and screeching and turning on one another.",
                                          "And then, they disappeared as if sucked through a crack in the world, leaving not a trace." };
    private string[] m_storyTextPart2 = { "And what remained was a beautiful kingdom, battered, but unbroken.",
                                          "What remained were triumphant people under the wise rule of a just queen.",
                                          "What remained was light and warmth and hope." };
    private string[] m_storyTextPart3 = { "THE END" };
    private string[] m_storyTextPart4 = { "THE END\n\nThank you for playing Pixelborne.\nPress Space tu return to Main Menu." };

    void Start()
    {
        m_background = GameObject.Find("Background");
        m_story = GameObject.Find("Story");
        m_storyPart = 0;
        m_storyText = m_storyTextPart0;
        m_background.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("OutroImages/dark_crown_destroyed");
        FadeOut();
    }

    void Update()
    {
        if (m_fadeMode == FadeMode.currentlyFading)
        {
            // Change the color to black.
            Color tmp = m_background.GetComponent<Image>().color;
            float takenTime = m_textStopwatch.ElapsedMilliseconds * 1.0f;
            float floatFadeTime = m_fadeTime * 1.0f;
            float percentage = takenTime / floatFadeTime;
            tmp.r = (1.0f - percentage) + 0.3f;
            tmp.g = (1.0f - percentage) + 0.3f;
            tmp.b = (1.0f - percentage) + 0.3f;
            m_background.GetComponent<Image>().color = tmp;

            // Complete the fade to black when enough time has passed.
            if (m_textStopwatch.ElapsedMilliseconds >= m_fadeTime)
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
            if (m_textStopwatch.ElapsedMilliseconds >= m_fadeTime)
            {
                m_textPart++;
                if (m_textPart == m_storyText.Length)
                {
                    ChangePic();
                }
                m_textStopwatch.Restart();
            }
        }
        else if (m_fadeMode == FadeMode.currentlyAnimated)
        {
            if (m_textStopwatch.ElapsedMilliseconds >= m_animationTime || m_animationPart < 0)
            {
                m_animationPart++;
                if (m_animationPart == m_animationPictures.Length)
                {
                    ChangePic();
                    return;
                }
                m_background.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(m_animationPictures[m_animationPart]);
                m_textStopwatch.Restart();
            }
        }
        else if (m_fadeMode == FadeMode.done && Input.GetKeyDown("space"))
        {
            ChangeScene();
        }
    }

    public void FadeOut()
    {
        m_textStopwatch.Restart();
        m_fadeMode = FadeMode.currentlyFading;
    }

    public void ShowText()
    {
        m_fadeMode = FadeMode.currentlyDisplaying;
        m_textPart = 0;
        m_textStopwatch.Restart();
    }

    public void Animate()
    {
        m_animationPart = -1;
        m_fadeMode = FadeMode.currentlyAnimated;
        m_textStopwatch.Restart();
    }

    public void ChangePic()
    {
        m_storyPart++;
        switch (m_storyPart)
        {
            case 1:
                m_background.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("OutroImages/possessed_land");
                m_storyText = m_storyTextPart1;
                break;

            case 2:
                m_animationPictures = m_animationPictures0;
                Animate();
                break;

            case 3:
                m_storyText = m_storyTextPart2;
                break;

            case 4:
                m_background.GetComponent<Image>().color = Color.black;
                m_storyText = m_storyTextPart3;
                ShowText();
                return;

            case 5:
                m_story.GetComponent<TextMeshProUGUI>().text = m_storyTextPart4[0];
                m_fadeMode = FadeMode.done;
                return;
        }
        m_background.GetComponent<Image>().color = Color.white;
        m_story.GetComponent<TextMeshProUGUI>().text = "";
        if (m_storyPart != 2)
        {
            FadeOut();
        }
    }

    public void ChangeScene()
    {
        Singleplayer.Instance.EndStage();
    }
}
