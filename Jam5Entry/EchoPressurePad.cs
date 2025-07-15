using UnityEngine;

namespace Jam5Entry
{
    public class EchoPressurePad : PressurePad
    {
        [SerializeField] private int _padIndex;
        [SerializeField] private EchoConsole _console;
        [SerializeField] private OWAudioSource _audioSource;
        private AudioType _tone = AudioType.NomaiOrbStartDrag;

        protected override void OnStep(GameObject hitObj)
        {
            _audioSource?.PlayOneShot(_tone);
            _console?.RegisterPadStep(_padIndex);
        }
    }
}
