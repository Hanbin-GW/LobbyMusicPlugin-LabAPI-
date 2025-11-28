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
            id: Main.Instance.Config.MusicToggleId,
            label: "Lobby Music Settings",
            optionA: "On",
            optionB: "Off",
            defaultIsB: false,
            hint: "Set whether to play music.",
            collectionId: byte.MaxValue,
            isServerOnly: true
        ));

        return settings.ToArray();
    }

}