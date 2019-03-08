using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Barley : Projectile{
    float _gravity ;
    float _forward_velovity ;
    float _bullet_up_speed;
    float _startTime ;
    float _maxTime;
    float _duration;

    float _gravityTime;
    float _yPos;

    public float _rotSpeed = 10;
    Quaternion targetRotation;

    Transform _meshTr;

    Vector3 _startPosition;
    Vector3 _landingPosition;

    public float _integrationRatio = 0.8f;

    public void TheStart(int damage, bool isSkill, Vector3 landingPosition )
    {
        _damage = damage; _isSkill = isSkill; 
        _landingPosition = landingPosition;

        _gravity = 9.8f;
        _forward_velovity = 5f;
        _startTime = Time.time;
        _maxTime = 2f;

        _startPosition = transform.position;
        float length = (landingPosition -transform.position).magnitude;
        _duration = (length / PlayerController.instance._playerStats._bulletRange) * _maxTime;
        _bullet_up_speed = 9.8f * (length / PlayerController.instance._playerStats._bulletRange);
        _gravityTime = 0f;
        Debug.Log(length / PlayerController.instance._playerStats._bulletRange);
        _meshTr = _bulletMeshObj.GetComponent<Transform>();
        targetRotation = _meshTr.rotation;
        _yPos = 0f;

        _hit = false;
    }
    
    // Update is called once per frame
    void Update() {
        if (_hit) return;

        _gravityTime += Time.deltaTime;
        float forwardCovered = (Time.time - _startTime) * _forward_velovity;
        float forwardJourney = forwardCovered / _landingPosition.magnitude;

        Vector3 endPos = new Vector3(_landingPosition.x, 0, _landingPosition.z);
        _yPos += (_bullet_up_speed - _gravity * _gravityTime) * Time.deltaTime * _integrationRatio;
        transform.position = Vector3.Lerp(_startPosition, endPos, forwardJourney);        
        Debug.Log(transform.position + " - " + (_bullet_up_speed - _gravity * _gravityTime) * Time.deltaTime * _integrationRatio);
        transform.position += new Vector3(
            0,
            _yPos,
            0);

        if (forwardJourney >= 1.0f)
        {
            _hit = true;
        }
        //transform.position += newPos;

        //targetRotation *= Quaternion.AngleAxis(10, Vector3.left);
        //_meshTr.rotation =
        //    Quaternion.Lerp(_meshTr.rotation, 
        //    targetRotation, 
        //    10 * _rotSpeed * Time.deltaTime);


        //transform.position 
    }

    protected override IEnumerator DestroySelf(float time)
    {
        throw new System.NotImplementedException();
    }
}
