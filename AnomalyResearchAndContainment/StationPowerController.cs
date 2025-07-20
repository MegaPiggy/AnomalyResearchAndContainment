using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class StationPowerController : MonoBehaviour
    {
        [SerializeField] private NomaiEnergyCable _centerCable;
        [SerializeField] private NomaiLamp[] _lamps;
        [SerializeField] private AnomalyDoorController[] _anomalyDoorControllers;

        private bool _initialized = false;
        private bool _powered = false;

        protected void Start()
        {
            _lamps = GetComponentsInChildren<NomaiLamp>(true);
            _anomalyDoorControllers = GetComponentsInChildren<AnomalyDoorController>(true);
            UpdateVisuals();
            GlobalMessenger.AddListener("DialogueConditionsReset", UpdateVisuals);
            GlobalMessenger<string, bool>.AddListener("DialogueConditionChanged", UpdateVisuals);
        }

        protected void OnDestroy()
        {
            GlobalMessenger.RemoveListener("DialogueConditionsReset", UpdateVisuals);
            GlobalMessenger<string, bool>.RemoveListener("DialogueConditionChanged", UpdateVisuals);
        }

        private void UpdateVisuals()
        {
            var wasPowered = _powered;
            var powered = DialogueConditionManager.SharedInstance.GetConditionState("ActivateAnomalyStation");
            _powered = powered;
            if (!_initialized || wasPowered != powered)
            {
                _initialized = true;
                _centerCable.SetPowered(powered);
                foreach (var lamp in _lamps)
                {
                    lamp.FadeTo(powered ? 1 : 0, 1);
                }
                foreach (var anomalyDoorController in _anomalyDoorControllers)
                {
                    anomalyDoorController.Set(powered);
                }
            }
        }

        private void UpdateVisuals(string conditionName, bool conditionState)
        {
            if (conditionName == "ActivateAnomalyStation")
            {
                UpdateVisuals();
            }
        }
    }
}
