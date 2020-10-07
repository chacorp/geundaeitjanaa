using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingPlayer : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        
    }

    void followTarget()
    {
        float distance = Vector3.Distance(target.position, transform.position);
    }

    void Update()
    {
     
        
    }
}
