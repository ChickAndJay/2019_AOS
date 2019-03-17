using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public float _rotateSpeed;
    Quaternion _targetRotation;

    public AudioClip _createSound;
    public AudioClip _absortionSound;
    AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _targetRotation = transform.rotation;
        GetComponent<SphereCollider>().enabled = false;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _createSound;
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= 0.3f)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<SphereCollider>().enabled = true;

        }
        _targetRotation *= Quaternion.AngleAxis(10, Vector3.up);
        transform.rotation =
            Quaternion.Lerp(transform.rotation,
            _targetRotation,
            10 * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") ||              
            other.CompareTag("Company") || 
            other.CompareTag("Competition"))
        {
            StartCoroutine(AbsorptedToCollider(other));
            other.GetComponent<PlayerStats>().IncreaseScore();
        }
    }

    IEnumerator AbsorptedToCollider(Collider other)
    {

        float time = 0;
        float absortionTime = 0.7f;
        Vector3 startPos = transform.position;
        Vector3 originalScale = transform.localScale;

        _audioSource.clip = _absortionSound;
        _audioSource.Play();

        while (time < absortionTime)
        {
            time += Time.deltaTime;

            transform.position = 
                Vector3.Lerp(
                    startPos, 
                    other.transform.position + new Vector3(0,1,0), 
                    time / absortionTime);
            transform.localScale = 
                Vector3.Lerp(
                    originalScale, 
                    new Vector3(0, 0, 0), 
                    time / absortionTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}
