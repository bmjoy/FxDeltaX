using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.UI; 


public class UIManager : MonoBehaviour
{

    public InputField equationInput;
    public Text timeLeftText;
    public Text pressSpaceText;

    void Start ()
    {
    }

    void FixedUpdate ()
    {
        var playerMode = GameManager.GetPlayerMode();
        var gameState = GameManager.GetGameState();
        bool showCanvas = (playerMode == PlayerMode.SHOOTING && gameState == State.GAMEPLAY);

        equationInput.gameObject.SetActive(showCanvas);
        equationInput.Select();
        equationInput.ActivateInputField();

        int time = GameManager.GetIdleLeftTime();
        bool visible = time > 0;

        timeLeftText.enabled = visible;
        timeLeftText.text = time.ToString();

        bool showPressSpace = (gameState == State.WAIT_FOR_ROUND_START);
        pressSpaceText.transform.position = equationInput.transform.position;
        pressSpaceText.enabled = showPressSpace;
        
    }
}
