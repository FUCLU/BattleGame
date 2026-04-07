using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace BattleGame.Client.Managers
{
    internal class SoundManager
    {
        private static AudioFileReader? _audioFile;
        private static WaveOutEvent? _waveOut;

        public static void PlayBGM(string fileName)
        {
            StopBGM(); 

            string path = Path.Combine(Application.StartupPath, "Assets", "Sounds", "BGM", fileName);

            _audioFile = new AudioFileReader(path);
            _waveOut = new WaveOutEvent();
            _waveOut.Init(_audioFile);
            _waveOut.PlaybackStopped += (s, e) =>
            {
                _audioFile.Position = 0;
                _waveOut.Play();
            };
            _waveOut.Play();
        }

        public static void StopBGM()
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _audioFile?.Dispose();
            _waveOut = null;
            _audioFile = null;
        }

        public static void SetVolume(float volume) 
        {
            if (_audioFile != null)
                _audioFile.Volume = volume;
        }
    }
}
