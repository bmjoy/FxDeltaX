using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class StaminaComponent : BarComponent
    {
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
            InvokeRepeating("decreaseStamina", 0.0f, 1f);
        }

        private void decreaseStamina()
        {
            Dec(2.0f);
        }
    }
}
