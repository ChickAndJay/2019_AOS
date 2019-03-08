using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public GameObject start;
    public GameObject end;

    float time;
    float up = 1f;
	// Use this for initialization
	void Start () {
        time = 0;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        Vector3 temp = Vector3.Lerp(start.transform.position, end.transform.position, time / 5);
        temp += new Vector3(0, Time.time, 0);

        transform.position = temp;
	}
}
