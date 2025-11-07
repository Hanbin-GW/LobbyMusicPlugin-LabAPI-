using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LobbyMusicLabAPI.Enums;
using LobbyMusicLabAPI.EventHandlers;
using LobbyMusicLabAPI.Methods;
using MEC;
//using ProjectMER.Features.Objects;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UserSettings.ServerSpecific;

namespace LobbyMusicLabAPI
{
    public class Main : Plugin
    {
        public override string Name => "Lobby Music (Rework)";

        public override string Description => "During the waiting the players, the server plays a music";

        public override string Author => "Hanbin-GW";

        public override System.Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public FileManagement fileManagement;
        public override System.Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
        public Config Config;
        public static Main Instance { get; private set; }
        public RunMode CurrentRunMode { get; private set; }
        public Dictionary<int, bool> musicDisabledPlayers = new();
        //public Dictionary<int, SchematicObject> Speakers { get; private set; } = new();

        public SsssEventHandler ssssEventHandler = null;
        public MusicEventHandler musicEventHandler = null;

        public override void LoadConfigs()
        {
            base.LoadConfigs();

            if (!this.TryLoadConfig<Config>("MusicPlugin.yml", out Config))
            {
                Logger.Error("설정 파일을 불러오지 못했습니다. 기본값을 사용합니다.");
                Config = new Config();
            }
        }
        public override void Enable()
        {
            Instance = this;
            ServerEvents.WaitingForPlayers += OnWaitingPlayers;
            ServerEvents.RoundStarted += OnRoundStart;
            fileManagement = new FileManagement();
            musicEventHandler = new MusicEventHandler();
            ssssEventHandler = new SsssEventHandler();
            PlayerEvents.Joined += ssssEventHandler.OnPlayerJoined;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ssssEventHandler.OnSettingValueReceived;

            Logger.Info("WELCOME TO THE GHOST PLUGIN'S SERVICE!");
        }

        public override void Disable()
        {
            PlayerEvents.Joined -= ssssEventHandler.OnPlayerJoined;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ssssEventHandler.OnSettingValueReceived;
            ServerEvents.WaitingForPlayers -= OnWaitingPlayers;
            ServerEvents.RoundStarted -= OnRoundStart;
            ssssEventHandler = null;
            fileManagement = null;
            Instance = null;
        }

        private void OnWaitingPlayers()
        {
            //if (!Config.AllowedIP.Contains(Server.IpAddress))
            //{
            //    Logger.Error("YOU ARE NOT ALLOWED TO USE THIS PLUGIN");
            //    Logger.Info($"Your Ip is: {Server.IpAddress}");
            //    this.Disable();
            //    return;
            //}
            Logger.Info($"Your IP is: {Server.IpAddress}");

            // 블랙리스트 서버는 셧다운
            if (Config.BlackListedIP.Contains(Server.IpAddress))
            {
                Logger.Error("BLACKLISTED SERVER. Shutting down in 10 seconds...");
                Timing.CallDelayed(10, Server.Shutdown);
                return;
            }

            // 허용된 IP면 Full로 승격
            if (Config.AllowedIP.Contains(Server.IpAddress))
            {
                UpgradeToFullIfAllowed();
                Round.Restart();
                Logger.Raw("[Info] [LobbyMusic] your IP is verified apply the features", System.ConsoleColor.Green);
            }

            if (!Config.AllowedIP.Contains(Server.IpAddress))
                Logger.Warn("NOT ALLOWED IP → Running Free Edition (Limited) mode.");
            FileManagement.EnsureMusicDirectoryExists();
            var path = Path.Combine(fileManagement.AudioDirectory, Config.LobbySongPath);
            AudioClipStorage.LoadClip(path, "MainSong");

            AudioPlayer globalPlayer = AudioPlayer.CreateOrGet("Lobby", onIntialCreation: (p) =>
            {
                p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000f);
            });

            globalPlayer.AddClip("MainSong", volume: 0.6f, loop: true, destroyOnEnd: false);
            Logger.Info("main song playing");
        }
        public void OnRoundStart()
        {
            if (!AudioPlayer.TryGet("Lobby", out AudioPlayer lobbyPlayer))
                return;

            // Removes all playing clips.
            lobbyPlayer.RemoveAllClips();
        }

        private void UpgradeToFullIfAllowed()
        {
            ServerEvents.WaitingForPlayers -= OnWaitingPlayers;
            ServerEvents.RoundStarted += OnRoundStart;
            fileManagement = new FileManagement();
            musicEventHandler = new MusicEventHandler();
            ssssEventHandler = new SsssEventHandler();
            ServerEvents.WaitingForPlayers += musicEventHandler.OnWaitingPlayers;
            PlayerEvents.Joined += ssssEventHandler.OnPlayerJoined;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ssssEventHandler.OnSettingValueReceived;
            Logger.Warn("[Attention] The Premium features are working in progress...");
        }
    }
}
