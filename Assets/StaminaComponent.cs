using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class StaminaComponent : BarComponent
    {
        protected float staminaDecreaseStep;

        StaminaComponent()
        {
            staminaDecreaseStep = 100.0f / GameSettings.instance.GetRoundTime();
        }

        protected override void WhenBelowOrEqualZero()
        {
            //throw new NotImplementedException();
        }

        protected override Material GetMaterial()
        {
            return GetColoredMaterial(Color.yellow);
        }

        static readonly Vector3 offset = new Vector3(0, 1.8f, 0);

        protected override Vector3 GetOffset()
        {
            return offset;
        }

        protected override void handleTimeoutOnStamina()
        {
            InvokeRepeating("decreaseStaminaInTime", 0.0f, 1f);
        }

        private void decreaseStamina()
        {
            Dec(staminaDecreaseStep);
        }

        private void decreaseStaminaInTime()
        {
            if (GameManager.GetGameState() == State.GAMEPLAY)
                decreaseStamina();
        }
    }
}
