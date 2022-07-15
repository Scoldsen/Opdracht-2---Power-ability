using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Sound
{
    public string name;
    public AudioClip sound;
}

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource musicPlayer;
    private AudioSource soundPlayer;

    private Dictionary<string, AudioClip> musicDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();
        SetupMusicPlayer();
        SetupSoundPlayer();
    }

    private void InitMusicDictionary()
    {
        var musicTracks = ResourceLoader.loadResources("Audio/Music", typeof(AudioClip));
        //Object[] musicTracks = Resources.LoadAll("Audio/Music", typeof(AudioClip));

        for (int i = 0; i < musicTracks.Length; i++)
        {
            var musicTrack = (AudioClip)musicTracks[i];
            musicDict.Add(musicTrack.name, musicTrack);
        }
    }

    private void InitSoundDictionary()
    {
        var sounds = ResourceLoader.loadResources("Audio/Sound", typeof(AudioClip));
        //Object[] sounds = Resources.LoadAll("Audio/Sound", typeof(AudioClip));

        for (int i = 0; i < sounds.Length; i++)
        {
            var sound = (AudioClip)sounds[i];
            soundDict.Add(sound.name, sound);
        }
    }

    private void SetupMusicPlayer()
    {
        musicPlayer = gameObject.AddComponent<AudioSource>();
        musicPlayer.loop = true;
        musicPlayer.playOnAwake = false;
        InitMusicDictionary();
    }

    private void SetupSoundPlayer()
    {
        soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.playOnAwake = false;
        InitSoundDictionary();
    }

    public void PlayMusic(string musicName)
    {
        if (musicDict.ContainsKey(musicName))
        {
            AudioClip clip = musicDict[musicName];
            StopMusic();
            musicPlayer.clip = clip;
            musicPlayer.Play();
        }        
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
    }

    public void PlaySound(string soundName)
    {
        if (soundDict.ContainsKey(soundName))
        {
            AudioClip clip = soundDict[soundName];
            soundPlayer.PlayOneShot(clip);
        }
    }
}
