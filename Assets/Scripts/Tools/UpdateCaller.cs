using UnityEngine;

// This class allows other objects to register themselves 
// so that they can use the Update() method without being attached to a game object.
public class UpdateCaller : MonoBehaviour
{
    private static UpdateCaller m_instance = null;

    public static System.Action OnUpdate;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != m_instance)
        { 
            Destroy(this);
        }
    }

    void Update()
    {
        OnUpdate?.Invoke();
    }
}
