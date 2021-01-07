using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [System.Serializable]
    public struct NamedAudioSource
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        public AudioSource source;
    };
    public NamedAudioSource[] namedSources;

    // Called during initialization
    void Awake()
    {
        for(int i = 0; i < namedSources.GetLength(0); i++)
        {
            namedSources[i].source = gameObject.AddComponent<AudioSource>();
            namedSources[i].source.clip = namedSources[i].clip;
            namedSources[i].source.volume = namedSources[i].volume;
        }
    }

    public void Play(string name)
    {
        AudioSource source = Array.Find(namedSources, namedSource => namedSource.name == name).source;
        if(source == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        source.Play();
    }
}
