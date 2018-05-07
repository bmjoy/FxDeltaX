using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    private Vector3 offset;

	void Start ()
    {
        player = GameManager.instance.curentCharacter;
        offset = transform.position - player.transform.position;
	}
		
	void LateUpdate ()
    {
        player = GameManager.instance.curentCharacter;
        transform.position = player.transform.position + offset;
	}
    
}
