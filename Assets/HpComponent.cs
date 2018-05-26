using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class HpComponent : BarComponent
    {
        protected override void WhenBelowOrEqualZero()
        {
            GameManager.instance.KillCharacter(gameObject);
        }


        protected override Material GetMaterial()
        {
            return GetColoredMaterial(Color.red);
        }

        static readonly Vector3 offset = new Vector3(0, 2, 0);

        protected override Vector3 GetOffset()
        {
            return offset;
        }

    }
}
