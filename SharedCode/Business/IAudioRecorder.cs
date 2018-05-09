using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutoScribeClient.Business
{
    /// <summary>
    /// Interface of an audio recorder, implemented by the platform specific classes.
    /// </summary>
    public interface IAudioRecorder
    {
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
