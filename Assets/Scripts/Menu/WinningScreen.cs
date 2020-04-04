using TMPro;
using UnityEngine;

/// <summary></summary>
public class WinningScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_winningTextMesh;

    void Start()
    {
        // Set winningPlayer on canvas.
        m_winningTextMesh.SetText($"{Game.Current.GetWinner()} won!");
    }

    /// <summary>Opens the main menu.</summary>
    public void OpenMainMenu()
    {
        SceneChanger.SetMainMenuAsActiveScene();
    }
}
