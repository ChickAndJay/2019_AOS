using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpecialAttack : MonoBehaviour
{
    #region Singleton
    public static SpecialAttack instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }
    #endregion

    public GameObject _outterCircle;
    public GameObject _innerCircle;
    public GameObject _gage;

    Vector3 _stickDir;
    Vector3 _originPos_InnerCircle;

    int _fingerID;

    // Use this for initialization
    void Start()
    {
        _stickDir = Vector3.zero;
        _originPos_InnerCircle = _innerCircle.transform.position;
        _fingerID = -1;
    }

    // Update is called once per frame
    void Update()
    {
        //if (_gage.GetComponent<Image>().fillAmount < 1) return;
        Debug.Log(_fingerID);
        foreach (Touch touch in Input.touches)
        {
               // Debug.Log("EventSysten");
                if (touch.phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        continue;
                    }
                Rect image = _innerCircle.GetComponent<Image>().rectTransform.rect;
                    Rect detectRect =
                        new Rect(
                            _innerCircle.GetComponent<Image>().rectTransform.position.x - 40,
                            _innerCircle.GetComponent<Image>().rectTransform.position.y - 40,
                            80, 80);
                    if (detectRect.Contains(touch.position))
                    {
                        Debug.Log("Detect Rect");
                        _fingerID = touch.fingerId;
                    }
                }
                else if ((touch.phase == TouchPhase.Moved ||
                   touch.phase == TouchPhase.Stationary))
                {
                    if (_fingerID == -1 )
                        continue;

                    _innerCircle.transform.position = touch.position;
                    _stickDir = _innerCircle.transform.position - _outterCircle.transform.position;

                    if (_stickDir.magnitude < 50)
                    {
                        _innerCircle.transform.position = touch.position;
                    }
                    else
                    {
                        Vector2 dirNormalized = _stickDir.normalized * 50;

                        Vector2 newPos = new Vector2(_outterCircle.transform.position.x + dirNormalized.x,
                            _outterCircle.transform.position.y + dirNormalized.y);
                        _innerCircle.transform.position = newPos;

                    }

                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (_fingerID == -1)
                      continue;

                    _fingerID = -1;
                    _innerCircle.transform.position = _originPos_InnerCircle;

                }
            }
        
    }

}

