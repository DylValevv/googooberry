using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioMixerGroup masterMixer;

    [Header("Action")]
    [SerializeField] private AudioMixerGroup actionMixer;
    [SerializeField] private Sound[] actionSounds;

    [Header("Ambient")]
    [SerializeField] private AudioMixerGroup ambientMixer;
    [SerializeField] private Sound[] ambientSounds;

    [Header("Dialogue")]
    [SerializeField] private AudioMixerGroup dialogueMixer;
    [SerializeField] private Sound[] dialogueSounds;

    [Header("Music")]
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private Sound[] musicSounds;


    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        setup(dialogueSounds, dialogueMixer);
        setup(actionSounds, actionMixer);
        setup(ambientSounds, ambientMixer);
        setup(musicSounds, musicMixer);
    }

    private void setup(Sound[] sounds, AudioMixerGroup audioMixer)
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.loop = s.loop;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.outputAudioMixerGroup = audioMixer;
        }
    }

    #region Play Sounds
    private void Play(string name, Sound[] sounds)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }
    public void PlayAmbient(string name)
    {
        Play(name, ambientSounds);
    }

    public void PlayAction(string name)
    {
        Play(name, actionSounds);
    }

    public void PlayDialogue(string name)
    {
        Play(name, dialogueSounds);
    }

    public void PlayMusic(string name)
    {
        Play(name, musicSounds);
    }
    #endregion

    #region Stop Sounds
    private void Stop(string name, Sound[] sounds)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    public void StopAmbient(string name)
    {
        Stop(name, ambientSounds);
    }

    public void StopAction(string name)
    {
        Stop(name, actionSounds);
    }

    public void StopDialogue(string name)
    {
        Stop(name, dialogueSounds);
    }

    public void StopMusic(string name)
    {
        Stop(name, musicSounds);
    }
    #endregion

    public void SetLevel(float sliderValue)
    {
        sliderValue = Mathf.Log10(sliderValue) * 20;
        masterMixer.audioMixer.SetFloat("SFXVol", sliderValue);
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}