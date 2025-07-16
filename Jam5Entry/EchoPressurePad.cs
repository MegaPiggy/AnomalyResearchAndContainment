using UnityEngine;

namespace Jam5Entry
{
    public class EchoPressurePad : PressurePad
    {
        [SerializeField] private int _padIndex;
        [SerializeField] private EchoConsole _console;
        [SerializeField] private OWAudioSource _audioSource;
        private AudioType _tone = AudioType.NomaiOrbStartDrag;
        private float _glowFraction;
        [SerializeField] private MeshRenderer _glowRenderer;
        private Color _glowBaseColor = new Color32(1, 1, 3, 255);
        private Color _draggingGlowColor = new Color(1, 1, 1*3); // Color.white;

        private void Awake()
        {
            _glowRenderer = GetComponentInChildren<MeshRenderer>(true);
        }
            
        protected override void OnStep(GameObject hitObj)
        {
            _audioSource?.PlayOneShot(_tone);
            _console?.RegisterPadStep(_padIndex);
        }

        private void Update()
        {
            float target = (WasTriggered() ? 1f : 0f);
            _glowFraction = Mathf.MoveTowards(_glowFraction, target, Time.deltaTime * 3f);
            if (_glowRenderer != null)
            {
                _glowRenderer.material.SetColor("EmissionColor", Color.Lerp(_glowBaseColor, _draggingGlowColor, _glowFraction));
            }
        }
    }
}
