using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class BankUIController : MonoBehaviour
    {
        private TextMeshProUGUI _pointsText;

        private void Awake()
        {
            _pointsText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ChangePoints(int value) => _pointsText.text = value.ToString();
    }
}