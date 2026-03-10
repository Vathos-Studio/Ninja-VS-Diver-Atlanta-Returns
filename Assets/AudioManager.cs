using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instance = new GameObject("AudioManager");
                _instance = instance.AddComponent<AudioManager>();
                DontDestroyOnLoad(instance);
            }
            return _instance;
        }
    }

    private List<AudioSource> _sources = new List<AudioSource>();
    private AudioSource _cancion;

 

    public static void PlayAudio(AudioClip clip)
    {
        Instance.PlayAudioInternal(clip);
    }

    public static void PlaySong(AudioClip clip)
    {
        Instance.PlaySongInternal(clip);
    }

    private void PlayAudioInternal(AudioClip clip)
    {
        AudioSource source = BuscaFuenteSonido();
        source.clip = clip;
        source.Play();
    }

    private void PlaySongInternal(AudioClip song)
    {
        if (_cancion == null)
        {
            _cancion = gameObject.AddComponent<AudioSource>();
            _cancion.loop = true;
            _cancion.playOnAwake = false;
        }

        _cancion.clip = song;
        _cancion.Play();
    }

    public static void StopAll()
    {
        foreach (var source in Instance._sources)
        {
            if (source != null)
                source.Stop();
        }

        if (Instance._cancion != null)
            Instance._cancion.Stop();
    }


    private AudioSource BuscaFuenteSonido()
    {
        AudioSource source = _sources.FirstOrDefault(s => !s.isPlaying);

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            _sources.Add(source);
        }

        return source;
    }

    public static AudioClip GetSong()
    {
        return Instance._cancion.clip;
    }

}
