using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{

    public float X = 1, Y = 0, Z = 0;

    // Update is called once per frame
    void Update()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 diff = new Vector3(X, Y, Z);
        currentRot += diff * Time.deltaTime;

        transform.rotation = Quaternion.Euler(currentRot);
    }
}
