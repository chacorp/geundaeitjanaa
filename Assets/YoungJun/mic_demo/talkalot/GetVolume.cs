//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(AudioSource))]

//public class GetVolume : MonoBehaviour
//{
//    void Start()
//    {
//        _audio.loop = true; // AudioClip loop setting
//        _audio.mute = false; // Mute.
//        _input = Microphone.devices[0].ToString();
//        GetMicCaps();
//    }

//    void Update()
//    {
//        loudness = GetAveragedVolume() * sensitivity;

//        if (!Microphone.IsRecording(_input))
//        {
//            StartMicrophone();
//        }

//        mTimer += Time.deltaTime;

//        if (mTimer >= mRefTime)
//        {
//            StopMicrophone();
//            StartMicrophone();
//            mTimer = 0;
//        }
//    }

//    void GetMicCaps()
//    {
//        Microphone.GetDeviceCaps(_input, out minFreq, out maxFreq); // gets freq of device

//        if (maxFreq - minFreq < 1)
//        {
//            minFreq = 0;
//            maxFreq = 44100;
//        }
//    }

//    public void StartMicrophon()
//    {
//        _audio.clip = Microphone.Start(_input, true, 10, maxFreq); // starts recording
//        while (!(Microphone.GetPosition(input) > 0)) { } // wait until the recording starts
//        _audio.Play(); // plays the audio source
//    }

//    public void StopMicrophone()
//    {
//        _audio.Stop(); // stops the audio
//        Microphone.End(input); // stops the recording of the device
//    }

//    float GetAveragedVolume()
//    {
//        float[] data = new float[amountSamples];
//        float a = 0;
//        _audio.GetOutputData(data, 0);

//        foreach (float s in data)
//        {
//            a += Mathf.Abs(s);
//        }
//        return a / amountSamples;
//    }
//}
