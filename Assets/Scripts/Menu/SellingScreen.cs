using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SellingScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_fileTextMesh;
    [SerializeField]
    private TextMeshProUGUI m_priceTextMesh;

    private string m_fileToSell = string.Empty;
    private string m_priceToPay = string.Empty;
    private static int s_currentSellingFileIndex = 0;
    private static string[] s_importantFiles = GetImportantFiles();

    private const float m_DEFAULT_PRICE = 1.0f;
    private const string m_LOG_FILE = "SellingLog.txt";

    void Start()
    {
        Game.Freeze();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        // Set file for sell on canvas.
        m_fileToSell = s_currentSellingFileIndex < s_importantFiles.Length ? s_importantFiles[s_currentSellingFileIndex] : Path.GetTempFileName();
        
        m_fileTextMesh.SetText(m_fileToSell);

        if (Singleplayer.Instance.PriceToPay < m_DEFAULT_PRICE)
        {
            Singleplayer.Instance.PriceToPay = m_DEFAULT_PRICE;
        }

        // Set price to pay in format of US currency on canvas.
        m_priceToPay = Singleplayer.Instance.PriceToPay.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));
        m_priceTextMesh.SetText(m_priceToPay);
    }

    private static string[] GetImportantFiles(){
        // Manually combine this path to make it work on Linux, because strangely
        // Environment.SpecialFolder.MyDocuments also leads to the user's home directory.
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string directory = Path.Combine(homeDir, "Documents");

        return Toolkit.GetFiles(directory, new List<string>() { }).ToArray();
    }

    // This method resumes the gameplay and logs the sold file.
    public void SellFile()
    {
        Toolkit.LogToFile($"Sold {m_fileToSell}", m_LOG_FILE);
        s_currentSellingFileIndex++;
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
        // Reload current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UnfreezeGame();
    }

    private void UnfreezeGame()
    {
        SceneChanger.UnloadSellingScreenAdditive();
        Game.Unfreeze();
    }
}
