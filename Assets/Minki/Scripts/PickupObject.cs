using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Box collider를 젠된 아이템과 플레이어가 던질 튜브에 추가, is Trigger 버튼 활성화
public class PickupObject : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            print ("Item picked up");
            Destroy(gameObject);
        }
    }
}
