using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAudioHandler : MonoBehaviour
{
    public static float stdSoundsVolume = 0.6f;
    public static float stdMusicsVolume = 0.6f;
    private static List<AudioSource> audioSrcSoundList = new List<AudioSource>();
    private static AudioSource audioSrcMusic;

    public static StaticAudioHandler instance;



    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void init()
    {
        //start sound / music
        setSoundsOnOff(SaveLoadSystem.instance.loadSoundState());
        setMusicOnOff(SaveLoadSystem.instance.loadMusicState());
        if (getAudioSrcMusic() == null || (getAudioSrcMusic().clip.name != null && getAudioSrcMusic().clip.name != SoundChooser.instance.menu_music.name))
        {
            if (getAudioSrcMusic() != null)
                Destroy(getAudioSrcMusic().gameObject);

            playMusic(SoundChooser.instance.menu_music);
        }
    }

    public static bool switchSoundsOnOff()
    {
        if (SaveLoadSystem.instance.loadSoundState())
            PlayCloudDataManager.currentSavegame.soundIsOn = setSoundsOnOff(false);
        else
            PlayCloudDataManager.currentSavegame.soundIsOn = setSoundsOnOff(true);

        return getSoundIsOn();
    }

    public static bool setSoundsOnOff(bool soundState)
    {
        PlayCloudDataManager.currentSavegame.soundIsOn = soundState;

        for (int i = 0; i < audioSrcSoundList.Count; i++)
            setSoundVolume(audioSrcSoundList[i]);

        return soundState;
    }

    public static bool getSoundIsOn()
    {
        return SaveLoadSystem.instance.loadSoundState();
    }

    private static void setSoundVolume(AudioSource audioSrc)
    {
        if (audioSrc == null)
            return;

        if (getSoundIsOn())
            audioSrc.volume = stdSoundsVolume;
        else
            audioSrc.volume = 0;
    }


    public static bool switchMusicOnOff()
    {
        if (SaveLoadSystem.instance.loadMusicState())
            PlayCloudDataManager.currentSavegame.musicIsOn = setMusicOnOff(false);
        else
            PlayCloudDataManager.currentSavegame.musicIsOn = setMusicOnOff(true);

        return getMusicIsOn();
    }

    public static bool setMusicOnOff(bool musicState)
    {
        PlayCloudDataManager.currentSavegame.musicIsOn = musicState;

        setMusicVolume(audioSrcMusic);

        return musicState;
    }

    public static bool getMusicIsOn()
    {
        return SaveLoadSystem.instance.loadMusicState();
    }

    private static void setMusicVolume(AudioSource audioSrc)
    {
        if (audioSrc == null)
            return;

        if (getMusicIsOn())
            audioSrc.volume = stdMusicsVolume;
        else
            audioSrc.volume = 0;
    }


    public static AudioSource playSound(AudioClip sound, bool randomPitch = false)
    {
        if (sound == null)
            return null;

        GameObject goAudioSrc = new GameObject("tmpAudio: " + sound.name);
        AudioSource audioSrc = goAudioSrc.AddComponent<AudioSource>();
        audioSrcSoundList.Add(audioSrc);

        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        setSoundVolume(audioSrc);
        audioSrc.clip = sound;
        if (randomPitch)
            audioSrc.pitch = Random.Range(0.8f, 0.9f);
        audioSrc.Play();

        DontDestroyOnLoad(goAudioSrc);
        Destroy(goAudioSrc, audioSrc.clip.length);

        return audioSrc;
    }

    public static AudioSource playSoundWithPitch(AudioClip sound, float newPitch = 1)
    {
        if (sound == null)
            return null;

        GameObject goAudioSrc = new GameObject("tmpAudioSrc");
        AudioSource audioSrc = goAudioSrc.AddComponent<AudioSource>();
        audioSrcSoundList.Add(audioSrc);

        audioSrc.playOnAwake = false;
        audioSrc.loop = false;
        setSoundVolume(audioSrc);
        audioSrc.clip = sound;
        audioSrc.pitch = newPitch;
        audioSrc.Play();

        DontDestroyOnLoad(goAudioSrc);
        Destroy(goAudioSrc, audioSrc.clip.length);

        return audioSrc;
    }


    public static AudioSource playMusic(AudioClip musicClip)
    {
        if (musicClip == null)
            return null;

        if (audioSrcMusic != null)
            Destroy(audioSrcMusic);

        GameObject goAudioSrc = new GameObject("tmpAudioSrcMusic");
        AudioSource audioSrc = goAudioSrc.AddComponent<AudioSource>();
        audioSrcMusic = audioSrc;

        audioSrc.clip = musicClip;
        setMusicVolume(audioSrc);
        audioSrc.loop = true;
        audioSrc.Play();

        DontDestroyOnLoad(goAudioSrc);

        return audioSrc;
    }

    public static AudioSource getAudioSrcMusic()
    {
        return audioSrcMusic;
    }

}
