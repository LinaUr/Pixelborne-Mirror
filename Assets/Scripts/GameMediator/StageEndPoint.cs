using System.Linq;
using UnityEngine;


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
