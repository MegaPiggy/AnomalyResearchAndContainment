using NewHorizons.Handlers;
using System;
using UnityEngine;

namespace Jam5Entry
{
    public class BatteryConsole : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _screen;
        [SerializeField] private MeshRenderer _info;
        [SerializeField] private Material _battery;
        [SerializeField] private Material _noBattery;
        [SerializeField] private SingleInteractionVolume _button;

        private void Start()
        {
            if (_button != null)
            {
                _button.OnPressInteract += OnButtonPressed;
                var text = (UITextType)TranslationHandler.AddUI("Check Power Status", false);
                _button._textID = text;
                _button.Awake();
                _button.SetPromptText(text);
                _button.ResetInteraction();
            }

            // Hide info at start
            if (_info != null)
                _info.gameObject.SetActive(false);
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
            if (_info != null)
                _info.gameObject.SetActive(true);

            if (_screen != null)
            {
                _screen.material = DialogueConditionManager.SharedInstance.GetConditionState("ActivateAnomalyStation") ? _battery : _noBattery;
            }
        }
    }
}
