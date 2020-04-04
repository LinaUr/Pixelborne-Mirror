using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary></summary>
public class ChapterScreen : MonoBehaviour
{
    [SerializeField]
    private int m_fadeTime = 5000;

    private enum FadeMode
    {
        currentlyDisplayed,
        currentlyFading,
        completelyFaded
    }

    private FadeMode m_fadeMode;
    private int m_fadeStartTime;
    private GameObject m_background;
    private GameObject m_story;
  
    void Start()
    {
        Singleplayer.Instance.LockPlayerInput(true);
        m_background = transform.Find("ChapterBackground").gameObject;
        m_story = GameObject.Find("Story");
        m_fadeMode = 0;
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    void Update()
    {
        if (m_fadeMode == FadeMode.currentlyDisplayed)
        {
            if (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime >= m_fadeTime)
            {
                m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
                Singleplayer.Instance.LockPlayerInput(false);
                m_fadeMode = FadeMode.currentlyFading;
            }
        }
        else if (m_fadeMode == FadeMode.currentlyFading)
        {
            Color tmp = m_background.GetComponent<Image>().color;
            float takenTime = (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime) * 1.0f;
            float floatFadeTime = m_fadeTime * 1.0f;
            float percentage = takenTime / floatFadeTime;
            tmp.a = (1.0f - percentage);
            m_background.GetComponent<Image>().color = tmp;

            // Complete the fade to black when enough time has passed.
            if (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime >= m_fadeTime)
            {
                m_background.GetComponent<Image>().color = Color.clear;
                m_story.GetComponent<TextMeshProUGUI>().text = "";
                m_fadeMode = FadeMode.completelyFaded;
            }
        }
    }
}
