﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScoreSaber.Core.Data
{
    internal class Settings
    {
        private static int _currentVersion => 6;

        public bool hideReplayUI = false;

        public int fileVersion { get; set; }
        public bool disableScoreSaber { get; set; }
        public bool showLocalPlayerRank { get; set; }
        public bool showScorePP { get; set; }
        public bool showStatusText { get; set; }
        public bool saveLocalReplays { get; set; }
        public bool enableCountryLeaderboards { get; set; }
        public float replayCameraFOV { get; set; }
        public float replayCameraXOffset { get; set; }
        public float replayCameraYOffset { get; set; }
        public float replayCameraZOffset { get; set; }
        public float replayCameraXRotation { get; set; }
        public float replayCameraYRotation { get; set; }
        public float replayCameraZRotation { get; set; }
        public bool enableReplayFrameRenderer { get; set; }
        public string replayFramePath { get; set; }
        public bool hideNAScoresFromLeaderboard { get; set; }
        public bool hasClickedScoreSaberLogo { get; set; }
        public bool hasOpenedReplayUI { get; set; }
        public bool leftHandedReplayUI { get; set; }
        public bool lockedReplayUIMode { get; set; }
        public List<SpectatorPoseRoot> spectatorPositions { get; set; }

        internal static string dataPath => "UserData";
        internal static string configPath => dataPath + @"\ScoreSaber";
        internal static string replayPath => configPath + @"\Replays";

        public void SetDefaults() {

            disableScoreSaber = false;
            showLocalPlayerRank = true;
            showScorePP = true;
            showStatusText = true;
            saveLocalReplays = true;
            enableCountryLeaderboards = true;
            replayCameraFOV = 70f;
            replayCameraXOffset = 0.0f;
            replayCameraYOffset = 0.0f;
            replayCameraZOffset = 0.0f;
            replayCameraXRotation = 0.0f;
            replayCameraYRotation = 0.0f;
            replayCameraZRotation = 0.0f;
            enableReplayFrameRenderer = false;
            replayFramePath = "Z:\\Example\\Directory\\";
            hideNAScoresFromLeaderboard = false;
            hasClickedScoreSaberLogo = false;
            hasOpenedReplayUI = false;
            leftHandedReplayUI = false;
            lockedReplayUIMode = false;
            SetDefaultSpectatorPositions();
        }

        public void SetDefaultSpectatorPositions() {

            spectatorPositions = new List<SpectatorPoseRoot>();
            spectatorPositions.Add(new SpectatorPoseRoot(new SpectatorPose(new Vector3(0f, 0f, -2f)), "Main"));
            spectatorPositions.Add(new SpectatorPoseRoot(new SpectatorPose(new Vector3(0f, 4f, 0f)), "Bird's Eye"));
            spectatorPositions.Add(new SpectatorPoseRoot(new SpectatorPose(new Vector3(-3f, 0f, 0f)), "Left"));
            spectatorPositions.Add(new SpectatorPoseRoot(new SpectatorPose(new Vector3(3f, 0f, 0f)), "Right"));
        }

        internal static Settings LoadSettings() {

            try {
                if (!Directory.Exists(dataPath)) {
                    Directory.CreateDirectory(dataPath);
                }

                if (!Directory.Exists(configPath)) {
                    Directory.CreateDirectory(configPath);
                }

                if (!Directory.Exists(replayPath)) {
                    Directory.CreateDirectory(replayPath);
                }

                if (!File.Exists(configPath + @"\ScoreSaber.json")) {
                    Settings settings = new Settings();
                    settings.SetDefaults();
                    SaveSettings(settings);
                    return settings;
                }

                Settings decoded = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configPath + @"\ScoreSaber.json"));

                //Upgrade settings if old
                if (decoded.fileVersion < _currentVersion) {
                    if (decoded.spectatorPositions == null) {
                        decoded.SetDefaultSpectatorPositions();
                    }
                    SaveSettings(decoded);
                }
                return decoded;
            } catch (Exception ex) {
                Plugin.Log.Error("Failed to load settings " + ex.ToString());
                return new Settings();
            }
        }

        internal static void SaveSettings(Settings settings) {

            try {
                settings.fileVersion = _currentVersion;
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                serializerSettings.Formatting = Formatting.Indented;
                string serialized = JsonConvert.SerializeObject(settings, serializerSettings);
                File.WriteAllText(configPath + @"\ScoreSaber.json", serialized);
            } catch (Exception ex) {
                Plugin.Log.Error("Failed to save settings " + ex.ToString());
            }
        }

        internal struct SpectatorPoseRoot {
            [JsonProperty("name")]
            internal string name { get; set; }
            [JsonProperty("spectatorPose")]
            internal SpectatorPose spectatorPose { get; set; }

            internal SpectatorPoseRoot(SpectatorPose spectatorPose, string name) {
                this.name = name;
                this.spectatorPose = spectatorPose;
            }
        }

        internal struct SpectatorPose {
            [JsonProperty("x")]
            internal float x { get; set; }
            [JsonProperty("y")]
            internal float y { get; set; }
            [JsonProperty("z")]
            internal float z { get; set; }

            internal SpectatorPose(Vector3 position) {
                x = position.x;
                y = position.y;
                z = position.z;
            }

            internal Vector3 ToVector3() {
                return new Vector3(x, y, z);
            }
        }
    }
}