using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform watching;
    Vector3 relativePosition;

    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    // Start is called before the first frame update
    void Start()
    {
        relativePosition = transform.position - watching.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 watchPos = transform.position - relativePosition;

        if (followX)
            watchPos.x = watching.position.x;
        if (followY)
            watchPos.y = watching.position.y;
        if (followZ)
            watchPos.z = watching.position.z;

        transform.position = watchPos + relativePosition;
    }
}
