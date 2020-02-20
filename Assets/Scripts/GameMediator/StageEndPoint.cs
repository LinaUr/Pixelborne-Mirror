using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class StageEndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (GameMediator.Instance.ActivePlayers.Any(player => collider == player))
        {
            // Player reached end of stage.
            if (GameMediator.Instance.ActiveEnemies.Count == 0)
            {
                Singleplayer.Instance.EndOfStageIsReached();
            }
        }
    }

}
