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
        public static AudioType audioType;
        public static AudioType normalAudioType;
        public static OWAudioSource endTimesSource;

        public static void LoadAudio()
        {
            audioType = AudioTypeHandler.GetAudioType("planets/EndTimes_Redone.wav", Jam5Entry.Instance);
        }

        public static void Initialize()
        {
            LoadAudio();

            endTimesSource = Locator.GetGlobalMusicController()._endTimesSource;
            normalAudioType = endTimesSource.audioLibraryClip;
        }

        public static void Assign()
        {
            endTimesSource.AssignAudioLibraryClip(audioType);
        }

        public static void Unassign()
        {
            endTimesSource.AssignAudioLibraryClip(normalAudioType);
        }
    }
}
