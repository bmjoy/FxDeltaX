using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.UI; 


public class UIManager : MonoBehaviour
{

    public InputField equationInput;
    public Text timeLeftText;


    void Start ()
    {
    }

    void FixedUpdate ()
    {
        var playerMode = GameManager.GetPlayerMode();
        bool showCanvas = (playerMode == PlayerMode.SHOOTING);

        equationInput.gameObject.SetActive(showCanvas);
        equationInput.Select();
        equationInput.ActivateInputField();

        timeLeftText.text = GameManager.GetRoundLeftTime().ToString();
    }
}
