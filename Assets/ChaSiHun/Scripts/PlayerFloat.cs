using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloat : MonoBehaviour
{
    public float height = 2f;
    public float floatSpeed = 0.5f;
    Vector3 maxYpos;
    Vector3 minYpos;
    Vector3 velo = Vector3.zero;
    bool up = true;

    void Start()
    {
        Vector3 pos = transform.position;
        maxYpos = new Vector3(pos.x, pos.y + (height / 2), pos.z);
        minYpos = new Vector3(pos.x, pos.y - (height / 2), pos.z);
    }

    void Update()
    {
        if (!GameSceneManager.Instance.useCamera)
            return;

        if (!up)
        {
            transform.position = Vector3.SmoothDamp(transform.position, minYpos, ref velo, floatSpeed);
            if (transform.position.y <= minYpos.y + 0.01f)
            {
                up = true;
            }
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, maxYpos, ref velo, floatSpeed);
            if (transform.position.y >= maxYpos.y - 0.01f)
            {
                up = false;
            }
        }
    }
}
