using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject _oneShot;
    public static AudioManager Instance;

    [SerializeField]
    private AudioInfo StartSound;
    
    public AudioInfo ClickSound;
    public AudioInfo DrawSound;
    
    [SerializeField]
    private AudioInfo Drums;
    
    [SerializeField]
    private AudioInfo Synth;

    private float timer = 0.0f;
    private double sectionlength = 16.0;
    private double goalTime = 0.0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        TriggerSound(StartSound,Vector3.zero);
        goalTime = AudioSettings.dspTime;
    }

    public static void TriggerSound(AudioInfo Sound, Vector3 Location)
    {
        OneShot oneShot = Instantiate(Instance._oneShot, Location, Quaternion.identity).GetComponent<OneShot>();
        oneShot.SetInfo(Sound);
        oneShot.Play();
    }

    private void Update()
    {
        if (AudioSettings.dspTime > goalTime - 1)
        {
            TriggerSoundSched(Drums,Vector3.zero,goalTime);
            TriggerSoundSched(Synth,Vector3.zero,goalTime);
            goalTime += sectionlength;
        }
    }
    
    public static void TriggerSoundSched(AudioInfo Sound, Vector3 Location,double time)
    {
        OneShot oneShot = Instantiate(Instance._oneShot, Location, Quaternion.identity).GetComponent<OneShot>();
        oneShot.SetInfo(Sound);
        oneShot.PlayScheduled(time);
    }
}
