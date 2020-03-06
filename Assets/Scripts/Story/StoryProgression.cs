using UnityEngine;

public class StoryProgression : MonoBehaviour
{
    [SerializeField]
    DialogueStage1 m_dialog;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Storytrigger")
        {
            collider.enabled = false;
            m_dialog.PlayerProgressed();
        }
    }
}
