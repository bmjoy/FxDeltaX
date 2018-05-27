using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class EquationScriptComponent : MonoBehaviour
    {
        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        public void OnStringChange(string newString)
        {
            _storedString = newString;
        }

        public string GetString()
        {
            return _storedString;
        }

        private string _storedString = "";
    }
}
