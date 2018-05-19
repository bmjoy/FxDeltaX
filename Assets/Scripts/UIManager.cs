using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.UI; 


public class UIManager : MonoBehaviour
{

    private InputField equationInput;

    void Start ()
    {
        equationInput = Canvas.FindObjectOfType<InputField>();
    }

    void Update ()
    {
        var playerMode = GameManager.GetPlayerMode();
        bool showCanvas = (playerMode == PlayerMode.SHOOTING);

        equationInput.gameObject.SetActive(showCanvas);
        equationInput.Select();
        equationInput.ActivateInputField();
    }
}
