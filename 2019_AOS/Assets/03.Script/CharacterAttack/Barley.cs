using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barley : CharacterAttack {
    LineRenderer _shotIndicator;
    Coroutine _lineRenderingRoutine;

    bool isIndicating;
    public GameObject _circleIndicator;
    public float _indicatorSensitivity = 1f;

    Vector3 _landingPosition;
    // Use this for initialization
    void Start () {
        base.Start();
        _shotIndicator = GetComponent<LineRenderer>();
        isIndicating = false;
        _circleIndicator.SetActive(false);
        _landingPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update () {
        _fireDir = PlayerController.instance._attackStickDir;

    }

    override public void StartDrawIndicator(bool isSkill)
    {
        if (_shotIndicator.enabled == false)
        {
            isIndicating = true;
            if (isSkill)
            {
                _shotIndicator.enabled = true;
                _lineRenderingRoutine = StartCoroutine(DrawSkillIndicator());
            }
            else
            {
                _shotIndicator.enabled = true;
                _lineRenderingRoutine = StartCoroutine(DrawAttackIndicator());
            }
        }
    }

    override public void StopDrawIndicator()
    {
        if (_lineRenderingRoutine != null)
        {
            StopCoroutine(_lineRenderingRoutine);
        }
        else
        {

        }
        _circleIndicator.SetActive(false);
        _shotIndicator.enabled = false;

    }

    override public void FireBaseAttack()
    {
        if (!_playerStats.onFire()) return;

        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
        //transform.rotation = Quaternion.LookRotation(fireDir);

        GameObject muzzle = Instantiate(_muzzleEffect,
            _firePos.transform.position,
            _firePos.transform.rotation);
        StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

        GameObject projectile = GameObject.Instantiate(_projectile,
            _firePos.transform.position,
            _firePos.transform.rotation);
        projectile.GetComponent<Projectile_Barley>().TheStart(
            _playerStats._damage,
            false,
            _circleIndicator.transform.position);

    }

    IEnumerator DestroyMuzzle(GameObject muzzle, float destroyTime)
    {
        float startTime = 0;

        while (startTime <= destroyTime)
        {
            startTime += Time.deltaTime;

            muzzle.transform.localScale =
                new Vector3(1 - startTime / destroyTime,
                1 - startTime / destroyTime,
                1 - startTime / destroyTime);

            yield return null;
        }
        Destroy(muzzle);
    }

    public IEnumerator DrawSkillIndicator()
    {
        Vector3 fireDir;
        Vector3 endPoint;
        _shotIndicator.material = PlayerController.instance._skillIncatorMat;

        while (true)
        {
            fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);

            endPoint = transform.position + fireDir.normalized * _playerStats._skillRange;
            endPoint.y = 1;

            _shotIndicator.SetPosition(0, transform.position + new Vector3(0, 1, 0));
            _shotIndicator.SetPosition(1, endPoint);
            yield return null;
        }
    }

    IEnumerator DrawAttackIndicator()
    {
        Vector3 startPoint;
        Vector3 drawPoint;
        
        _shotIndicator.material = PlayerController.instance._attackIncatorMat;
        _circleIndicator.transform.position = transform.position;
        _landingPosition = Vector3.zero;

        _circleIndicator.SetActive(true);

        while (true)
        {
            startPoint = _firePos.transform.position;

            float magnitude = new Vector2(
                _fireDir.x / InGameMainUI.instance._resolutionWidthRatio,
                _fireDir.y / InGameMainUI.instance._resolutionHeightRatio).magnitude;
            magnitude *= _indicatorSensitivity;

            _landingPosition = new Vector3(_fireDir.normalized.x * magnitude,
                    0,
                    _fireDir.normalized.y * magnitude);
            if (magnitude >= PlayerController.instance._playerStats._bulletRange)
            {
                _landingPosition = _landingPosition.normalized *
                                            PlayerController.instance._playerStats._bulletRange;
            }
            _landingPosition.y = 1f;

            drawPoint = startPoint + _landingPosition + new Vector3 (0,-1,0);
            _circleIndicator.transform.position = drawPoint;

           // Debug.Log(drawPoint + " - " + _landingPosition);
            yield return null;
        }
    }

}
