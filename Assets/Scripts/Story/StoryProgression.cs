using UnityEngine;

/// <summary></summary>
public class StoryProgression : MonoBehaviour
{
    [SerializeField]
    private Dialogue m_dialog;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Storytrigger") && !m_dialog.HasPlayerProgressed)
        {
            m_dialog.HasPlayerProgressed = true;
            collider.enabled = false;
        }
    }
}
