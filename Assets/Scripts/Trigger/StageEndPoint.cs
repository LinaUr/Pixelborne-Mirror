using UnityEngine;

// This class marks a GameObject that functions as a trigger.
// It needs to be assigned to a GameObject as a Script component.
public class StageEndPoint : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D collider)
    {
        if (collider.gameObject == Singleplayer.Instance.Player && Singleplayer.Instance.ActiveEnemies.Count == 0)
        {
            // The Player reached end of the stage.
            Singleplayer.Instance.EndStage();
        }
    }
}
