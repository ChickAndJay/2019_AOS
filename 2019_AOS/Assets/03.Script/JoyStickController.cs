using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyStickController : MonoBehaviour {
    public GameObject InnerCircle;
    public GameObject OutterCircle;

    PlayerController _playerController;

    bool aiming = false;
    int fingerID = -1;

    public bool readToFire { get; private set; }
    public Vector3 stickDir { get; private set; }

    // Use this for initialization
    void Start () {
        _playerController = PlayerController.instance;
        readToFire = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (aiming)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.fingerId == fingerID)
                {
                    InnerCircle.transform.position = touch.position;
                    stickDir = InnerCircle.transform.position - OutterCircle.transform.position;
                    //Debug.Log(dir.magnitude);
                    if (stickDir.magnitude < 50)
                    {
                        InnerCircle.transform.position = touch.position;

                        if (fingerID == PlayerController.instance._right_touch_id)
                        {
                            OutterCircle.GetComponent<Image>().color = Color.white;
                            InnerCircle.GetComponent<Image>().color = Color.white;

                            PlayerController.instance._isReadyToFire = false;
                        }
                    }
                    else
                    {
                        if (fingerID == PlayerController.instance._right_touch_id)
                        {
                            OutterCircle.GetComponent<Image>().color = Color.red;
                            InnerCircle.GetComponent<Image>().color = Color.red;

                            PlayerController.instance._isReadyToFire = true;
                        }
                        Vector2 dirNormalized = stickDir.normalized * 50;

                        Vector2 newPos = new Vector2(OutterCircle.transform.position.x + dirNormalized.x,
                            OutterCircle.transform.position.y + dirNormalized.y);
                        InnerCircle.transform.position = newPos;

                        readToFire = true;
                    }
                }
            }
        }
    }

    public void StartControlling(int fingerID)
    {
        if (aiming) return;
        aiming = true;
        this.fingerID = fingerID;

        OutterCircle.GetComponent<Image>().color = Color.white;
        InnerCircle.GetComponent<Image>().color = Color.white;
        Vector2 position = _playerController._attackStartPos;
        
        InnerCircle.GetComponent<Transform>().position = (Input.touches)[fingerID].position;
        OutterCircle.GetComponent<Transform>().position = (Input.touches)[fingerID].position;

        InnerCircle.SetActive(true);
        OutterCircle.SetActive(true);
    }

    public void StopControlling()
    {
        aiming = false;
        this.fingerID = -1;

        OutterCircle.GetComponent<Image>().color = Color.white;
        InnerCircle.GetComponent<Image>().color = Color.white;

        InnerCircle.SetActive(false);
        OutterCircle.SetActive(false);

    }
}
