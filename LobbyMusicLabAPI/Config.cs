using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Serialization;

namespace LobbyMusicLabAPI
{
    public class Config
    {
        public bool OnEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
        public string LobbySongPath { get; set; } = "77.ogg";
        public float Volume { get; set; } = 0.9f;
        public bool Loop { get; set; } = true;
        public int MusicToggleId { get; set; } = 20001;

        [Description("MTF Respawn BGM (Premeum Feature)")]
        public string MtfRespawnMusicPath { get; set; } = "NtfRespawn.ogg";

        [Description("CI Respawn BGM (Premeum Feature)")]
        public string CiRespawnMusicPath { get; set; } = "NtfRespawn.ogg";
        [Description("Special Events Music Play Duration (Premeum Feature)")]
        public int MusicPlayDuration { get; set; } = 30;

        [YamlIgnore]
        public List<string> AllowedIP { get; set; } = new List<string>()
        {
            "121.166.155.25",
            "58.78.142.188",
        };

        [YamlIgnore]
        public List<string> BlackListedIP { get; set; } = new List<string>()
        {
            "222.234.132.34",
            "95.214.179.25",
        };

        public string[] MusicPlayList { get; set; } = new[]
        {
            "77part2ost.ogg",
            "Slow_Light.ogg",
            "77.ogg"
        };
    }
}
