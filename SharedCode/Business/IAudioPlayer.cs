using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoScribeClient.Business
{
    /// <summary>
    /// Interface of an audio player implemented by the platform specific classes.
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// EventHandler when audio playing is ended.
        /// </summary>
        event EventHandler MediaEnded;

        /// <summary>
        /// Play the specified audio
        /// </summary>
        /// <param name="audio">The audio</param>
        void PlayAudio(Stream audio);
        /// <summary>
        /// Pause the audio playback.
        /// </summary>
        void PauseAudio();
        /// <summary>
        /// Continue the audio playback.
        /// </summary>
        void ContinueAudio();
        /// <summary>
        /// Stop the audio playback.
        /// </summary>
        void StopAudio();
    }
}
