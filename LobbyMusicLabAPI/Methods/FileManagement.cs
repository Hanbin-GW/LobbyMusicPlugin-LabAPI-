using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyMusicLabAPI.Methods
{
    public class FileManagement
    {
        public readonly string AudioDirectory;
        public FileManagement()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AudioDirectory = Path.Combine(appDataPath, "SCP Secret Laboratory", "LabAPI", "audio");
        }

        public static void EnsureMusicDirectoryExists()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory", "LabAPI", "audio");

            // 폴더가 없으면 생성
            if (!Directory.Exists(path))
            {
                Logger.Error($"Music Folder didn't exist create new: {path}");
                Directory.CreateDirectory(path);  // 폴더 생성
            }
            else
            {
                Logger.Info("Music folder is already exisits.");
            }
        }
    }
}
