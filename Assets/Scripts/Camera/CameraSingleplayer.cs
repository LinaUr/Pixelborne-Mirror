using UnityEngine;

// This class controlls the camera of the singleplayer scene.
public class CameraSingleplayer : MonoBehaviour, ICamera
{
    [SerializeField]
    GameObject m_follows;

    void Start()
    {
        Singleplayer.Instance.Camera = this;
    }

    void Update()
    {
        gameObject.transform.position = new Vector3(m_follows.transform.position.x, m_follows.transform.position.y, gameObject.transform.position.z);
    }

    public void FadeOut()
    {
    }

    public void FadeIn()
    {
    }

    public void SwapHudSymbol (GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

}
