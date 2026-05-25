using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [Header("Audio Setup")]
    [SerializeField] private List<SoundItem> soundEffects;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private Dictionary<string, AudioClip> clipDictionary;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        } else {
            Destroy(gameObject);
            return;
        }

        clipDictionary = new Dictionary<string, AudioClip>();
        foreach (var sfx in soundEffects) {
            if (!clipDictionary.ContainsKey(sfx.name)) {
                clipDictionary.Add(sfx.name, sfx.clip);
            }
        }
    }

    private void InitializeAudioSources() {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    public void PlaySFX(string sfxName, float volume = 1f, float pitch = 1f) {
        if (clipDictionary.TryGetValue(sfxName, out AudioClip clip)) {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
        } else {
            Debug.LogWarning($"AudioManager: SFX with name '{sfxName}' not found!");
        }
    }

    public void PlayMusic(string musicName, float volume = 0.5f) {
        if (clipDictionary.TryGetValue(musicName, out AudioClip clip)) {
            if (musicSource.clip == clip) return;

            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }
    }
}