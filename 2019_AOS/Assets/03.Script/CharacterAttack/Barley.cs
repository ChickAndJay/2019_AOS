using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barley : CharacterAttack {
    LineRenderer _shotIndicator;
    Coroutine _lineRenderingRoutine;

    Vector2 _attackStartPos;
    Vector2 _attackCurrentPos;

    bool isIndicating;
    public GameObject _circleIndicator;
    public float _indicatorSensitivity = 1f;

    // Use this for initialization
    void Start () {
        base.Start();
        _shotIndicator = GetComponent<LineRenderer>();
        isIndicating = false;
        _circleIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        _fireDir = PlayerController.instance._attackStickDir;
        _attackStartPos = PlayerController.instance._attackStartPos;
        _attackCurrentPos = PlayerController.instance._attackCurrentPos;
  
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
        transform.rotation = Quaternion.LookRotation(fireDir);

        GameObject muzzle = Instantiate(_muzzleEffect,
            _firePos.transform.position,
            _firePos.transform.rotation);
        StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

        GameObject projectile = GameObject.Instantiate(_projectile,
            _firePos.transform.position,
            _firePos.transform.rotation);
        //projectile.GetComponent<Projectile>().TheStart(_playerStats._bulletVelocity,
        //    _playerStats._damage, _playerStats._bulletRange, false, gameObject);

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
        _circleIndicator.SetActive(true);
        Vector3 fireDir;
        Vector3 endPoint;
        
        _shotIndicator.material = PlayerController.instance._attackIncatorMat;
        _circleIndicator.transform.position = transform.position;

        while (true)
        {
            endPoint = (_attackCurrentPos - _attackStartPos) * _indicatorSensitivity;            
            endPoint = new Vector3(endPoint.x, 1, endPoint.y); // make 2d to 3d
            
            _circleIndicator.transform.position = endPoint + transform.position;

            //fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, fireDir, out hit, _playerStats._bulletRange, LayerMask.NameToLayer("Grass")))
            //    endPoint = hit.point;
            //else
            //    endPoint = transform.position + fireDir.normalized * _playerStats._bulletRange;

            //endPoint.y = 1;
            //_shotIndicator.SetPosition(0, transform.position + new Vector3(0, 1, 0));
            //_shotIndicator.SetPosition(1, endPoint);
            yield return null;
        }
    }

}
