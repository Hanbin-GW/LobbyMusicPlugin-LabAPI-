using System.Collections.Generic;
using UserSettings.ServerSpecific;

namespace LobbyMusicLabAPI.SSSS;

public class ssss
{
    public static ServerSpecificSettingBase[] GetMinimalMusicSetting()
    {
        List<ServerSpecificSettingBase> settings = new List<ServerSpecificSettingBase>();

        settings.Add(new SSGroupHeader("🎵 음악 재생 설정"));
        settings.Add(new SSTwoButtonsSetting(
            Main.Instance.Config.MusicToggleId,
            "로비 및 이벤트 음악 설정",
            "듣기",
            "끄기",
            false,
            "음악 재생 여부를 설정합니다."
        ));

        return settings.ToArray();
    }

}