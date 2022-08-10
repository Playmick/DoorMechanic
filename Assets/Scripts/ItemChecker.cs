using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    [SerializeField] Door door;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "door")
        {
            door.showMessageOpenDoor = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "door")
        {
            door.showMessageOpenDoor = false;
        }
    }
}
