using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class TouchingKiller : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            HpComponent hpComponent = collision.gameObject.GetComponent<HpComponent>();
            if (hpComponent != null)
            {
                hpComponent.Set(-1);
            }
        }
        private void Update()
        {
            
        }
        private void Awake()
        {
            
        }
    }
