using UnityEngine;
using UnityEditor;
using System;

namespace Assets.scripts.util
{
    class GameSettings
    {

        public void loadAllSettings()
        {
            // Set game volume
            float volume = PlayerPrefs.GetFloat(PrefKeys.VOLUME);
            AudioListener.volume = volume;
        }

        public void setVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat(PrefKeys.VOLUME, volume);
        }

        public float getVolume()
        {
            return PlayerPrefs.GetFloat(PrefKeys.VOLUME);
        }
    }
}