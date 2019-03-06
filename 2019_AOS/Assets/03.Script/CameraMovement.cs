using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    Transform target_Tr;

    public Vector3 offset = new Vector3(0, 37.5f, -25);
    public Vector3 rotation = new Vector3(55, 0, 0);

    bool following = true;

    public float smoothSpeed = 0.125f;

    // Use this for initialization
    void Start()
    {
        target_Tr = PlayerController.instance.GetComponent<Transform>();

        transform.rotation = Quaternion.Euler(rotation);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (following)
        {
            Vector3 ExceptX = new Vector3(0, 0, target_Tr.position.z);
            Vector3 desiredPos = ExceptX + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
            transform.position = smoothedPos;
        }
    }
}
