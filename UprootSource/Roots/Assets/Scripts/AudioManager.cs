using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;
    public bool fart;

    private void Awake() {
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            
            DontDestroyOnLoad(gameObject);
            Instance = this;
            AudioManager.Instance.Play("music");
        }
    }

    public void Start() {
        
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Play();
    }

    public void PlayOne(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.PlayOneShot(s.clip);
    }

}
