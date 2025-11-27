using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LobbyMusicLabAPI.Addons;
using LobbyMusicLabAPI.Enums;
using LobbyMusicLabAPI.EventHandlers;
using LobbyMusicLabAPI.Methods;
using MEC;
using System;

//using ProjectMER.Features.Objects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        //public MusicEventHandler musicEventHandler = null;
        //public PaidFeatures paidFeatures = null;
        //public KillSoundEffects killSoundEffects = null;

        private IPremiumAddon _premiumAddon;
        private void TryLoadPremiumAddon()
        {
            try
            {
                // 이 플러그인 DLL이 있는 폴더 경로
                string pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string premiumPath = Path.Combine(pluginDir, "LobbyMusicPremium.dll"); // 프리미엄 DLL 이름

                if (!File.Exists(premiumPath))
                {
                    Console.WriteLine("[LobbyMusic] Premium DLL not found, running free version.");
                    return;
                }

                var asm = Assembly.LoadFrom(premiumPath);

                // IPremiumAddon 구현한 타입 하나 찾기
                var addonType = asm.GetTypes()
                    .FirstOrDefault(t =>
                        typeof(IPremiumAddon).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        t.GetConstructor(Type.EmptyTypes) != null);

                if (addonType == null)
                {
                    Console.WriteLine("[LobbyMusic] Premium DLL found, but no IPremiumAddon implementation.");
                    return;
                }

                _premiumAddon = (IPremiumAddon)Activator.CreateInstance(addonType);
                _premiumAddon.Register();

                Console.WriteLine("[LobbyMusic] Premium addon loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[LobbyMusic] Failed to load premium addon: " + ex);
                _premiumAddon = null;
            }
        }

        public override void LoadConfigs()
        {
            base.LoadConfigs();

            if (!this.TryLoadConfig<Config>("MusicPlugin.yml", out Config))
            {
                Logger.Error("Cannot Create ConfigFile...Create a default one.");
                Config = new Config();
            }
        }
        public override void Enable()
        {
            Instance = this;
            

            RegisterCoreEvents();

            TryLoadPremiumAddon();


            //fileManagement = new FileManagement();
            //musicEventHandler = new MusicEventHandler();
            ssssEventHandler = new SsssEventHandler();
            //killSoundEffects = new KillSoundEffects();
            //ServerSpecificSettingsSync.ServerOnSettingValueReceived += ssssEventHandler.OnSettingValueReceived;
            MusicMethods.EnsureMusicDirectoryExists();
            Logger.Info("WELCOME TO THE GHOST PLUGIN'S SERVICE!");
        }

        public override void Disable()
        {
            //PlayerEvents.Joined -= ssssEventHandler.OnPlayerJoined;
            //ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ssssEventHandler.OnSettingValueReceived;
            //ServerEvents.WaitingForPlayers -= OnWaitingPlayers;
            //ServerEvents.RoundStarted -= OnRoundStart;
            //ServerEvents.WaitingForPlayers -= musicEventHandler.OnWaitingPlayers;
            //ServerEvents.WaveRespawning -= paidFeatures.OnRespawningTeam;
            //ServerEvents.RoundStarting -= killSoundEffects.OnRoundStarting;
            //ServerEvents.RoundEnded -= killSoundEffects.OnRoundEnded;
            //ObjectiveEvents.KilledEnemyCompleted -= killSoundEffects.OnEnemyKilledObjective;

            //paidFeatures = null;
            //fileManagement = null;
            //killSoundEffects = null;

            if (_premiumAddon != null)
            {
                try { _premiumAddon.Unregister(); }
                catch (Exception ex)
                {
                    Console.WriteLine("[LobbyMusic] Premium Unregister error: " + ex);
                }

                _premiumAddon = null;
            }

            UnregisterCoreEvents();
            ssssEventHandler = null;
            Instance = null;
        }

        private void RegisterCoreEvents()
        {
            ServerEvents.WaitingForPlayers += OnWaitingPlayers;
            ServerEvents.RoundStarted += OnRoundStart;
            PlayerEvents.Joined += ssssEventHandler.OnPlayerJoined;
        }

        // ★ Disable()에 있던 해제 코드도 여기로.
        private void UnregisterCoreEvents()
        {
            ServerEvents.WaitingForPlayers -= OnWaitingPlayers;
            ServerEvents.RoundStarted -= OnRoundStart;
            PlayerEvents.Joined -= ssssEventHandler.OnPlayerJoined;
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

            // I did not support my blacklists servers
            if (Config.BlackListedIP.Contains(Server.IpAddress))
            {
                Logger.Error("BLACKLISTED SERVER. Shutting down in 10 seconds...");
                Timing.CallDelayed(10, Server.Shutdown);
                return;
            }

            /*if (Config.AllowedIP.Contains(Server.IpAddress))
            {
                Logger.Raw("[Notice] NW wants to make 'open-source', So next update, private events handlers will be remove.", System.ConsoleColor.DarkRed);
                UpgradeToFullIfAllowed();
                Round.Restart();
                Logger.Raw("[Info] [LobbyMusic] your IP is verified apply the features", System.ConsoleColor.Green);
            }*/

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
            
            //fileManagement = new FileManagement();
            //musicEventHandler = new MusicEventHandler();
            //ssssEventHandler = new SsssEventHandler();
            //paidFeatures = new PaidFeatures();
            //killSoundEffects = new KillSoundEffects();

            //ServerEvents.RoundStarting += killSoundEffects.OnRoundStarting;
            //ServerEvents.RoundEnded += killSoundEffects.OnRoundEnded;
            //ObjectiveEvents.KilledEnemyCompleted += killSoundEffects.OnEnemyKilledObjective;

            //ServerEvents.WaitingForPlayers += musicEventHandler.OnWaitingPlayers;
            //PlayerEvents.Joined += ssssEventHandler.OnPlayerJoined;
            //ServerEvents.WaveRespawning += paidFeatures.OnRespawningTeam;
            //ServerSpecificSettingsSync.ServerOnSettingValueReceived += ssssEventHandler.OnSettingValueReceived;
            Logger.Warn("[Attention] The Premium features are working in progress...");
        }
    }
}
