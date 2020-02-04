using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonNavigation : MonoBehaviour
{
    public GameObject topmostButton;
    public Button[] buttons;
    public EventSystem eventSystem;


    void Start()
    {
        //topmostButton.GetComponent<Button>().Select();
    }

    private void DeselectAll() { }

    private void Update()
    {
    }

    public void MouseOver()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnDown()
    {
        topmostButton.GetComponent<Button>().Select();
    }

    void OnWHATEVER() { }
}
