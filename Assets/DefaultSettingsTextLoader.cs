using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultSettingsTextLoader : MonoBehaviour {

    public GameObject playersQtyInput;
    public GameObject roundTimeInput;

	void Start ()
    {
        InputField field = playersQtyInput.GetComponent<InputField>();
        field.text = GameSettings.instance.GetCharactersQty().ToString();

        field = roundTimeInput.GetComponent<InputField>();
        field.text = GameSettings.instance.GetRoundTime().ToString();
	}

}
