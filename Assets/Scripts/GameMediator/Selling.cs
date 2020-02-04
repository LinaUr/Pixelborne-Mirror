using TMPro;
using UnityEngine;
using System.IO;

public class Selling : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_fileTextMesh;
    // Start is called before the first frame update
    void Start()
    {
        m_fileTextMesh.SetText(Path.GetRandomFileName());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
