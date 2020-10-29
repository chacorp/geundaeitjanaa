using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloat : MonoBehaviour
{
    int floatingScript = 0;
    Floater_wo_Physics FwP;
    void Start()
    {
    }

    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish") && floatingScript < 1)
        {
            FwP = gameObject.AddComponent<Floater_wo_Physics>();
            FwP.amplitude = 0.3f;
            floatingScript++;
        }
    }
}
