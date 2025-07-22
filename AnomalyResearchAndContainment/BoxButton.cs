using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class BoxButton : MonoBehaviour
    {
        [SerializeField] private SingleInteractionVolume _interactVolume;
        [SerializeField] private BoxDoor _door;
        [SerializeField] private bool _isInsideButton = false;

        public void Start()
        {
            _interactVolume.OnPressInteract += OnPressInteract;
            _interactVolume.ChangePrompt(UITextType.RebindX);
        }

        private void OnPressInteract()
        {
            if (_isInsideButton)
            {
                _door.Close();

                Locator.GetShipLogManager().RevealFact("ARC_FINAL_ROOM_X4", true, true);
                DialogueConditionManager.SharedInstance.SetConditionState("BoxEnding");
            }
            else
            {
                _door.Open();
            }

            _interactVolume.DisableInteraction(); // Optional: prevent repeat use
        }
    }
}
