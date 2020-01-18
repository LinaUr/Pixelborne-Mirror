using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    public GameObject m_credits;
    public GameObject m_mainMenu;

    Vector3 m_originalPos;
    
    void Start()
    {
        // Save the original position of credits for reset.
        m_originalPos = m_credits.transform.position;
    }
    
    void Update()
    {
        // Scroll the credits.
        m_credits.transform.Translate(Vector3.up * 0.05f);

        // If the credits are out of screen, reset the position of credits and activate the main menu.
        if (transform.position.y >= 15)
        {
            m_credits.SetActive(false);
            m_credits.transform.position = m_originalPos;
            m_mainMenu.SetActive(true);
        }
    }
}
