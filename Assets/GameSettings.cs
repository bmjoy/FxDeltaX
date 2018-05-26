using System;
using UnityEngine;


public class GameSettings : MonoBehaviour {

    public static GameSettings instance;
    private int roundTime = 30;
    private int charactersQty = 4;

    public GameSettings()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SetCharactersQty(string str)
    {
        if (str.Length == 0)
            return;
        charactersQty = Int32.Parse(str);
    }
    public int GetCharactersQty()
    {
        return charactersQty;
    }

    public void SetRoundTime(string str)
    {
        if (str.Length == 0)
            return;
        roundTime = Int32.Parse(str);
    }
    public int GetRoundTime()
    {
        return roundTime;
    }

}
