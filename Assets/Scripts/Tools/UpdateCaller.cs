using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCaller : MonoBehaviour
{
    //Registers other objects so that they can use the update event
    private static UpdateCaller instance = null;
    public static System.Action OnUpdate;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != instance)
            Destroy(this);
    }

    void Update()
    {
        if (OnUpdate != null)
            OnUpdate();
    }
}
