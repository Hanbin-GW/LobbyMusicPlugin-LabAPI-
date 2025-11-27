using System.Collections.Generic;
using UserSettings.ServerSpecific;

namespace LobbyMusicLabAPI.SSSS;

public class ssss
{
    public static ServerSpecificSettingBase[] GetMinimalMusicSetting()
    {
        List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();

        settings.Add(new SSGroupHeader("🎵 Music playback settings"));
        settings.Add(new SSTwoButtonsSetting(
            Main.Instance.Config.MusicToggleId,
            "Lobby Music Settings",
            "On",
            "Off",
            false,
            "Set whether to play music."
        ));

        return settings.ToArray();
    }

}