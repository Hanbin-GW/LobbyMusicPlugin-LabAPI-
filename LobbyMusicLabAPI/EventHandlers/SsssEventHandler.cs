using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System;
using LobbyMusicLabAPI.SSSS;
using UserSettings.ServerSpecific;
using LabApi.Events.Arguments.PlayerEvents;

namespace LobbyMusicLabAPI.EventHandlers
{
    public class SsssEventHandler
    {
        public void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            if (Main.Instance.ssssEventHandler == null)
                return;
            //if (!Round.IsRoundStarted)
            if (true)
            {
                try
                {
                    ServerSpecificSettingsSync.DefinedSettings = ssss.GetMinimalMusicSetting();
                    ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error($"Error: InvalidCastException occurred: {ex.Message}");
                }
            }
        }
        public void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (Main.Instance == null)
            {
                Logger.Error("Instance is null");
                return;
            }



            if (hub == null)
                return;
            var player = Player.Get(hub);
            var playerkey = hub.PlayerId;

            if (settingBase is SSTwoButtonsSetting musicSetting &&
                musicSetting.SettingId == Main.Instance.Config.MusicToggleId)
            {

                if (musicSetting.SyncIsA)
                {
                    Main.Instance.musicDisabledPlayers[playerkey] = false;
                    player.SendHint("<color=green>🎵 Turn <b>on</b> the music</color>", 2f);
                }
                else if (musicSetting.SyncIsB)
                {
                    Main.Instance.musicDisabledPlayers[playerkey] = true;
                    player.SendHint("<color=red>🔇 Turn <b>off</b> the music</color>", 2f);
                }
                return;
            }
        }

    }
}
