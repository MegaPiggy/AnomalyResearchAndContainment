using NewHorizons.Handlers;
using NewHorizons.Utility.OWML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jam5Entry
{
    public static class EndTimesHandler
    {
        public static bool initialized = false;
        public static AudioType audioType;
        public static AudioType normalAudioType;
        public static OWAudioSource endTimesSource;

        public static void LoadAudio()
        {
            audioType = AudioTypeHandler.GetAudioType("planets/EndTimes_Redone.wav", Jam5Entry.Instance);
        }

        public static void Initialize()
        {
            initialized = false;
            LoadAudio();

            Delay.RunWhen(() => Locator.GetGlobalMusicController() != null, () =>
            {
                endTimesSource = Locator.GetGlobalMusicController()._endTimesSource;
                normalAudioType = endTimesSource.audioLibraryClip;
                initialized = true;
            });
        }

        public static void Assign()
        {
            endTimesSource.AssignAudioLibraryClip(audioType);
        }

        public static void Unassign()
        {
            endTimesSource.AssignAudioLibraryClip(normalAudioType);
        }

        public class CustomEndTimesVolume : EffectVolume
        {
            public override void OnEffectVolumeEnter(GameObject hitObj)
            {
                if (hitObj.CompareTag("PlayerDetector"))
                {
                    Jam5Entry.Instance.ModHelper.Console.WriteLine("Assigning custom end times");
                    Assign();
                }
            }

            public override void OnEffectVolumeExit(GameObject hitObj)
            {
                if (hitObj.CompareTag("PlayerDetector"))
                {
                    Jam5Entry.Instance.ModHelper.Console.WriteLine("Unassigning custom end times");
                    Unassign();
                }
            }
        }
    }
}
