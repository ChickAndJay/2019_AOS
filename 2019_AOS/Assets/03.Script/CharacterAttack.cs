using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterAttack : MonoBehaviour {
    protected Vector3 _fireDir;
    protected PlayerStats _playerStats;
    protected GameObject _firePos;

    public GameObject _projectile;
    public GameObject _muzzleEffect;
    
    protected void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _firePos = PlayerController.instance._firePos;
    }

    virtual public void StartDrawIndicator(bool isSkill)   {    }

    virtual public void StopDrawIndicator()    {    }

    virtual public void FireBaseAttack()   {    }

    virtual public void FireSkillAttack()    {    }
}
