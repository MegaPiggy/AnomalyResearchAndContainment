using NewHorizons.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jam5Entry
{
    public class MemoryOrbConsole : MonoBehaviour
    {
        [SerializeField] private MemoryOrbController _memoryOrbController;
        [SerializeField] private SingleInteractionVolume _interactVolume;

        private enum Mode
        {
            Idle,
            Recording,
            ReadyToPlay
        }

        private Mode _mode = Mode.Idle;

        private void Start()
        {
            if (_interactVolume != null)
            {
                _interactVolume.OnPressInteract += OnPressInteract;
            }
            SetPromptText();
            DeactivateInteraction();
        }

        private void OnDestroy()
        {
            if (_interactVolume != null)
                _interactVolume.OnPressInteract -= OnPressInteract;
        }

        private void OnPressInteract()
        {
            if (_memoryOrbController == null || !_memoryOrbController.IsActive) return;
            switch (_mode)
            {
                case Mode.Idle:
                    _memoryOrbController.StartRecording();
                    _mode = Mode.Recording;
                    SetPromptText();
                    break;

                case Mode.Recording:
                    _memoryOrbController.StopRecording();
                    _mode = Mode.ReadyToPlay;
                    SetPromptText();
                    break;

                case Mode.ReadyToPlay:
                    _memoryOrbController.StartPlayback();
                    _mode = Mode.Idle;
                    SetPromptText();
                    break;
            }
        }

        private void SetPromptText()
        {
            if (_interactVolume == null) return;
            switch (_mode)
            {
                case Mode.Idle:
                    _interactVolume.ChangePrompt((UITextType)TranslationHandler.AddUI("Start Recording", false)); // "Start Recording"
                    break;

                case Mode.Recording:
                    _interactVolume.ChangePrompt((UITextType)TranslationHandler.AddUI("Stop Recording", false)); // "Stop Recording"
                    break;

                case Mode.ReadyToPlay:
                    _interactVolume.ChangePrompt((UITextType)TranslationHandler.AddUI("Start Playback", false)); // "Start Playback"
                    break;
            }
        }

        public void ActivateInteraction()
        {
            _interactVolume.EnableInteraction();
        }

        public void DeactivateInteraction()
        {
            _interactVolume.DisableInteraction();
        }
    }
}
