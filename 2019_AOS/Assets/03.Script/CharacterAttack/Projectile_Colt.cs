using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Basic
}

public class Projectile_Colt : Projectile
{
    float _range;
    float _velocity;

    Vector3 _startPos;
    Vector3 _endPos;
    float _startTime;
    float _journeyLength;

	// Use this for initialization

    // not unity default Start method
    public void TheStart(float velocity, int damage, float range,bool isSkill)
    {
        _velocity = velocity; _damage = damage; _range = range;
        _isSkill = isSkill;

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
                StartCoroutine(DestroySelf(1.0f));
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
                    StartCoroutine(DestroySelf(1.0f));
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
                    PlayerController.instance._playerStats.HitCompetition(other.gameObject);
                }
                StartCoroutine(DestroySelf(1.0f));
            }
            
        }
    }

    protected override IEnumerator DestroySelf(float time)
    {
        _hit = true;
        _bulletMeshObj.GetComponent<Renderer>().enabled = false;
        _explosionPlarticle.GetComponent<ParticleSystem>().Play();
        _trail.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
