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

        public void Start()
        {
            _interactVolume.OnPressInteract += OnPressInteract;
            _interactVolume.ChangePrompt(UITextType.RebindX);
        }

        private void OnPressInteract()
        {
            _door.Toggle();
        }
    }
}
