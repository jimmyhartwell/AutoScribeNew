using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoScribeClient.Business
{
    public interface IAudioController
    {
        /// <summary>
        /// Register a handler (in AudioViewModel) for MediaEnded event.
        /// </summary>
        /// <param name="handler">Event handler to register.</param>
        void RegisterMediaEndedHandler(EventHandler handler);

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

        /// <summary>
        /// Start recording a new audio.
        /// </summary>
        Task StartRecording();

        /// <summary>
        /// Pause the recording.
        /// </summary>
        void PauseRecording();

        /// <summary>
        /// Continue the recording.
        /// </summary>
        void ContinueRecording();

        /// <summary>
        /// Stop the recording and get the created audio.
        /// </summary>
        /// <returns>The recorded audio</returns>
        Stream StopRecording();
    }
}
