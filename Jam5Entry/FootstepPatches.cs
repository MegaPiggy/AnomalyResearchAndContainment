using HarmonyLib;
using UnityEngine;

namespace Jam5Entry
{
    [HarmonyPatch(typeof(PlayerMovementAudio), "PlayFootstep")]
    public static class FootstepPatches
    {
        public static bool Prefix(PlayerMovementAudio __instance)
        {
            if (PlayerState.IsTransitioningToDreamUnderground())
                return false;

            var fluidDetector = __instance._fluidDetector;
            var playerController = __instance._playerController;
            var footstepAudio = __instance._footstepAudio;

            AudioType audioType;

            if (!PlayerState.IsCameraUnderwater() && fluidDetector != null && fluidDetector.InFluidType(FluidVolume.Type.WATER))
            {
                audioType = AudioType.MovementShallowWaterFootstep;
            }
            else
            {
                audioType = PlayerMovementAudio.GetFootstepAudioType(playerController.GetGroundSurface());
            }

            if (audioType != AudioType.None && footstepAudio != null)
            {
                footstepAudio.pitch = Random.Range(0.9f, 1.1f);
                footstepAudio.PlayOneShot(audioType, 0.7f);

                // Send footstep to EchoCellController
                var echoCell = EchoCellController.Instance;
                if (echoCell != null)
                {
                    echoCell.RegisterSound(audioType);
                }
            }

            return false; // Prevent original method from running
        }
    }
}
