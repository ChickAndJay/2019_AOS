using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Basic
}

public class Projectile : MonoBehaviour {
    public ProjectileType projectileType;

    float velocity;
    int damage;
    float range;

    public GameObject Trail;
    public GameObject RockhitParticle;
    public GameObject PlayerHitParticle;
    public GameObject ExplosionPlarticle;

    public Renderer BulletMesh;

    GameObject playerController;

    Vector3 startPos;
    Vector3 endPos;
    float startTime;
    float journeyLength;

    bool hit;
	// Use this for initialization

    // not unity default Start method
    public void TheStart(float velocity, int damage, float range, GameObject playerController)
    {
        this.velocity = velocity; this.damage = damage; this.range = range;
        this.playerController = playerController;

        startPos = transform.position;
        Vector3 dir = transform.forward.normalized * range;
        endPos = dir;
        endPos += startPos;
        journeyLength = (endPos - startPos).magnitude;
        startTime = Time.time;
        hit = false;
    }

    // Update is called once per frame
    void Update () {
        if (!hit)
        {
            float distCovered = (Time.time - startTime) * velocity;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
            if (fracJourney >= 1.0f)
            {
                StartCoroutine(DestroySelf());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hit) // 한놈만 패려고 
        {
            if (other.CompareTag("NotGrass"))
            {
                GameObject particle = Instantiate(RockhitParticle, transform.position, transform.rotation);

                StartCoroutine(DistortCollider(other));
                StartCoroutine(DestroySelf());
            }
            else if (other.CompareTag("Competition"))
            {
                playerController.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
                StartCoroutine(DestroySelf());
            }
            else
            {

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
        hit = true;
        BulletMesh.enabled = false;
        ExplosionPlarticle.GetComponent<ParticleSystem>().Play();
        Trail.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
