using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class SoundManager : MonoBehaviour
    {
        public enum Sound
        {
            PlayerShoot,
            PlayerHit,
            GameWin,
            GameLose
        }

        public AudioSource audioSource;
        public AudioClip playerShootSound;
        public AudioClip playerHitSound;
        public AudioClip gameWinSound;
        public AudioClip gameLoseSound;

        private Dictionary<Sound, AudioClip> dicSounds;

        static SoundManager instance;
        static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SoundManager>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
            this.Init();
        }

        private void Init()
        {
            this.dicSounds = new Dictionary<Sound, AudioClip>()
            {
                {Sound.PlayerHit, this.playerHitSound },
                {Sound.PlayerShoot, this.playerShootSound },
                {Sound.GameWin, this.gameWinSound },
                {Sound.GameLose, this.gameLoseSound },
            };
        }

        private void PlaySound(Sound soundType)
        {
            if (this.dicSounds.TryGetValue(soundType, out var clip))
            {
                this.audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogError($"No audio clip found for sound {soundType}");
            }
        }

        public static void PlayBackgroundMusic(Sound soundType)
        {
            if (Instance.dicSounds.TryGetValue(soundType, out var clip))
            {
                Instance.audioSource.clip = clip;
                Instance.audioSource.Play();
            }
            else
            {
                Debug.LogError($"No audio clip found for sound {soundType}");
            }
        }

        public static void Play(Sound soundType)
        {
            Instance.PlaySound(soundType);
        }
    }
}