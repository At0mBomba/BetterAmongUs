using BetterAmongUs.Data;
using BetterAmongUs.Helpers;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Manages audio override files and their corresponding AudioClip data.
/// </summary>
internal static class AudioOverrideManager
{
    /// <summary>
    /// Collection of all discovered audio override data instances.
    /// </summary>
    internal static readonly List<AudioOverrideData> Overrides = [];

    internal static readonly AudioOverrideData Music_MainMenuSong = new("Music/MainMenuSong.wav");

    internal static readonly AudioOverrideData Music_LobbySong = new("Music/LobbySong.wav");

    internal static readonly AudioOverrideData Sounds_EmergencyMeeting = new("Sounds/Meeting/EmergencyMeeting.wav");

    /// <summary>
    /// Initializes the AudioOverrideManager by discovering all AudioOverrideData fields,
    /// </summary>
    internal static void Initialize()
    {
        // Discover all AudioOverrideInfo fields
        var infoFields = typeof(AudioOverrideManager)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType == typeof(AudioOverrideData))
            .Select(f => f.GetValue(null) as AudioOverrideData)
            .Where(v => v != null);

        Overrides.Clear();
        foreach (var overrideData in infoFields)
        {
            if (overrideData != null)
            {
                Overrides.Add(overrideData);
            }
        }

        foreach (var overrideInfo in Overrides)
        {
            overrideInfo.Load();
        }
    }

    /// <summary>
    /// Represents an audio override file with its path and loaded AudioClip.
    /// </summary>
    /// <param name="path">The relative path to the audio file within the audio overrides folder.</param>
    internal sealed class AudioOverrideData(string path)
    {
        private readonly string _path = path;
        private AudioClip? audioClip;

        /// <summary>
        /// Gets or sets the original audio clip associated with this instance.
        /// </summary>
        internal AudioClip? Original;

        /// <summary>
        /// Loads the audio clip from disk. Creates the file and directory structure if they don't exist.
        /// </summary>
        internal void Load()
        {
            if (string.IsNullOrEmpty(_path)) return;

            var fullPath = Path.Combine(BetterDataManager.Folders.audioOverridesFolderPath, _path);
            if (!Directory.Exists(fullPath))
            {
                string? directoryPath = Path.GetDirectoryName(fullPath);
                if (directoryPath == null) return;

                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();
            }
            audioClip = Utils.LoadWavFromDisk(fullPath);
            if (audioClip != null)
            {
                audioClip.name = "Override_" + Path.GetFileNameWithoutExtension(_path);
            }
        }

        /// <summary>
        /// Attempts to retrieve the loaded AudioClip.
        /// </summary>
        /// <param name="clip">The AudioClip if loaded and available; otherwise, null.</param>
        /// <returns>True if the AudioClip is available; otherwise, false.</returns>
        internal bool TryGetAudioClip(out AudioClip? clip)
        {
            if (audioClip != null)
            {
                clip = audioClip;
                return true;
            }

            clip = null;
            return false;
        }
    }
}