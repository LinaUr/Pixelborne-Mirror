using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterScreen : MonoBehaviour
{
    [SerializeField]
    private float m_fadeTime = 2000;
    [SerializeField]
    private GameObject m_background;
    [SerializeField]
    private GameObject m_story;

    private FadeMode m_fadeMode;
    private Stopwatch m_stopwatch = new Stopwatch();

    enum FadeMode
    {
        Displayed,
        Fading,
        Faded
    }
  
    void Start()
    {
        Singleplayer.Instance.LockPlayerInput(true);
        m_fadeMode = FadeMode.Displayed;
        m_stopwatch.Start();
    }

    void Update()
    {
        long elapsedTime = m_stopwatch.ElapsedMilliseconds;

        if (m_fadeMode == FadeMode.Displayed)
        {
            if (elapsedTime >= m_fadeTime)
            {
                m_stopwatch.Reset();
                m_stopwatch.Start();
                Singleplayer.Instance.LockPlayerInput(false);
                m_fadeMode = FadeMode.Fading;
            }
        }
        else if (m_fadeMode == FadeMode.Fading)
        {
            Color tmp = m_background.GetComponent<Image>().color;
            float percentage = elapsedTime / m_fadeTime;
            tmp.a = (1.0f - percentage);
            m_background.GetComponent<Image>().color = tmp;

            // Complete the fade to black when enough time has passed.
            if (elapsedTime >= m_fadeTime)
            {
                m_background.GetComponent<Image>().color = Color.clear;
                m_story.GetComponent<TextMeshProUGUI>().text = "";
                m_fadeMode = FadeMode.Faded;
            }
        }
    }
}
