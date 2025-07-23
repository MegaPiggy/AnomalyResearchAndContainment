using NewHorizons.Utility;
using System;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class PhotoTarget : VisibilityObject
    {
        [SerializeField]
        private float _maxPhotoDistance = 200f;
        [SerializeField]
        private float _raycastOffset = 10f;
        [SerializeField]
        private ShapeVisibilityTracker _shapeVisibilityTracker;
        [SerializeField]
        private RendererVisibilityTracker _rendererVisibilityTracker;

        public event PhotoEvent OnPhotographed;

        public override void Awake()
        {
            base.Awake();
            GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
        }

        public override void Update()
        {
            base.Update();
            _rendererVisibilityTracker.transform.localEulerAngles = Vector3.zero;
            _rendererVisibilityTracker._worldRotation = _rendererVisibilityTracker.transform.rotation;
        }

        public bool CheckVisibilityFromProbe(ProbeCamera probeCamera) => base.CheckVisibilityFromProbe(probeCamera.GetOWCamera());

        public bool IsInProbeSnapshot(ProbeCamera probeCamera)
        {
            if (CheckVisibilityFromProbe(probeCamera))
            {
                Vector3 vector = transform.position - probeCamera.transform.position;
                float magnitude = vector.magnitude;
                if (magnitude > _maxPhotoDistance)
                {
                    AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("photo max distance reached");
                    return false;
                }
                if (Physics.Raycast(probeCamera.transform.position, vector.normalized, magnitude - _raycastOffset, OWLayerMask.physicalMask))
                {
                    AnomalyResearchAndContainment.Instance.ModHelper.Console.WriteLine("photo blocked");
                    return false;
                }
                return true;
            }
            return false;
        }

        private void OnProbeSnapshot(ProbeCamera probeCamera)
        {
            if (IsProbeInSnapshot(probeCamera))
            {
                if (OnPhotographed != null)
                {
                    OnPhotographed(this);
                }
            }
        }

        private new void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _raycastOffset);
        }

        public delegate void PhotoEvent(PhotoTarget target);
    }

}