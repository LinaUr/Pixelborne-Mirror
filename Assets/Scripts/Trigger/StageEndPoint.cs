using UnityEngine;

// This class is marks a GameObject that functions as a trigger.
// It needs to be assigned to a GameObject as a Script component.
public class StageEndPoint : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D collider)
    {
        if (collider.gameObject == Singleplayer.Instance.Player)
        {
            // Player reached end of stage.
            if (Singleplayer.Instance.ActiveEnemies.Count == 0)
            {
                Singleplayer.Instance.ReachedEndOfStage();
            }
        }
    }
}
