using NewHorizons.Components.Props;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class KeyDropper : MonoBehaviour
    {
        [Header("Pedestal Animation")]
        [SerializeField] private Transform _pedestal;
        [SerializeField] private Transform _raisedPosition;
        [SerializeField] private float _raiseDuration = 2f;

        private OWItem _keyObject;
        private Vector3 _startPosition;
        private bool _isRaising = false;
        private float _raiseStartTime;

        public void Start()
        {
            _keyObject = GetComponentInChildren<OWItem>(true);
            if (_keyObject != null)
                _keyObject.gameObject.SetActive(false);

            if (_pedestal != null)
                _startPosition = _pedestal.localPosition;
        }

        public void DropKey()
        {
            // Show key
            if (_keyObject != null)
            {
                _keyObject.gameObject.SetActive(true);
            }

            // Start pedestal animation
            if (_pedestal != null && _raisedPosition != null)
            {
                _raiseStartTime = Time.time;
                _isRaising = true;
            }
        }

        private void Update()
        {
            if (_isRaising && _pedestal != null && _raisedPosition != null)
            {
                float t = (Time.time - _raiseStartTime) / _raiseDuration;
                t = Mathf.SmoothStep(0f, 1f, t);

                _pedestal.localPosition = Vector3.Lerp(_startPosition, _raisedPosition.localPosition, t);

                if (t >= 1f)
                {
                    _pedestal.localPosition = _raisedPosition.localPosition;
                    _isRaising = false;
                }
            }
        }
    }
}