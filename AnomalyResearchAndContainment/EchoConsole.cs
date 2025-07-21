using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace AnomalyResearchAndContainment
{
    public class EchoConsole : MonoBehaviour
    {
        [SerializeField] public EchoCellController controller;
        [SerializeField] private KeyDropper _keyDropper;
        [SerializeField] private Indicator _indicator;
        //private float _timeout = 10f;

        private List<int> _inputSequence = new List<int>();
        private float _lastInputTime;
        private bool _completed = false;

        public void RegisterPadStep(int index)
        {
            if (!controller.IsActive || _completed) return;

            //if (Time.time - _lastInputTime > _timeout)
            //{
            //    _inputSequence.Clear();
            //    PlayFailFeedback();
            //}

            _lastInputTime = Time.time;
            _inputSequence.Add(index);

            if (_inputSequence.Count >= controller.correctSequence.Count)
            {
                if (IsCorrectSequence())
                {
                    _completed = true;
                    if (_keyDropper != null) _keyDropper.DropKey();
                    controller.SetActivation(false);
                    controller.OpenDoor();
                    _indicator.PlaySuccessFeedback();
                }
                else
                {
                    _inputSequence.Clear();
                    _indicator.PlayFailFeedback();
                }
            }
        }

        private bool IsCorrectSequence()
        {
            for (int i = 0; i < controller.correctSequence.Count; i++)
            {
                if (controller.correctSequence[i] != _inputSequence[i])
                    return false;
            }
            return true;
        }
    }
}
