using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyMusicLabAPI.Addons
{
    public interface IPremiumAddon
    {
        void Register();
        void Unregister();
    }

    public interface IPremiumContext
    {
        string MusicFolderPath { get; }
        void StartLobbyMusic(string path, float volume);
        void StopLobbyMusic();
    }
}
