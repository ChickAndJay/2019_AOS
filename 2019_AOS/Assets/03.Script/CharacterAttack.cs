using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterAttack : MonoBehaviour {
    public GameObject _upperSpine;
    protected Vector3 _fireDir;
    protected PlayerStats _playerStats;
    protected GameObject _firePos;

    public GameObject _projectile;
    public GameObject _muzzleEffect;

    protected Animator _animator;
    protected Vector3 _upperBodyDir;
    protected bool rotate = false;

    protected void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _firePos = PlayerController.instance._firePos;
        _animator = GetComponentInChildren<Animator>();
    }

    private void LateUpdate()
    {
        if (rotate)
        {
            Vector3 spineRot = Quaternion.LookRotation(_upperBodyDir).eulerAngles;
            spineRot -= transform.eulerAngles;

            _upperSpine.transform.localRotation = Quaternion.Euler(
                _upperSpine.transform.localEulerAngles.x + spineRot.y,
                _upperSpine.transform.localEulerAngles.y,
                _upperSpine.transform.localEulerAngles.z
                );
        }
    }

    virtual public void StartDrawIndicator(bool isSkill)   {    }

    virtual public void StopDrawIndicator()    {    }

    virtual public void FireBaseAttack()   {    }

    virtual public void FireSkillAttack()    {    }
}
