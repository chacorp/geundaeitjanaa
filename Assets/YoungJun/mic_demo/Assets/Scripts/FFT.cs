//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using B83.MathHelpers;

//public class FFT : MonoBehaviour
//{
//    float[] spec = new float[1024];
//    float[] tmp = new float[2048];
//    Complex[] spec2 = new Complex[2048];

//    // Update is called once per frame
//    void Update()
//    {
//        // Unity's FFT function
//        AudioListener.GetSpectrumData(spec, 0, FFTWindow.Rectangular);
//        for (int i = 0; i < spec.Length; i++)
//        {
//            Debug.DrawLine(new Vector3(i, 0), new Vector3(i, spec[i]), Color.cyan);
//        }

//        // My FFT based on the output samples.
//        AudioListener.GetOutputData(tmp, 0);
//        // copy the output data into the complex array
//        for (int i = 0; i < tmp.Length; i++)
//        {
//            spec2[i] = new Complex(tmp[i], 0);
//        }
//        // calculate the FFT
//        FFT.CalculateFFT(spec2, false);
//        for (int i = 0; i < spec2.Length/2; i++)  // plot only the first half
//        {
//            // multiply the magnitde of each value by 2
//            Debug.DrawLine(new Vector3(i, 4), new Vector3(i, 4+(float)spec2[i].magnitude+2), Color.white)
//        }
//    }
//}
