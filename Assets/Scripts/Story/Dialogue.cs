using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    protected int m_displayTime = 3000;
    [SerializeField]
    protected GameObject m_background;
    [SerializeField]
    protected TextMeshProUGUI m_dialogue;
    [SerializeField]
    protected TextMeshProUGUI m_nameTag;

    protected Stopwatch m_stopwatch = new Stopwatch();

    public bool HasPlayerProgressed { get; set; } = false;
    protected static readonly string DEFAULT_KNIGHT = "Ni";

}
