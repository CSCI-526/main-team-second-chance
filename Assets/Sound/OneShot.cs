using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShot : MonoBehaviour
{
    private AudioInfo _audioInfo;

    [SerializeField]
    private AudioSource _audioSource;
    
    public void SetInfo(AudioInfo info)
    {
        _audioInfo = info;
    }

    public void Play()
    {
        DontDestroyOnLoad(gameObject);
        int index = Random.Range(0, _audioInfo.Clips.Length);
        _audioSource.clip = _audioInfo.Clips[index];

        _audioSource.pitch = _audioInfo.Pitch + Random.Range(-_audioInfo.PitchRandomness, _audioInfo.PitchRandomness);
        _audioSource.volume = 1.0f - Random.Range(0.0f, _audioInfo.VolumeRandomness);
        
        _audioSource.bypassReverbZones = _audioInfo.bIgnoreReverb;
        _audioSource.spatialBlend = _audioInfo.Spatialization;
        _audioSource.outputAudioMixerGroup = _audioInfo.MixerGroup;
        
        _audioSource.Play();
        Destroy(gameObject,_audioSource.clip.length);
    }

    public void PlayScheduled(double time)
    {
        DontDestroyOnLoad(gameObject);
        int index = Random.Range(0, _audioInfo.Clips.Length);
        _audioSource.clip = _audioInfo.Clips[index];

        _audioSource.pitch = _audioInfo.Pitch + Random.Range(-_audioInfo.PitchRandomness, _audioInfo.PitchRandomness);
        _audioSource.volume = 1.0f - Random.Range(0.0f, _audioInfo.VolumeRandomness);
        
        _audioSource.bypassReverbZones = _audioInfo.bIgnoreReverb;
        _audioSource.spatialBlend = _audioInfo.Spatialization;
        _audioSource.outputAudioMixerGroup = _audioInfo.MixerGroup;
        
        _audioSource.PlayScheduled(time);
        Destroy(gameObject,_audioSource.clip.length);
    }
}
