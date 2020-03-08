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

    [SerializeField]
    private GameObject Player = null;
    [SerializeField]
    private GameObject Enemies = null;

    void Awake()
    {
        if (gameMode == Mode.Singleplayer)
        {
            var sp = Singleplayer.Instance;
            Game.Current = sp;
            sp.DEBUG_currentStageIndex = stageIndex;
            sp.Go();

            Player.SetActive(true);
            Enemies.SetActive(true);
        }
        else if (gameMode == Mode.Multiplayer)
        {
            var mp = Multiplayer.Instance;
            Game.Current = mp;
            mp.Go();
            mp.DEBUG_currentStageIndex = stageIndex;
        }
    }
}
