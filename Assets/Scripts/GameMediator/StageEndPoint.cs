using System.Linq;
using UnityEngine;


public class StageEndPoint : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D collider)
    {
        if (GameMediator.Instance.ActivePlayers.Any(player => collider.gameObject == player))
        {
            // Player reached end of stage.
            if (GameMediator.Instance.ActiveEnemies.Count == 0)
            {
                Singleplayer.Instance.ReachedEndOfStage();
            }
        }
    }

}
