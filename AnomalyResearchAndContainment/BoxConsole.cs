using NewHorizons.Handlers;
using System;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class BoxConsole : MonoBehaviour
    {
        [SerializeField] private GlassLiftController _glassLift;
        [SerializeField] private SingleInteractionVolume _button;

        private void Start()
        {
            _button = GetComponentInChildren<SingleInteractionVolume>();
            if (_button != null)
            {
                _button.OnPressInteract += OnButtonPressed;
                _button.ChangePrompt((UITextType)TranslationHandler.AddUI("Raise Containment Glass", false));
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
            _button.DisableInteraction();
            _glassLift.Raise();
        }
    }
}