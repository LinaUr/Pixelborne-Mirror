using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    public GameObject credits;
    public GameObject mainMenu;

    Vector3 originalPos;
    
    void Start()
    {
        // save the original position of credits for reset:
        originalPos = credits.transform.position;
    }
    
    void Update()
    {
        // scroll the credits:
        credits.transform.Translate(Vector3.up * 0.05f);

        // if the credits are out of screen, reset the position of credits and activate the main menu:
        if (transform.position.y >= 15)
        {
            credits.SetActive(false);
            credits.transform.position = originalPos;
            mainMenu.SetActive(true);
        }
    }
}
