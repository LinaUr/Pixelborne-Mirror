using UnityEngine;
using System;

// This class implements a simple color filter that gets stronger the further you go in a stage.
public class SimpleColorFilter : MonoBehaviour
{
    [SerializeField]
    private GameObject m_filterImage;
    [SerializeField]
    private float m_startX;
    [SerializeField]
    private float m_startY;
    [SerializeField]
    private float m_maxDistance;
    [SerializeField]
    private float m_maxFilterStrength;
    
    void Start()
    {
        m_startX = 0;
        m_startY = 0;
        m_maxDistance = 100;
        m_maxFilterStrength = 0.5,

    }

    
    void Update()
    {
        float opacity;
        if (m_maxDistance != 0)
        {
            float distX = Math.Abs(gameObject.transform.position.x - m_startX);
            float distY = Math.Abs(gameObject.transform.position.y - m_startY);
            float distance = (float)Math.Sqrt(distX * distX + distY * distY);
            float distPercentage = distance / m_maxDistance;
            opacity = distPercentage * m_maxFilterStrength;
        }
        else
        {
            opacity = m_maxFilterStrength;
        }
        Color color = m_filterImage.GetComponent<SpriteRenderer>().color;
        color.a = opacity;
        m_filterImage.GetComponent<SpriteRenderer>().color = color;
    }
}
