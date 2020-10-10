//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Audio;

//[RequireComponent(typeof(AudioSource))]
//public class GetMicV2 : MonoBehaviour
//{
//    AudioSource _audioSource;

//    //Microphone input
//    public AudioClip _audioClip;
//    public bool _useMicrophone;
//    public string _selectedDevice;

//    //FFT values
//    private float[] _samplesLeft = new float[512];
//    private float[] _samplesRight = new float[512];

//    private float[] _freqBand = new float[8];
//    private float[] _bandBuffer = new float[8];
//    private float[] _bufferDecrease = new float[8];
//    private float[] _freqBandHighest = new float[8];

//    // Audio Band values
//    [HideInInspector]
//    public float[] _audioBand, _audioBandBuffer;

//    // Amplitude variables
//    [HideInInspector]
//    public float _Amplitude, _AmplitudeBuffer;
//    private float _AmplitudeHighest;

//    // Audio Profile
//    public float _audioProfile;

//    // stereo channels
//    public enum _channel { Stereo, Left, Right };
//    public _channel channel = new _channel();

//    // Audio64
//    float[] _freqBand64 = new float[64];
//    float[] _bandBuffer64 = new float[64];
//    float[] _bufferDecrease64 = new float[64];
//    float[] _freqBandHighest64 = new float[64];

//    // audio band64 values
//    [HideInInspector]
//    public float[] _audioBand64, _audioBandBuffer64;
    


//    // Use this for initialization
//    void Start()
//    {
//        _audioBand = new float[8];
//        _audioBandBuffer = new float[8];
//        _audioBand64 = new float[64];
//        _audioBandBuffer64 = new float[64];
//        _audioSource = GetComponent<AudioSource>();
//        AudioProfile(_audioProfile);

//        // Microphone input
//        if(_useMicrophone)
//        {
//            if(Microphone.devices.Length > 0)
//            {
//                _selectedDevice = Microphone.devices[0].ToString();
//                _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
//            }
//            else
//            {
//                _useMicrophone = false;
//            }
//        }

//        if(!_useMicrophone)
//        {
//            _audioSource.clip = _audioClip;
//        }

//        _audioSource.Play();
//    }


//    // Update is called once per frame
//    void Update()
//    {
//        if(_audioSource.flip != null)
//        {
//            GetSpectrumAudioSource();
//            MakeFrequencyBands();
//            MakeFrequencyBands64();
//            BandBuffer();
//            BandBuffer64();
//            CreateAudioBands();
//            CreateAudioBands64();
//            GetAmplitude();
//        }
//    }
//}
