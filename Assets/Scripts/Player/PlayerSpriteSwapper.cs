﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

// This class is responsible for swapping sprites at runtime.
// NOTE: For reference see https://www.erikmoberg.net/article/unity3d-replace-sprite-programmatically-in-animation
public class PlayerSpriteSwapper : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The folder must be in Assets/Resources.")]
    private string m_resourceSubfolderName = "VaiDrogulChar 1";
    [SerializeField]
    [Tooltip("This must be the name of sprite sheet that is used in the Animations.")]
    private string m_spriteBaseName = "VaiDrogulChar";
    [SerializeField]
    private SpriteColor m_spriteColor = SpriteColor.red;

    // The name of the currently loaded sprite sheet
    private SpriteColor m_loadedSpriteColor;
    // The dictionary containing all the sliced up sprites in the sprite sheet
    private Dictionary<string, Sprite> m_spriteSheet;
    // The Unity sprite renderer so that we don't have to get it multiple times
    private SpriteRenderer m_spriteRenderer;

    private enum SpriteColor
    {
        blue,
        green,
        red
    }

    private void Start()
    {
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        SwapSpriteSheet();
    }

    // Runs after the animation has done its work
    private void LateUpdate()
    {
        if (m_loadedSpriteColor != m_spriteColor)
        {
            SwapSpriteSheet();
        }

        // Swap out the sprite to be rendered by its name
        // Important: The base name of the sprite must be the same!
        m_spriteRenderer.sprite = m_spriteSheet[m_spriteRenderer.sprite.name];
    }

    private void SwapSpriteSheet()
    {
        LoadSpriteSheet();
        ChangeHudSymbol();
    }

    // This method loads the sprites from a sprite sheet.
    private void LoadSpriteSheet()
    {
        string spriteSheetName = m_spriteColor == SpriteColor.blue ? m_spriteBaseName : $"{m_spriteBaseName}_{Enum.GetName(typeof(SpriteColor), m_spriteColor)}";
        string spriteSheetPath = Path.Combine(new string[] { m_resourceSubfolderName, spriteSheetName });

         // Load the sprites from a sprite sheet file (png). 
         // Note: The file specified must exist in a folder named Resources
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetPath);

        m_spriteSheet = sprites.ToDictionary(x => $"{m_spriteBaseName}_{x.name.Split('_').Last().ToString()}", x => x);

        m_loadedSpriteColor = m_spriteColor;
    }

    private void ChangeHudSymbol()
    {
        GameMediator.Instance.SwapHudSymbol(gameObject, m_spriteSheet[$"{m_spriteBaseName}_0"]);
    }
}
