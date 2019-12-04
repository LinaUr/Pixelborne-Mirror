using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class PlayerHealthController
{
    //Keeps references for the health objects of both players, might want to rename at some point

    public static int maxhealth = 1;
    public static int respawnTime = 2;

    public static PlayerHealth player1Health = new PlayerHealth(maxhealth, respawnTime);
    public static PlayerHealth player2Health = new PlayerHealth(maxhealth, respawnTime);
}
