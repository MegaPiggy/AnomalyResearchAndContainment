using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class StationVolumeController : MonoBehaviour
    {
        public NomaiWarpReceiver warpReceiver;
        public NomaiWarpTransmitter warpTransmitter;
        [SerializeField] private OWTriggerVolume _volume;

        public void Initialize()
        {
            warpReceiver.OnReceiveWarpedBody += this.OnReceiveWarpedBody;
            warpTransmitter.OnReceiveWarpedBody += this.OnReceiveWarpedBody;
        }

        private void OnDestroy()
        {
            warpReceiver.OnReceiveWarpedBody -= this.OnReceiveWarpedBody;
            warpTransmitter.OnReceiveWarpedBody -= this.OnReceiveWarpedBody;
        }

        public void OnReceiveWarpedBody(OWRigidbody body, NomaiWarpPlatform startPlatform, NomaiWarpPlatform targetPlatform)
        {
            if (body.CompareTag("Player"))
            {
                if (targetPlatform == warpReceiver)
                {
                    _volume.AddObjectToVolume(Locator.GetPlayerDetector());
                    _volume.AddObjectToVolume(Locator.GetPlayerCameraDetector());
                }
                else if (targetPlatform == warpTransmitter)
                {
                    _volume.RemoveObjectFromVolume(Locator.GetPlayerDetector());
                    _volume.RemoveObjectFromVolume(Locator.GetPlayerCameraDetector());
                }
            }
            else if (body.CompareTag("Probe"))
            {
                if (targetPlatform == warpReceiver)
                {
                    _volume.AddObjectToVolume(Locator.GetProbe().GetDetectorObject());
                }
                else if (targetPlatform == warpTransmitter)
                {
                    _volume.RemoveObjectFromVolume(Locator.GetProbe().GetDetectorObject());
                }
            }
        }
    }
}
