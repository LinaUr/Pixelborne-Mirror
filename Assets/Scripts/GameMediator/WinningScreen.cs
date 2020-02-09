using TMPro;
using UnityEngine;

public class WinningScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_winningTextMesh;

    void Start()
    {
        // Set winningPlayer on canvas.
        m_winningTextMesh.SetText($"Player {Toolkit.WinnerIndex} has won!");
    }

    public void OpenMainMenu()
    {
        SceneChanger.SetMainMenuAsActiveScene();
    }
}
