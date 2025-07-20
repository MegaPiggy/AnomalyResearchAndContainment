using System;
using System.Reflection;
using Epic.OnlineServices;
using HarmonyLib;
using ModJam5;
using NewHorizons.Builder.Atmosphere;
using NewHorizons.Builder.Volumes;
using NewHorizons.Handlers;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace AnomalyResearchAndContainment
{
    public class AnomalyResearchAndContainment : ModBehaviour
    {
        public static AnomalyResearchAndContainment Instance;
        public INewHorizons NewHorizons;

        public static AudioType TickUp;
        public static AudioType TickDown;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public static void LoadAudio()
        {
            TickUp = AudioTypeHandler.GetAudioType("planets/tick_up.wav", AnomalyResearchAndContainment.Instance);
            TickDown = AudioTypeHandler.GetAudioType("planets/tick_down.wav", AnomalyResearchAndContainment.Instance);
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(AnomalyResearchAndContainment)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);
            NewHorizons.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);

            new Harmony("MegaPiggy.AnomalyResearchAndContainment").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        private void OnStarSystemLoaded(string name)
        {
            if (name == ModJam5.ModJam5.SystemName)
            {
                EndTimesHandler.Initialize();
                LoadAudio();

                var star = AnomalyResearchAndContainment.Instance.NewHorizons.GetPlanet("Verdant Beacon");
                var planet = AnomalyResearchAndContainment.Instance.NewHorizons.GetPlanet("Emerald Reverie");
                var station = AnomalyResearchAndContainment.Instance.NewHorizons.GetPlanet("Anomaly Station");
                var platform = AnomalyResearchAndContainment.Instance.NewHorizons.GetPlanet("Anomaly Station Warp Platform");

                var alignment = planet.AddComponent<AlignWithTargetBody>();
                alignment.SetTargetBody(station.GetComponent<OWRigidbody>());
                alignment._localAlignmentAxis = Vector3.down;
                alignment._owRigidbody = planet.GetComponent<OWRigidbody>();
                var currentDirection = alignment.transform.TransformDirection(alignment._localAlignmentAxis);
                var targetDirection = alignment.GetAlignmentDirection();
                alignment.transform.rotation = Quaternion.FromToRotation(currentDirection, targetDirection) * alignment.transform.rotation;
                alignment._owRigidbody.SetAngularVelocity(Vector3.zero);
                alignment._usePhysicsToRotate = true;

                var materialReplacer = ModJam5.ModJam5.Instance.GetComponent<StarshipCommunityHelper>();
                materialReplacer.ReplaceMaterials(station);
                materialReplacer.ReplaceMaterials(platform);

                VolumeBuilder.MakeAndEnable<EndTimesHandler.CustomEndTimesVolume>(star, star.GetComponentInChildren<Sector>(true), new NewHorizons.External.Modules.Volumes.VolumeInfos.VolumeInfo
                {
                    radius = 2500,
                    rename = "CustomEndTimesVolume",
                    isRelativeToParent = true
                });

                var volumeController = station.GetComponentInChildren<StationVolumeController>();
                volumeController.warpReceiver = station.GetComponentInChildren<NomaiWarpReceiver>();
                volumeController.warpTransmitter = platform.GetComponentInChildren<NomaiWarpTransmitter>();
                volumeController.Initialize();

                var box = station.transform.Find("Sector/Root/HiddenRoom/Center/Floor/BoxRoot/Box");
                var door = box.transform.Find("Door");
                var interact = box.transform.Find("Button/Interact");
                interact.gameObject.AddComponent<Button>().door = door;

#if DEBUG
                ModHelper.Events.Unity.FireInNUpdates(() => DialogueConditionManager.SharedInstance.SetConditionState("AnomalyTest", true), 1000);
#endif
            }
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }
    }

}
