using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class _PlayerPrefsManager : MonoBehaviour
{
    const string MICROPHONE_KEY = "microphone";

    public static void SetMicrophone(int mic)
    {
        PlayerPrefs.SetInt(MICROPHONE_KEY, mic);
    }

    public static int GetMicrophone()
    {
        return PlayerPrefs.GetInt(MICROPHONE_KEY);
    }
}
