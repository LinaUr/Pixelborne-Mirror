using UnityEngine;

public class EpilogueProgression : MonoBehaviour
{
    [SerializeField]
    DialogueStage4 m_dialog;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Storytrigger" && !m_dialog.PlayerProgressed)
        {
            m_dialog.PlayerProgressed = true;
            collider.enabled = false;
        }
    }
}
