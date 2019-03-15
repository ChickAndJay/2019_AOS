using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarleyPlaneAttack : MonoBehaviour {
    int _damage;

    List<GameObject> _enteredEnemy;
    Dictionary<GameObject, float> _dictionary;
	// Use this for initialization
	public void TheStart (int damage) {
        _damage = damage;
        _dictionary = new Dictionary<GameObject, float>();

        StartCoroutine(DestorySelf(2.0f));
	}
	
    IEnumerator DestorySelf(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponentInChildren<SphereCollider>().enabled = false;

        float startTime = 0;
        float destroyTime = 0.3f;
        while (startTime <= destroyTime)
        {
            startTime += Time.deltaTime;

            transform.localScale =
                new Vector3(0.2f - 0.2f * startTime / destroyTime,
                0.2f - 0.2f * startTime / destroyTime,
                0.2f - 0.2f * startTime / destroyTime);

            yield return null;
        }

        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Competition"))
        {
            PlayerController.instance._playerStats.HitCompetition(other.gameObject);
            _dictionary.Add(other.gameObject, Time.time);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Competition"))
        {
            float t = _dictionary[other.gameObject];
            if (Time.time - t >= 1.2f)
            {
                PlayerController.instance._playerStats.HitCompetition(other.gameObject);
                _dictionary[other.gameObject] = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Competition"))
        {
            _dictionary.Remove(other.gameObject);
        }
    }
}
