﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject player;
    private Vector3 offset;
    private bool enabled = false;

	void Start ()
    {
        offset = new Vector3(0, 0, -10);
	}

    public void Enable (bool en)
    {
        enabled = en;
    }
	void LateUpdate ()
    {
        if (!enabled)
            return;

        player = GameManager.instance.currentCharacter;
        if (player == null)
            return;
        transform.position = player.transform.position + offset;
	}
    
}
