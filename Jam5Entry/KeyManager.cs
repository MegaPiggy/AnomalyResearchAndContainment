using Epic.OnlineServices;
using NewHorizons.Components.Props;
using NewHorizons.Handlers;
using System.Linq;
using UnityEngine;

namespace Jam5Entry
{
    public class KeyManager : MonoBehaviour
    {
        [SerializeField]
        public ElevatorPad _elevatorPad;
        [SerializeField]
        public KeyPanel[] keyPanels = new KeyPanel[0];

        public int keysInserted => keyPanels.Count(panel => panel.IsKeyInserted());
        public bool allKeysInserted => keysInserted >= 4;
        public bool powered => DialogueConditionManager.SharedInstance.GetConditionState("ActivateAnomalyStation");

        private void Start()
        {
            keyPanels = GetComponentsInChildren<KeyPanel>(true);
            // Initially disable elevator
            _elevatorPad.DisableElevator();
        }

        private void Update()
        {
            if (allKeysInserted && powered)
                UnlockElevator();
            else
                LockElevator();
        }

        private void UnlockElevator()
        {
            if (!_elevatorPad.IsDeactivated) return;

            _elevatorPad.EnableElevator();

            Jam5Entry.Instance.ModHelper.Console.WriteLine("All keys inserted. Elevator unlocked.");
        }

        private void LockElevator()
        {
            if (_elevatorPad.IsDeactivated) return;

            _elevatorPad.DisableElevator();

            Jam5Entry.Instance.ModHelper.Console.WriteLine($"{(allKeysInserted ? "Keys inserted" : "Keys not inserted")} and {(powered ? "power on" : "power not on")}. Elevator locked.");
        }
    }
}
