﻿using TMPro;
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

    private const float m_DEFAULT_PRICE = 1.00f;

    void Start()
    {
        GameMediator.Instance.FreezeGame();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        // Set file for sell on canvas.
        m_fileToSell = Path.GetRandomFileName();
        m_fileTextMesh.SetText(m_fileToSell);

        if (GameMediator.Instance.PriceToPay < m_DEFAULT_PRICE)
        {
            GameMediator.Instance.PriceToPay = m_DEFAULT_PRICE;
        }

        // Set price to pay in format of US currency on canvas.
        m_priceToPay = GameMediator.Instance.PriceToPay.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));
        m_priceTextMesh.SetText(m_priceToPay);
    }

    // This method resumes the gameplay and logs the sold file.
    public void SellFile()
    {
        Log($"Sold {m_fileToSell}");

        UnfreezeGame();
    }

    // This method resumes the gameplay and logs the payed price.
    public void PayPrice()
    {
        
        Log($"Payed {m_priceToPay}");
        GameMediator.Instance.PriceToPay *= 1.25f;

        UnfreezeGame();
    }

    public void RejectAll()
    {
        UnfreezeGame();

        // TODO: Reset game to checkpoint and respawn all enemies.
    }

    private void UnfreezeGame()
    {
        SceneChanger.UnloadSellingScreenAdditive();
        GameMediator.Instance.UnfreezeGame();
    }

    private void Log(string logMessage)
    {
        using (StreamWriter w = File.AppendText("SellingLog.txt"))
        {
            Toolkit.Log(logMessage, w);
        }
    }
}
