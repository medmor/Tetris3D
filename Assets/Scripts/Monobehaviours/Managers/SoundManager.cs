using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

public class SoundManager : Manager<SoundManager>
{
    public float SoundVolum { 
        get { return AudioListener.volume; } 
        set { AudioListener.volume = value; }
    }

    public bool soundMute {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    [SerializeField] private AudioSource audioSource = default;
    [SerializeField]
    private AudioClipDictionary audioClips = new AudioClipDictionary();




    private void Start()
    {
    }

    public void PlaySound(Enums.SoundsEffects audioClipKey)
    {
        audioSource.PlayOneShot(audioClips[audioClipKey]);
    }


}

[Serializable]
public class AudioClipDictionary : SerializableDictionaryBase<Enums.SoundsEffects, AudioClip> { }

