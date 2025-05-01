using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioInfo", menuName = "ScriptableObjects/AudioInfo")]
public class AudioInfo : ScriptableObject
{
    public AudioClip[] Clips;
    public float PitchRandomness = 0.0f;
    public float Pitch = 1.0f;
    public float VolumeRandomness = 0.0f;
    public float Spatialization = 0.0f;

    public bool bIgnoreReverb = false;

    public AudioMixerGroup MixerGroup;
}
