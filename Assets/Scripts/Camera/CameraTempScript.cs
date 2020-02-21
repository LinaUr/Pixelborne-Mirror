using UnityEngine;

//This is a temp camera script for single player level construction and testing
public class CameraTempScript : MonoBehaviour, ICamera
{
    [SerializeField]
    GameObject m_follows;
    // Start is called before the first frame update
    void Start()
    {
        GameMediator.Instance.ActiveCamera = this;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(m_follows.transform.position.x, m_follows.transform.position.y, gameObject.transform.position.z);
    }

    public void FadeOut()
    {
        GameMediator.Instance.FadedOut();
    }

    public void FadeIn()
    {
        GameMediator.Instance.FadedIn();
    }

    public void SetPosition(int value) { }
    public void SwapHudSymbol (GameObject x, Sprite y) { }

}
