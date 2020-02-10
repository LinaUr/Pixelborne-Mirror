using UnityEngine;

public class DisableCursor : MonoBehaviour
{
    void Start()
    {
        // Lock and hide curser.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
