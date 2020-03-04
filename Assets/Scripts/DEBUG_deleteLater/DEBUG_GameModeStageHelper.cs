using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// This class is for testing and debugging single stages quicker without having to start from the MainMenu.
// Add this script to every playable multiplayer or singleplayer scene and set the stage index (starting from 0, including the Intro/Outro for singleplayer)
// TODO: Remove later.
class DEBUG_GameModeStageHelper : MonoBehaviour
{
    [SerializeField]
    private Mode gameMode = Mode.Singleplayer;
    [SerializeField]
    private int stageIndex;

    private void Awake()
    {
        if (gameMode == Mode.Singleplayer)
        {
            var sp = Singleplayer.Instance;
            Game.Current = sp;
            sp.DEBUG_currentStageIndex = stageIndex;
            sp.Go();
        }
        else if (gameMode == Mode.Multiplayer)
        {
            var mp = Multiplayer.Instance;
            Game.Current = mp;
            mp.Go();
            mp.DEBUG_currentStageIndex = stageIndex;
            mp.PrepareStage();
        }
    }
}
