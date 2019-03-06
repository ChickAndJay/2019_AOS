using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyStickController : MonoBehaviour {
    public GameObject _innerCircle;
    public GameObject _outterCircle;

    PlayerController _playerController;

    bool aiming = false;
    int fingerID = -1;

    public bool _readToFire { get; protected set; }
    public Vector3 _stickDir { get; protected set; }

    // Use this for initialization
    protected void Start () {
        _playerController = PlayerController.instance;
	}
	
	// Update is called once per frame
	void Update () {
        if (aiming)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.fingerId == fingerID)
                {
                    _innerCircle.transform.position = touch.position;
                    _stickDir = _innerCircle.transform.position - _outterCircle.transform.position;
                    //Debug.Log(dir.magnitude);
                    if (_stickDir.magnitude < 50)
                    {
                        _innerCircle.transform.position = touch.position;

                        if (fingerID == PlayerController.instance._right_touch_id)
                            PlayerController.instance._isReadyToFire = false;
                    }
                    else
                    {
                        if (fingerID == PlayerController.instance._right_touch_id)
                        {
                            PlayerController.instance._isReadyToFire = true;
                            PlayerController.instance._attackStickDir = _stickDir;
                        }
                        Vector2 dirNormalized = _stickDir.normalized * 50;

                        Vector2 newPos = new Vector2(_outterCircle.transform.position.x + dirNormalized.x,
                            _outterCircle.transform.position.y + dirNormalized.y);
                        _innerCircle.transform.position = newPos;

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

        _outterCircle.GetComponent<Image>().color = Color.white;
        _innerCircle.GetComponent<Image>().color = Color.white;
        Vector2 position = _playerController._attackStartPos;
        
        _innerCircle.GetComponent<Transform>().position = (Input.touches)[fingerID].position;
        _outterCircle.GetComponent<Transform>().position = (Input.touches)[fingerID].position;

        _innerCircle.SetActive(true);
        _outterCircle.SetActive(true);
    }

    public void StopControlling()
    {
        aiming = false;
        this.fingerID = -1;

        _outterCircle.GetComponent<Image>().color = Color.white;
        _innerCircle.GetComponent<Image>().color = Color.white;

        _innerCircle.SetActive(false);
        _outterCircle.SetActive(false);

    }
}
