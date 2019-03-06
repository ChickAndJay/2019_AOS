using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum FightType
{
    Knight,
    Archer,
    Mage
}

public class PlayerController : MonoBehaviour {
    #region Singleton
    public static PlayerController instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }
    #endregion

    private CharacterController _controller;
    Animator _playerAnimator;

    #region Movement
    private bool _isMoving = false;
    public float _speed = 5.0f;
    public float _rotationSpeed = 100.0f;

    private float _width;
    private float _height;

    public Vector2 _movingStartPos { get; private set; }
    public Vector2 _movingCurrentPos { get; private set; }

    public Vector2 _attackStartPos { get; private set;}
    public Vector2 _attackCurrentPos { get; private set; }

    public int _left_touch_id { get; private set; }
    public int _right_touch_id { get; private set; }
    
    public bool _isReadyToFire { get; set; }

    bool _dash;
    #endregion

    #region Shooting
    public GameObject _attackProjectile;
    public GameObject _firePos;
    Quaternion _fireRot;

    public GameObject _muzzleEffect;
    LineRenderer _shotIndicator;
    Coroutine _LineRenderingRoutine;
    #endregion

    public PlayerStats _playerStats { get; set; }

    GrassDetect _grassDetect;
    //bool _isAttacking;
    //private int _attack;
    //private bool canChain;
    //public FightType fightType;

    //Coroutine chainLockMovementRoutine;
    //Coroutine chainBrokeChain;

    //private bool[] _skill_Able = new bool[3];
    //private bool _all_skill_able;
    //private bool _isSkillActivate = false;

    // Use this for initialization
    void Start () {
        _playerAnimator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();

        _width = (float)Screen.width / 2.0f;
        _height = (float)Screen.height / 2.0f;

        _movingStartPos = Vector2.zero;
        _movingCurrentPos = Vector2.zero;
        _attackStartPos = Vector2.zero;
        _attackCurrentPos = Vector2.zero;

        _left_touch_id = -1;
        _right_touch_id = -1;

        _dash = false;

        _shotIndicator = GetComponent<LineRenderer>();
        _shotIndicator.enabled = false;

        _playerStats = GetComponent<PlayerStats>();
        _grassDetect = GetComponentInChildren<GrassDetect>();
        //_grassDetect.enabled = false;
        //_isAttacking = false;
        //_attack = 0;
        //canChain = false;
        //fightType = FightType.Knight;

        //_skill_Able[0] = true;
        //_skill_Able[1] = true;
        //_skill_Able[2] = true;
        //_all_skill_able = true;

    }

	// Update is called once per frame
	void Update () {
        foreach (Touch touch in Input.touches)
        {           
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    continue;
                }
                Vector2 pos = touch.position;
                pos.x = (pos.x - _width) / _width;
                pos.y = (pos.y - _height) / _height;

                if (pos.x <= 0)
                {
                    if (_left_touch_id >= 0) continue;

                    _left_touch_id = touch.fingerId;
                    _movingStartPos = new Vector2(pos.x, pos.y);
                    _isMoving = true;

                    InGameMainUI.instance.MovementStick.StartControlling(touch.fingerId);

                }
                else
                {
                    if (_right_touch_id >= 0) continue;

                    _right_touch_id = touch.fingerId;
                    _attackStartPos = new Vector2(pos.x, pos.y);

                    InGameMainUI.instance.AttackStick.StartControlling(touch.fingerId);
                }
            }
            if (touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary &&
                _isMoving)
            {
                if (touch.fingerId != _left_touch_id && touch.fingerId != _right_touch_id)
                    continue;
                Vector2 pos = touch.position;
                pos.x = (pos.x - _width) / _width;
                pos.y = (pos.y - _height) / _height;

                if (_left_touch_id == touch.fingerId)
                    _movingCurrentPos = new Vector2(pos.x, pos.y);
                else if(_right_touch_id != touch.fingerId)
                {
                    _attackCurrentPos = new Vector2(pos.x, pos.y);
                }
                                       
                Vector2 dragDir = Vector2.zero;
                if(_left_touch_id == touch.fingerId)
                    dragDir = _movingCurrentPos - _movingStartPos;
                else if(_right_touch_id == touch.fingerId)
                    dragDir = _attackCurrentPos - _attackStartPos;

                float dragAmount = dragDir.magnitude ;
                if (_left_touch_id == touch.fingerId)
                    dragAmount *= 7;
                else if (_right_touch_id == touch.fingerId)
                    ;

                dragAmount = Mathf.Clamp(dragAmount, 0f, 1f);

                Vector3 horizontal = new Vector3(1, 0, 0);
                Vector3 vertical = new Vector3(0, 0, 1);

                Vector3 moveDir = (horizontal * dragDir.x + vertical * dragDir.y).normalized;
                //Vector3 moveDir = dragDir.normalized;



                if (touch.fingerId == _left_touch_id)
                {
                    if (dragAmount > 0.4f)
                    {
                        _playerAnimator.SetBool("isMoving", true);
                        _playerAnimator.SetFloat("MoveSpeed", dragAmount);
                        transform.rotation = Quaternion.LookRotation(moveDir);
                    }
                    else
                    {
                        dragDir = Vector2.zero;
                    }

                    if (!_dash)
                    {
                        _controller.Move(moveDir * Time.deltaTime * _speed * dragAmount);
                        _playerAnimator.SetBool("Rolling", false);
                    }
                }else if(touch.fingerId == _right_touch_id)
                {
                    _shotIndicator.enabled = true;
                    _LineRenderingRoutine = StartCoroutine(DrawIndicatorLine());
                    //_shotIndicator.SetPosition(1, transform.position);
                    //Debug.Log(dragDir);
                    //_shotIndicator.SetPosition(1, 
                    //    transform.position + new Vector3(dragDir.x, 0, dragDir.y).normalized * _playerStats._bulletRange);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (touch.fingerId == _left_touch_id && _left_touch_id >= 0)
                {
                    InGameMainUI.instance.MovementStick.StopControlling();
                    _left_touch_id = -1;
                }
                else if (touch.fingerId == _right_touch_id && _right_touch_id >= 0)
                {
                    StopCoroutine(_LineRenderingRoutine);
                    _shotIndicator.enabled = false;

                    if (_isReadyToFire)
                        FireBasicAttack();

                    _isReadyToFire = false;
                    InGameMainUI.instance.AttackStick.StopControlling();
                    _right_touch_id = -1;
                }
            }            
        } // foreach end
        if (Input.touchCount <= 0)
        {
            _isMoving = false;
            _playerAnimator.SetBool("isMoving", false);
            _playerAnimator.SetFloat("MoveSpeed", 0);
            if (!_dash)
            {
                _playerAnimator.SetBool("Rolling", false);
            }
            _movingStartPos = Vector3.zero;
            _movingCurrentPos = Vector3.zero;
        }
    }

    void FireBasicAttack()
    {
        if (!GetComponent<PlayerStats>().onFire()) return;

        Vector3 fireDir = new Vector3(InGameMainUI.instance.AttackStick.stickDir.x, 0 , InGameMainUI.instance.AttackStick.stickDir.y);
        transform.rotation = Quaternion.LookRotation(fireDir);

        GameObject muzzle = Instantiate(_muzzleEffect, 
            _firePos.transform.position,
            _firePos.transform.rotation);
        StartCoroutine(DestroyMuzzle(muzzle, 0.4f));
        
        GameObject projectile = GameObject.Instantiate(_attackProjectile,
            _firePos.transform.position,
            _firePos.transform.rotation);
        projectile.GetComponent<Projectile>().TheStart(_playerStats._bulletVelocity,
            _playerStats._damage, _playerStats._bulletRange, gameObject);
        //projectile.SetActive(true);

    }

    IEnumerator DestroyMuzzle(GameObject muzzle, float destroyTime)
    {
        float startTime = 0;

        while(startTime <= destroyTime)
        {
            startTime += Time.deltaTime;

            muzzle.transform.localScale =
                new Vector3(1 - startTime / destroyTime, 
                1 - startTime / destroyTime, 
                1 - startTime / destroyTime);

            yield return null;
        }
        //yield return new WaitForSeconds(0.2f);
        Destroy(muzzle);
    }

    IEnumerator DrawIndicatorLine()
    {
        Vector3 fireDir;
        Vector3 endPoint;
        while (true)
        {
            fireDir = new Vector3(InGameMainUI.instance.AttackStick.stickDir.x, 0, InGameMainUI.instance.AttackStick.stickDir.y);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, fireDir, out hit, _playerStats._bulletRange, LayerMask.NameToLayer("Grass") ))
            {
                endPoint = hit.point;
            }
            else
            {
                endPoint = transform.position + fireDir.normalized * _playerStats._bulletRange;

            }

            endPoint.y = 1; 
            _shotIndicator.SetPosition(0, transform.position + new Vector3(0,1,0));
            //Debug.Log(dragDir);
            _shotIndicator.SetPosition(1, endPoint);


            yield return null;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Grass"))
    //    {

    //    }
    //}
}
