using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LobbyMusicLabAPI.Methods;
using System;
using System.IO;
using System.Security;

namespace LobbyMusicLabAPI
{
    public class Main : Plugin
    {
        public override string Name => "Lobby Music (Rework)";

        public override string Description => "During the waiting the players, the server plays a music";

        public override string Author => "Hanbin-GW";

        public override Version Version => new Version(0,1,0);
        public FileManagement fileManagement;
        public override Version RequiredApiVersion => throw new NotImplementedException();
        public Config Config;
        public override void LoadConfigs()
        {
            base.LoadConfigs();

            if (!this.TryLoadConfig<Config>("ClassicPlugin.yml", out Config))
            {
                Logger.Error("설정 파일을 불러오지 못했습니다. 기본값을 사용합니다.");
                Config = new Config();
            }
        }
        public override void Enable()
        {
            ServerEvents.WaitingForPlayers += OnWaitingPlayers;
            ServerEvents.RoundStarted += OnRoundStart;
            fileManagement = new FileManagement();
            Logger.Info("WELCOME TO THE GHOST PLUGIN'S SERVICE!");
        }

        public override void Disable()
        {
            ServerEvents.WaitingForPlayers -= OnWaitingPlayers;
            ServerEvents.RoundStarted -= OnRoundStart;
            fileManagement = null;
        }

        private void OnWaitingPlayers()
        {
            if (!Config.AllowedIP.Contains(Server.IpAddress))
            {
                Logger.Error("YOU ARE NOT ALLOWED TO USE THIS PLUGIN");
                Logger.Info($"Your Ip is: {Server.IpAddress}");
                this.Disable();
                return;
            }
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
    }
}
