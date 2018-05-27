using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets._2D;

namespace Assets
{
    public class FallingHitter : MonoBehaviour
    {
        const float speedPowerThreshold = 50;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (GameManager.instance.GetGameState() == State.GAMEPLAY)
            {
                PlatformerCharacter2D platformer = GetComponent<PlatformerCharacter2D>();
                if (platformer)
                {
                    Animator anim = platformer.getAnim();
                    float vSpeed = anim.GetFloat("vSpeed");
                    float speedPower = Mathf.Abs(vSpeed);
                    if (speedPower > speedPowerThreshold)
                    {
                        float toHit = speedPower - speedPowerThreshold;
                        HpComponent hpComponent = GetComponent<HpComponent>();
                        hpComponent.Dec(toHit);
                    }
                }
            }
        }
    }
}
