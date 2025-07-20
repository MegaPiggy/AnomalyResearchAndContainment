using NewHorizons.Handlers;
using NewHorizons.Utility.OWML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public static class EndTimesHandler
    {
        public static bool initialized = false;
        public static AudioType audioType;
        public static AudioType normalAudioType;

        public static void LoadAudio()
        {
            audioType = AudioTypeHandler.GetAudioType("planets/EndTimes_Redone.wav", AnomalyResearchAndContainment.Instance);
        }

        public static void Initialize()
        {
            initialized = false;
            LoadAudio();

            Delay.RunWhen(() => Locator.GetGlobalMusicController() != null, () =>
            {
                normalAudioType = Locator.GetGlobalMusicController()._endTimesSource.audioLibraryClip;
                initialized = true;
            });
        }

        public static void Assign()
        {
            Locator.GetGlobalMusicController()._endTimesSource.AssignAudioLibraryClip(audioType);
        }

        public static void Unassign()
        {
            Locator.GetGlobalMusicController()._endTimesSource.AssignAudioLibraryClip(normalAudioType);
        }

        public static void FadeOut()
        {
            Locator.GetGlobalMusicController()._playingEndTimes = false;
            Locator.GetGlobalMusicController()._endTimesSource.FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP, 0f);
        }

        public class CustomEndTimesVolume : EffectVolume
        {
            public override void OnEffectVolumeEnter(GameObject hitObj)
            {
                if (hitObj.CompareTag("PlayerDetector"))
                {
                    AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("Assigning custom end times");
                    Assign();
                    FadeOut();
                }
            }

            public override void OnEffectVolumeExit(GameObject hitObj)
            {
                if (hitObj.CompareTag("PlayerDetector"))
                {
                    AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("Unassigning custom end times");
                    Unassign();
                    FadeOut();
                }
            }
        }
    }
}
