using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Barley : MonoBehaviour {
    float _gravity = 9.8f;
    float _forward_velovity = 5f;
    float _bullet_up_speed;
    float _time = 0f;

    Vector3 _rotateDir;
    public float _rotSpeed = 10;
    Quaternion targetRotation;


    public GameObject _trail;
    public GameObject _playerHitParticle;
    public GameObject _explosionPlarticle;

    GameObject _rockhitParticle;
    GameObject _rockExplosionParticle;
    GameObject _grassParticle;

    public GameObject _bulletMeshObj;
    Transform _meshTr;

    GameObject _playerController;

    public void Start () {
        _gravity = 9.8f;
        _forward_velovity = 5f;
        _bullet_up_speed = 9.8f;
        _time = 0f;

        _meshTr = _bulletMeshObj.GetComponent<Transform>();
        targetRotation = _meshTr.rotation;

        Debug.Log(transform.forward + " - " + transform.forward.magnitude);
    }

    // Update is called once per frame
    void Update() {
        if (_time > 2f) return;

        Vector3 newPos = new Vector3(
            transform.forward.x * _forward_velovity * Time.deltaTime,
            (_bullet_up_speed - _gravity * _time) * Time.deltaTime,
            transform.forward.z * _forward_velovity * Time.deltaTime);

        transform.position += newPos;

        targetRotation *= Quaternion.AngleAxis(10, Vector3.left);
        _meshTr.rotation =
            Quaternion.Lerp(_meshTr.rotation, 
            targetRotation, 
            10 * _rotSpeed * Time.deltaTime);
           
        _time += Time.deltaTime;

        //transform.position 
    }
}
