using NewHorizons.Handlers;
using System;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class BatteryConsole : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _battery;
        [SerializeField] private MeshRenderer _noBattery;
        [SerializeField] private SingleInteractionVolume _button;

        private void Start()
        {
            _button = GetComponentInChildren<SingleInteractionVolume>();
            if (_button != null)
            {
                _button.OnPressInteract += OnButtonPressed;
                _button.ChangePrompt((UITextType)TranslationHandler.AddUI("Check Power Status", false));
            }

            // Hide info at start
            if (_battery != null)
            {
                _battery.gameObject.SetActive(false);
            }
            if (_noBattery != null)
            {
                _noBattery.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.OnPressInteract -= OnButtonPressed;
            }
        }

        private void OnButtonPressed()
        {
            if (_battery != null)
            {
                _battery.gameObject.SetActive(DialogueConditionManager.SharedInstance.GetConditionState("ActivateAnomalyStation"));
            }
            if (_noBattery != null)
            {
                _noBattery.gameObject.SetActive(!DialogueConditionManager.SharedInstance.GetConditionState("ActivateAnomalyStation"));
            }
        }
    }
}