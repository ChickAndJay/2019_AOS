using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Basic
}

public class Projectile : MonoBehaviour {
    public ProjectileType _projectileType;

    float _velocity;
    int _damage;
    float _range;
    bool _isSkill;

    public GameObject _trail;
    public GameObject _playerHitParticle;
    public GameObject _explosionPlarticle;

    GameObject _rockhitParticle;
    GameObject _rockExplosionParticle;
    GameObject _grassParticle;

    public Renderer _bulletMesh;

    GameObject _playerController;

    Vector3 _startPos;
    Vector3 _endPos;
    float _startTime;
    float _journeyLength;

    bool _hit;
	// Use this for initialization

    // not unity default Start method
    public void TheStart(float velocity, int damage, float range,bool isSkill,
        GameObject playerController)
    {
        _velocity = velocity; _damage = damage; _range = range;
        _isSkill = isSkill;
        _playerController = playerController;

        _rockhitParticle = Resources.Load<GameObject>("Particles/RockHitParticle");
        _rockExplosionParticle = Resources.Load<GameObject>("Particles/Rock_Explode_Particle");
        _grassParticle = Resources.Load<GameObject>("Particles/Grass_Explode_Particle");

        _startPos = transform.position;
        Vector3 dir = transform.forward.normalized * range;
        _endPos = dir;
        _endPos += _startPos;
        _journeyLength = (_endPos - _startPos).magnitude;
        _startTime = Time.time;
        _hit = false;
    }

    // Update is called once per frame
    void Update () {
        if (!_hit)
        {
            float distCovered = (Time.time - _startTime) * _velocity;
            float fracJourney = distCovered / _journeyLength;
            transform.position = Vector3.Lerp(_startPos, _endPos, fracJourney);
            if (fracJourney >= 1.0f)
            {
                StartCoroutine(DestroySelf());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hit) // 한놈만 패려고 
        {
            if (other.CompareTag("NotGrass"))
            {
                if (_isSkill)
                {
                    GameObject particle = Instantiate(_rockExplosionParticle, other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                }
                else
                {
                    GameObject particle = Instantiate(_rockhitParticle, transform.position, transform.rotation);
                    StartCoroutine(DistortCollider(other));
                    StartCoroutine(DestroySelf());
                }
            }
            else if (other.CompareTag("Grass"))
            {
                if (_isSkill)
                {
                    GameObject particle = Instantiate(_grassParticle, other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                }

            }
            else if (other.CompareTag("Competition"))
            {
                if (_isSkill)
                    ;
                else
                {
                    _playerController.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
                }
                StartCoroutine(DestroySelf());
            }
            
        }
    }

    IEnumerator DistortCollider(Collider other)
    {
        float startTime = Time.time;
        float timeChecker = 0 ;
        Vector3 origin = other.transform.localScale;
        // 크게
        while (timeChecker <= 0.05f)
        {
            timeChecker = Time.time - startTime;
            other.transform.localScale = new Vector3(other.transform.localScale.x + Time.deltaTime,
                other.transform.localScale.y + Time.deltaTime,
                other.transform.localScale.z + Time.deltaTime);
            yield return null;
        }
        startTime = Time.time;
        timeChecker = 0;
        // 작게
        while (timeChecker <= 0.1f)
        {
            timeChecker = Time.time - startTime;
            other.transform.localScale = new Vector3(other.transform.localScale.x - Time.deltaTime,
                other.transform.localScale.y - Time.deltaTime,
                other.transform.localScale.z - Time.deltaTime);
            yield return null;
        }
        startTime = Time.time;
        timeChecker = 0;
        // 크게
        while (timeChecker <= 0.05f)
        {
            timeChecker = Time.time - startTime;
            other.transform.localScale = new Vector3(other.transform.localScale.x + Time.deltaTime,
                other.transform.localScale.y + Time.deltaTime,
                other.transform.localScale.z + Time.deltaTime);
            yield return null;
        }

        other.transform.localScale = origin;
    }

    IEnumerator DestroySelf()
    {
        _hit = true;
        _bulletMesh.enabled = false;
        _explosionPlarticle.GetComponent<ParticleSystem>().Play();
        _trail.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
