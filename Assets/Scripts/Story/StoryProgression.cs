using UnityEngine;

/// <summary></summary>
public class StoryProgression : MonoBehaviour
{
    [SerializeField]
    DialogueStage1 m_dialog;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Storytrigger" && !m_dialog.PlayerProgressed)
        {
            m_dialog.PlayerProgressed = true;
            collider.enabled = false;
        }
    }
}
