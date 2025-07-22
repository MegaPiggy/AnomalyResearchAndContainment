using UnityEngine;
using NewHorizons.Handlers;

namespace AnomalyResearchAndContainment
{
    public class RotatingCrystal : ReflectiveSurface
    {
        [SerializeField] private SingleInteractionVolume _interactionVolume;
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationAmount = 45; // degrees per press
        [SerializeField] private bool _rotateClockwise = true;

        private void Awake()
        {
            if (_interactionVolume != null)
            {
                _interactionVolume.OnPressInteract += OnPressInteract;
                _interactionVolume.ChangePrompt((UITextType)TranslationHandler.AddUI("Rotate Crystal", false));
            }
        }

        private void OnDestroy()
        {
            if (_interactionVolume != null)
                _interactionVolume.OnPressInteract -= OnPressInteract;
        }

        private void OnPressInteract()
        {
            RotateCrystal();
        }

        public void RotateCrystal() => RotateCrystal(_rotateClockwise ? 1 : -1);

        public void RotateCrystal(float direction)
        {
            transform.Rotate(_rotationAxis, _rotationAmount * direction, Space.Self);
        }
    }
}
