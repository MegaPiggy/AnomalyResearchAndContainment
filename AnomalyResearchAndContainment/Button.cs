using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class Button : MonoBehaviour
    {
        private SingleInteractionVolume _interactVolume;

        public Transform door;

        public void Awake()
        {
            _interactVolume = GetComponent<SingleInteractionVolume>();
            _interactVolume.OnPressInteract += OnPressInteract;
            _interactVolume.ChangePrompt(UITextType.RebindX);
        }

        private void OnPressInteract()
        {
            door.transform.SetLocalPositionZ(-4.75f);
            gameObject.SetActive(false);
        }
    }
}
