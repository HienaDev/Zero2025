using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source Pool")]
    [SerializeField] private int poolSize = 10;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private Dictionary<AudioClip, AudioSource> loopingSounds = new Dictionary<AudioClip, AudioSource>();

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioManager()
    {
        // Create audio source pool
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(source);
        }
    }

    // Play one-shot sound (fire and forget)
    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        AudioSource source = GetAvailableAudioSource();

        if (source != null)
        {
            source.PlayOneShot(clip, volume);
        }
    }

    // Play sound with loop option
    public void Play(AudioClip clip, bool loop = false, float volume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        // If already playing and looping, don't restart
        if (loop && loopingSounds.ContainsKey(clip))
        {
            AudioSource existingSource = loopingSounds[clip];
            if (existingSource != null && existingSource.isPlaying)
            {
                return;
            }
        }

        AudioSource source = GetAvailableAudioSource();

        if (source != null)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            source.Play();

            // Track looping sounds
            if (loop)
            {
                if (loopingSounds.ContainsKey(clip))
                {
                    loopingSounds[clip] = source;
                }
                else
                {
                    loopingSounds.Add(clip, source);
                }
            }
        }
    }

    // Stop a specific audio clip
    public void Stop(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        // Check looping sounds
        if (loopingSounds.ContainsKey(clip))
        {
            AudioSource source = loopingSounds[clip];
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
            loopingSounds.Remove(clip);
        }

        // Check all sources in pool
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.clip == clip && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    // Stop all sounds
    public void StopAll()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }

        loopingSounds.Clear();
    }

    // Pause a specific audio clip
    public void Pause(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        if (loopingSounds.ContainsKey(clip))
        {
            AudioSource source = loopingSounds[clip];
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    // Resume a specific audio clip
    public void Resume(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        if (loopingSounds.ContainsKey(clip))
        {
            AudioSource source = loopingSounds[clip];
            if (source != null)
            {
                source.UnPause();
            }
        }
    }

    // Check if a clip is playing
    public bool IsPlaying(AudioClip clip)
    {
        if (clip == null)
        {
            return false;
        }

        if (loopingSounds.ContainsKey(clip))
        {
            AudioSource source = loopingSounds[clip];
            return source != null && source.isPlaying;
        }

        foreach (AudioSource source in audioSourcePool)
        {
            if (source.clip == clip && source.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    // Set volume for a specific clip
    public void SetVolume(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        volume = Mathf.Clamp01(volume);

        if (loopingSounds.ContainsKey(clip))
        {
            AudioSource source = loopingSounds[clip];
            if (source != null)
            {
                source.volume = volume;
            }
        }
    }

    // Set pitch for a specific clip
    public void SetPitch(AudioClip clip, float pitch)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        if (loopingSounds.ContainsKey(clip))
        {
            AudioSource source = loopingSounds[clip];
            if (source != null)
            {
                source.pitch = pitch;
            }
        }
    }

    // Fade in a sound
    public void FadeIn(AudioClip clip, float duration, float targetVolume = 1f, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        if (!IsPlaying(clip))
        {
            Play(clip, loop, 0f);
        }

        AudioSource source = GetSourcePlayingClip(clip);
        if (source != null)
        {
            StartCoroutine(FadeAudioSource(source, duration, targetVolume));
        }
    }

    // Fade out a sound
    public void FadeOut(AudioClip clip, float duration, bool stopAfterFade = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        AudioSource source = GetSourcePlayingClip(clip);
        if (source != null && source.isPlaying)
        {
            StartCoroutine(FadeAudioSource(source, duration, 0f, stopAfterFade, clip));
        }
    }

    // Get the audio source playing a specific clip
    private AudioSource GetSourcePlayingClip(AudioClip clip)
    {
        if (loopingSounds.ContainsKey(clip))
        {
            return loopingSounds[clip];
        }

        foreach (AudioSource source in audioSourcePool)
        {
            if (source.clip == clip && source.isPlaying)
            {
                return source;
            }
        }

        return null;
    }

    // Get an available audio source from the pool
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If no available source, create a new one
        Debug.LogWarning("Audio source pool exhausted. Creating new audio source.");
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool.Add(newSource);
        return newSource;
    }

    // Fade coroutine
    private IEnumerator FadeAudioSource(AudioSource source, float duration, float targetVolume, bool stopAfterFade = false, AudioClip clip = null)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        source.volume = targetVolume;

        if (stopAfterFade)
        {
            source.Stop();
            if (clip != null && loopingSounds.ContainsKey(clip))
            {
                loopingSounds.Remove(clip);
            }
        }
    }
}

/* 
USAGE EXAMPLES:

[SerializeField] private AudioClip jumpSound;
[SerializeField] private AudioClip bgMusic;
[SerializeField] private AudioClip explosionSound;

// Play one-shot sound (fire and forget)
AudioManager.Instance.PlayOneShot(jumpSound);

// Play looping background music
AudioManager.Instance.Play(bgMusic, loop: true);

// Play with custom volume and pitch
AudioManager.Instance.Play(explosionSound, loop: false, volume: 0.7f, pitch: 1.2f);

// Stop a sound
AudioManager.Instance.Stop(bgMusic);

// Stop all sounds
AudioManager.Instance.StopAll();

// Fade in music
AudioManager.Instance.FadeIn(bgMusic, 2f, targetVolume: 0.8f, loop: true);

// Fade out music
AudioManager.Instance.FadeOut(bgMusic, 2f);

// Check if playing
if (AudioManager.Instance.IsPlaying(bgMusic))
{
    // Do something
}

// Pause and resume
AudioManager.Instance.Pause(bgMusic);
AudioManager.Instance.Resume(bgMusic);

// Change volume at runtime
AudioManager.Instance.SetVolume(bgMusic, 0.5f);

SETUP:
1. Create an empty GameObject in your scene called "AudioManager"
2. Attach this AudioManager script to it
3. Reference your AudioClips in your scripts
4. Call the methods with your AudioClip references
*/