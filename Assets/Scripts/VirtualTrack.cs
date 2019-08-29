using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualTrack : MonoBehaviour
{
    public GameObject TrackingObject;
    // Update is called once per frame
    void Update()
    {
        this.transform.position = TrackingObject.transform.position;
    }
}
