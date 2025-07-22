using NewHorizons.Handlers;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class AnomalyConsole : MonoBehaviour
    {
        [SerializeField] private SingleInteractionVolume _button;
        [SerializeField] private AnomalyController _controller;

        private void Start()
        {
            if (_button == null)
                _button = GetComponentInChildren<SingleInteractionVolume>();

            if (_button != null)
            {
                _button.OnPressInteract += OnButtonPressed;
                _button.ChangePrompt((UITextType)TranslationHandler.AddUI("Start Puzzle", false));
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.OnPressInteract -= OnButtonPressed;
        }

        private void OnButtonPressed()
        {
            if (_controller == null) return;
            _controller.CloseDoor();
            _controller.SetActivation(true);
            DeactivateButton();
        }

        public void DeactivateButton()
        {
            _button.DisableInteraction();
        }
    }
}
