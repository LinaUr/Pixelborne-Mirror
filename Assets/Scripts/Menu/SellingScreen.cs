using TMPro;
using UnityEngine;
using System.IO;
using System.Globalization;

public class SellingScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_fileTextMesh;
    [SerializeField]
    private TextMeshProUGUI m_priceTextMesh;

    private string m_fileToSell = string.Empty;
    private string m_priceToPay = string.Empty;

    private const float m_DEFAULT_PRICE = 1.0f;
    private const string m_LOG_FILE = "SellingLog.txt";

    void Start()
    {
        Game.Freeze();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        // Set file for sell on canvas.
        m_fileToSell = Path.GetRandomFileName();
        m_fileTextMesh.SetText(m_fileToSell);

        if (Singleplayer.Instance.PriceToPay < m_DEFAULT_PRICE)
        {
            Singleplayer.Instance.PriceToPay = m_DEFAULT_PRICE;
        }

        // Set price to pay in format of US currency on canvas.
        m_priceToPay = Singleplayer.Instance.PriceToPay.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));
        m_priceTextMesh.SetText(m_priceToPay);
    }

    // This method resumes the gameplay and logs the sold file.
    public void SellFile()
    {
        Toolkit.LogToFile($"Sold {m_fileToSell}", m_LOG_FILE);
        Singleplayer.Instance.RevivePlayer();

        UnfreezeGame();
    }

    // This method resumes the gameplay and logs the payed price.
    public void PayPrice()
    {
        Toolkit.LogToFile($"Payed {m_priceToPay}", m_LOG_FILE);
        Singleplayer.Instance.PriceToPay *= 1.25f;
        Singleplayer.Instance.RevivePlayer();

        UnfreezeGame();
    }

    public void RejectAll()
    {
        UnfreezeGame();

        // TODO: Reset game to checkpoint and respawn all enemies.
        Singleplayer.Instance.PrepareStage();
    }

    private void UnfreezeGame()
    {
        SceneChanger.UnloadSellingScreenAdditive();
        Game.Unfreeze();
    }
}
