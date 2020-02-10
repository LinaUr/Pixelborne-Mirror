using TMPro;
using UnityEngine;
using System.IO;

public class SellingScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_fileTextMesh;

    void Start()
    {
        // Freeze game.
        Time.timeScale = 0;
        GameMediator.Instance.LockPlayerInput(true);

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        // Set file for sell on canvas.
        m_fileTextMesh.SetText(Path.GetRandomFileName());
    }

    // This method resumes the gameplay.
    public void FileSellApproved()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        SceneChanger.UnloadSellingScreenAdditive();
        GameMediator.Instance.LockPlayerInput(false);

        // TODO: Log sold file.
    }

    public void FileSellRejected()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        SceneChanger.UnloadSellingScreenAdditive();
        GameMediator.Instance.LockPlayerInput(false);

        // TODO: Reset game to checkpoint and respawn all enemies.
    }
}
