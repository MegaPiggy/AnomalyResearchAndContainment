using System;
using UnityEngine;

namespace Jam5Entry
{
    public class PhotoTarget : VisibilityObject
    {
        [SerializeField]
        private float _maxPhotoDistance = 200f;
        [SerializeField]
        private float _raycastOffset = 10f;

        public event PhotoEvent OnPhotographed;

        public override void Awake()
        {
            base.Awake();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
        }

        public override void OnSectorOccupantAdded(SectorDetector sectorDetector)
        {
            if (sectorDetector.GetOccupantType() == DynamicOccupant.Probe)
            {
                GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
            }
        }

        public override void OnSectorOccupantRemoved(SectorDetector sectorDetector)
        {
            if (sectorDetector.GetOccupantType() == DynamicOccupant.Probe)
            {
                GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
            }
        }

        private void OnProbeSnapshot(ProbeCamera camera)
        {
            Jam5Entry.Instance.ModHelper.Console.WriteLine("probe snapshot");
            if (CheckVisibilityFromProbe(camera.GetOWCamera()))
            {
                Vector3 vector = transform.position - camera.transform.position;
                float magnitude = vector.magnitude;
                if (magnitude > _maxPhotoDistance)
                {
                    Jam5Entry.Instance.ModHelper.Console.WriteLine("photo max distance reached");
                    return;
                }
                if (Physics.Raycast(camera.transform.position, vector.normalized, magnitude - _raycastOffset, OWLayerMask.physicalMask))
                {
                    Jam5Entry.Instance.ModHelper.Console.WriteLine("photo blocked");
                    return;
                }
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