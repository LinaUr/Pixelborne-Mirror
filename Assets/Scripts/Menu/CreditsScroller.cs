using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField]
    private GameObject m_credits;
    [SerializeField]
    private GameObject m_mainMenu;

    private Vector3 m_originalPos;
    
    void Awake()
    {
        // Save the original position of credits for reset.
        m_originalPos = gameObject.transform.position;
    }

    private void OnEnable()
    {
        gameObject.transform.position = m_originalPos;
    }

    void Update()
    {
        // Scroll the credits.
        gameObject.transform.Translate(Vector3.up * 0.05f);

        // If the credits are out of screen, reset the position of credits and activate the main menu.
        if (gameObject.transform.position.y >= 15)
        {
            m_mainMenu.SetActive(true);
            m_credits.SetActive(false);
        }
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }
}
