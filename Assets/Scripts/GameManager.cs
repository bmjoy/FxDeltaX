using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public GameObject[] characters;
    GameObject curentCharacter;
    int characterIdx;

	// Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        characterIdx = 0;
        curentCharacter = characters[characterIdx];
        enableCharacter(characterIdx);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            characterIdx = (characterIdx + 1) % characters.Length;
        }
        enableCharacter(characterIdx);
	}

    private void enableCharacter(int idx)
    {
        for(int i=0; i<characters.Length; i++)
        {
            characters[i].GetComponent<Platformer2DUserControl>().enabled = (i == idx);
        }
    }
}
