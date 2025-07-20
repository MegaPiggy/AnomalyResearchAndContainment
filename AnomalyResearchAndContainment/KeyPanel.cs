using NewHorizons.Components.Props;
using System.Net.Sockets;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class KeyPanel : MonoBehaviour
    {
        private OWItemSocket _keySlot;

        public void Start()
        {
            _keySlot = GetComponentInChildren<OWItemSocket>(true);
        }

        public bool IsKeyInserted() => _keySlot != null && _keySlot.GetSocketedItem() != null;
    }
}
