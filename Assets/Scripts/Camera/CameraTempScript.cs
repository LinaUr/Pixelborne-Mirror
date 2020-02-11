using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a temp camera script for single player level construction and testing
public class CameraTempScript : MonoBehaviour
{
    [SerializeField]
    GameObject m_follows;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(m_follows.transform.position.x, m_follows.transform.position.y, gameObject.transform.position.z);
    }
}
