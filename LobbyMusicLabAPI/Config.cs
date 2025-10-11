using System.Collections.Generic;
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

        [YamlIgnore]
        public List<string> AllowedIP { get; set; } = new List<string>()
        {
            "121.166.155.25",
        };
    }
}
