using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StoryProgression : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Storytrigger")
        {
            col.enabled = false;
            GameObject.Find("Dialogue").GetComponent<DialogueStage1>().PlayerProgressed();
        }
    }
}
