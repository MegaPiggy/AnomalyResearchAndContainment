using NewHorizons.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jam5Entry
{
    public class ElevatorPad : MonoBehaviour
    {
        public event Elevator.PressInteractEvent OnPressInteractEvent;

        public enum Floor
        {
            Top,
            Bottom
        }

        [SerializeField]
        private Floor floor = Floor.Top;
        [SerializeField]
        private Transform _elevatorPad;
        [SerializeField]
        private Transform _topPoint;
        [SerializeField]
        private Transform _bottomPoint;
        [SerializeField]
        private float _duration = 3f;
        [SerializeField]
        private PlayerAttachPoint _attachPoint;
        [SerializeField]
        private SingleInteractionVolume _interactVolume;
        [SerializeField]
        private OWAudioSource _owAudioSourceLP;
        [SerializeField]
        private OWAudioSource _owAudioSourceOneShot;
        [SerializeField]
        private OWTriggerVolume[] _killTriggers;
        private int _playerKillTriggerCount;
        private float _initElevatorTime;
        private Floor targetFloor;
        private bool _busy;
        private bool _deactivated;

        public Vector3 GetFloorPos(Floor floor)
        {
            return floor == Floor.Top ? _topPoint.transform.localPosition : _bottomPoint.transform.localPosition;
        }

        public Vector3 GetElevatorPadPos()
        {
            return _elevatorPad.transform.localPosition;
        }

        public void SetElevatorPadPos(Vector3 localPosition)
        {
            _elevatorPad.transform.localPosition = localPosition;
        }

        public float GetTrackPositionFraction()
        {
            return Vector3.Distance(GetElevatorPadPos(), GetFloorPos(floor)) / Vector3.Distance(GetFloorPos(floor), GetFloorPos(targetFloor));
        }

        private void Awake()
        {
            targetFloor = floor;
            SetElevatorPadPos(GetFloorPos(floor));
            _interactVolume.OnPressInteract += OnPressInteract;
            if (_killTriggers != null)
            {
                for (int i = 0; i < _killTriggers.Length; i++)
                {
                    _killTriggers[i].OnEntry += OnEnterKillTrigger;
                    _killTriggers[i].OnExit += OnExitKillTrigger;
                }
            }
        }

        private void Start()
        {
            _owAudioSourceLP.AssignAudioLibraryClip(AudioType.CageElevator_Loop_Winch);
        }

        public void ChangePromptText(UITextType promptID, bool showKeyCommandIcon)
        {
            _interactVolume.ChangePrompt(promptID);
            _interactVolume.SetKeyCommandVisible(showKeyCommandIcon);
        }

        public void ResetInteractVolume(UITextType promptID, bool showKeyCommandIcon)
        {
            _interactVolume.SetPromptText(promptID);
            _interactVolume.SetKeyCommandVisible(showKeyCommandIcon);
        }

        public void AttachPlayer()
        {
            _attachPoint.AttachPlayer();
        }

        public void DetachPlayer()
        {
            _attachPoint.DetachPlayer();
        }

        private void OnPressInteract()
        {
            if (OnPressInteractEvent != null) OnPressInteractEvent();
            AttachPlayerAndStartElevator();
        }

        public void AttachPlayerAndStartElevator()
        {
            if (_busy || _deactivated) return;
            AttachPlayer();
            StartElevator();
        }

        public void ForceStartElevator()
        {
            StartElevator();
        }

        private void StartElevator()
        {
            if (_busy || _deactivated) return;
            _busy = true;
            enabled = true;
            targetFloor = floor == Floor.Top ? Floor.Bottom : Floor.Top;
            _initElevatorTime = Time.time;
            _owAudioSourceOneShot.PlayOneShot(AudioType.CageElevator_Start, 1f);
            _owAudioSourceLP.FadeIn(0.5f, false, false, 1f);
            _interactVolume.DisableInteraction();
            _interactVolume.ResetInteraction();
        }

        private void Update()
        {
            float t = (Time.time - _initElevatorTime) / _duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            Vector3 position = Vector3.Lerp(GetFloorPos(floor), GetFloorPos(targetFloor), t);
            if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
            {
                Jam5Entry.Instance.ModHelper.Console.WriteLine(nameof(ElevatorPad) + ".Update() in " + gameObject.name + " encountered Infinity value", OWML.Common.MessageType.Warning);
                return;
            }
            if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
            {
                Jam5Entry.Instance.ModHelper.Console.WriteLine(nameof(ElevatorPad) + ".Update() in " + gameObject.name + " encountered NaN value", OWML.Common.MessageType.Warning);
                return;
            }
            SetElevatorPadPos(position);
            if (t >= 1f)
            {
                floor = targetFloor;
                enabled = false;
                _busy = false;
                DetachPlayer();
                if (_interactVolume != null)
                {
                    _interactVolume.ResetInteraction();
                    _interactVolume.EnableInteraction();
                }
                _owAudioSourceLP.FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP, 0f);
                _owAudioSourceOneShot.PlayOneShot(AudioType.CageElevator_End, 1f);
            }
        }

        private void OnEnterKillTrigger(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                _playerKillTriggerCount++;
                if (_playerKillTriggerCount == _killTriggers.Length)
                {
                    Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);
                    RumbleManager.PlayerCrushedByElevator();
                }
            }
        }
        private void OnExitKillTrigger(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                _playerKillTriggerCount--;
            }
        }

        public void EnableElevator()
        {
            _deactivated = false;
            ChangePromptText((UITextType)TranslationHandler.AddUI("Use Elevator", false), true);
        }

        public void DisableElevator()
        {
            _deactivated = true;
            ChangePromptText((UITextType)TranslationHandler.AddUI("Elevator Not Activated", false), false);
        }
    }
}
