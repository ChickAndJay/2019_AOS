using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colt : CharacterAttack {
    LineRenderer _shotIndicator;
    Coroutine _lineRenderingRoutine;

    // Use this for initialization
    private void Start()
    {
        base.Start();
        _shotIndicator = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _fireDir = PlayerController.instance._attackStickDir;
    }

    override public void StartDrawIndicator(bool isSkill) {
        if (_shotIndicator.enabled == false)
        {
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

    override public void StopDrawIndicator() {
        if (_lineRenderingRoutine != null)
        {
            StopCoroutine(_lineRenderingRoutine);
            _shotIndicator.enabled = false;
        }
        else
        {
            _shotIndicator.enabled = false;
        }
    }

    override public void FireBaseAttack() {
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
        projectile.GetComponent<Projectile_Colt>().TheStart(_playerStats._bulletVelocity,
            _playerStats._damage, _playerStats._bulletRange, false);

    }

    override public void FireSkillAttack()
    {
        StartCoroutine(ActivateSkill());
    }

    IEnumerator ActivateSkill()
    {
        int shotCount = 0;
        PlayerController.instance.isActivatingSkill = true;

        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
       // transform.rotation = Quaternion.LookRotation(fireDir);

        while (shotCount < 4)
        {
            GameObject muzzle = Instantiate(_muzzleEffect,
                  _firePos.transform.position,
                  _firePos.transform.rotation);
            StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

            GameObject projectile = GameObject.Instantiate(_projectile,
                _firePos.transform.position,
                _firePos.transform.rotation);
            projectile.GetComponent<Projectile_Colt>().TheStart(
                _playerStats._bulletVelocity,
                _playerStats._damage
                , _playerStats._bulletRange
                , true);

            shotCount++;
            yield return new WaitForSeconds(0.2f);
        }

        PlayerController.instance.isActivatingSkill = false;
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

    IEnumerator DrawAttackIndicator()
    {
        Vector3 fireDir;
        Vector3 endPoint;
        _shotIndicator.material = PlayerController.instance._attackIncatorMat;

        while (true)
        {
            fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, fireDir, out hit, _playerStats._bulletRange, LayerMask.NameToLayer("Grass")))
                endPoint = hit.point;
            else
                endPoint = transform.position + fireDir.normalized * _playerStats._bulletRange;

            endPoint.y = 1;
            _shotIndicator.SetPosition(0, transform.position + new Vector3(0, 1, 0));
            _shotIndicator.SetPosition(1, endPoint);
            yield return null;
        }
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
}
