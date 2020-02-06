using TMPro;
using UnityEngine;
using System.IO;

public class WinningScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_winningTextMesh;

    void Start()
    {
        // Freeze game.
        Time.timeScale = 0;
        GameMediator.Instance.LockPlayerInput(true);

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        ShowWinningMessage();
    }

    // This method resumes the gameplay.
    public void ShowWinningMessage()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        GameMediator.Instance.LockPlayerInput(false);
        string winningPlayer = GameMediator.Instance.Winner;
        // Set winningPlayer on canvas.
        m_winningTextMesh.SetText($"{winningPlayer} has won!");
        SceneChanger.LoadWinningScreenAdditive();
    }

    public void RemoveWinningScreenAndOpenMainMenu()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        GameMediator.Instance.LockPlayerInput(false);
        SceneChanger.UnloadWinningScreenAdditive();
        SceneChanger.SetMainMenuAsActiveScene();
    }
}
