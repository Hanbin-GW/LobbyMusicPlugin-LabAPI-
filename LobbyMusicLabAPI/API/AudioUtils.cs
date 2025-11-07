using NVorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyMusicLabAPI.API
{
    public class AudioUtils
    {
        public static float GetOggDurationInSeconds(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("오디오 파일을 찾을 수 없습니다.", filePath);
            }

            using (var stream = File.OpenRead(filePath))
            using (var vorbis = new VorbisReader(stream, false))
            {
                double duration = vorbis.TotalTime.TotalSeconds;
                return (float)duration;
            }
        }
    }
}
