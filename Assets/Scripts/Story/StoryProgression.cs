using UnityEngine;

public class StoryProgression : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Storytrigger")
        {
            col.enabled = false;
            GameObject.Find("Dialogue").GetComponent<DialogueStage1>().PlayerProgressed();
        }
    }
}
