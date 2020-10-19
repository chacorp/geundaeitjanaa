using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GetMic : MonoBehaviour
{
    public int audioSampleRate = 44100;
    public string microphone;

    private List<string> options = new List<string>();

    //public float[] _spectrum = new float[512];
    //public float[] outputs = new float[512];

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        foreach(string device in Microphone.devices)
        {
            if (microphone == null)
            {
                microphone = device;
            }
            options.Add(device);
        }

        microphone = options[_PlayerPrefsManager.GetMicrophone()];
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateMicrophone();
        }

        else if (Input.GetKeyUp(KeyCode.Space))
        {
            Microphone.End(microphone);

            //Play recorded audio, just for testing purpose.
            //audioSource.Play();
            //SavWav.Save("original", audioSource.clip);
        }

        else if (Input.GetKeyDown(KeyCode.P))
        {
            audioSource.Play();
            //audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
        }


    }

    void UpdateMicrophone()
    {
        Debug.Log("Start Recording");

        audioSource.Stop();
        // Start recording to audioclip from the mic
        // .Start(device name, loop, int time, int freq)
        audioSource.clip = Microphone.Start(microphone, false, 10, audioSampleRate);
        //audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.Blackman);
        //audioSource.loop = true;
        //Debug.Log(down + "seconds has been recorded");

        if (Microphone.IsRecording(microphone))
        {
            Debug.Log(microphone + " : ON AIR");
        }
        else Debug.Log(microphone + " : NOT being recorded");
    }
}
