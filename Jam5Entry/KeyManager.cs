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

        private void Start()
        {
            keyPanels = GetComponentsInChildren<KeyPanel>(true);
            // Initially disable elevator
            _elevatorPad.DisableElevator();
        }

        private void Update()
        {
            if (keysInserted < 4) return;

            // All keys are inserted!
            UnlockElevator();
        }

        private void UnlockElevator()
        {
            _elevatorPad.EnableElevator();

            Jam5Entry.Instance.ModHelper.Console.WriteLine("All keys inserted. Elevator unlocked.");
        }
    }
}
