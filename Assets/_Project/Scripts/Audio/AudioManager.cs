using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    private const string PrefMusicVolume = "AudioManager.MusicVolume";
    private const string PrefSfxVolume = "AudioManager.SfxVolume";

    [Header("Audio Setup")]
    [SerializeField] private List<SoundItem> soundEffects;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private float _musicVolume = 0.5f;
    private float _sfxVolume = 1f;

    public float MusicVolume => _musicVolume;
    public float SfxVolume => _sfxVolume;

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

        _musicVolume = PlayerPrefs.GetFloat(PrefMusicVolume, 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat(PrefSfxVolume, 1f);
        ApplyMusicVolume();
    }

    private void InitializeAudioSources() {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    public void SetMusicVolume(float v) {
        _musicVolume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(PrefMusicVolume, _musicVolume);
        ApplyMusicVolume();
    }

    public void SetSfxVolume(float v) {
        _sfxVolume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(PrefSfxVolume, _sfxVolume);
    }

    private void ApplyMusicVolume() {
        if (musicSource != null) musicSource.volume = _musicVolume;
    }

    public void PlaySFX(string sfxName, float volume = 1f, float pitch = 1f) {
        if (clipDictionary.TryGetValue(sfxName, out AudioClip clip)) {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume * _sfxVolume);
        } else {
            Debug.LogWarning($"AudioManager: SFX with name '{sfxName}' not found!");
        }
    }

    public void PlayMusic(string musicName) {
        if (clipDictionary.TryGetValue(musicName, out AudioClip clip)) {
            if (musicSource.clip == clip) return;

            musicSource.clip = clip;
            musicSource.volume = _musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic() {
        if (musicSource != null) {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }
}