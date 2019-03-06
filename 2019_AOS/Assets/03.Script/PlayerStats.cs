using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {
    public GameObject _playerInfoCanvas;

    public Vector3 _offset;
    public Vector3 _rot;
    public string _name;
    public int _health;
    public int _damage;
    public float _bulletVelocity;
    public float _bulletRange;
    public int _ammo;
    public float _reloadTime;

    public int _specialAttackEnergyLimit;
    int _specialAttackEnergy;

    int _score;
    int _currentAmmo;
    bool _reload;
    float _currentReloadAmount;

    GameObject _healthBar;
    GameObject _healthTxt;
    GameObject _ammoBar;
    GameObject _reloadBar;
    GameObject _ID;

    // Use this for initialization
    void Start () {
        _offset = new Vector3(0, 3.5f, 0.6f);
        _rot = new Vector3(68, 0, 0);

        _playerInfoCanvas = Instantiate(_playerInfoCanvas);
        _playerInfoCanvas.GetComponent<RectTransform>().position =
    transform.position + _offset;
        _playerInfoCanvas.GetComponent<RectTransform>().rotation = Quaternion.Euler(_rot);

        _healthBar = _playerInfoCanvas.GetComponent<PlayerUI>().healthBar;
        _healthTxt = _playerInfoCanvas.GetComponent<PlayerUI>().healthTxt;
        _ammoBar = _playerInfoCanvas.GetComponent<PlayerUI>().ammoBar;
        _reloadBar  = _playerInfoCanvas.GetComponent<PlayerUI>().reloadBar;
        _ID = _playerInfoCanvas.GetComponent<PlayerUI>().ID;

        _reload = false;
        _currentAmmo = _ammo;
        _currentReloadAmount = 1f;

        _specialAttackEnergy = 0;
    }

    // Update is called once per frame
    void Update () {
        _playerInfoCanvas.GetComponent<RectTransform>().position =
    transform.position + _offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
    }

    public bool onFire()
    {
        if (_currentAmmo <= 0) return false;

        _currentAmmo--;

        _ammoBar.GetComponent<Image>().fillAmount -= (float)1 / _ammo;
        _reloadBar.GetComponent<Image>().fillAmount -= (float)1 / _ammo;
        _reload = true;

        _currentReloadAmount = _reloadBar.GetComponent<Image>().fillAmount;
        StopAllCoroutines();
        StartCoroutine(Reload());

        return true;
    }

    IEnumerator Reload()
    {
        float startTime = Time.time;
        Image reloadBarImage = _reloadBar.GetComponent<Image>();

        while (_currentAmmo < _ammo)
        {
            reloadBarImage.fillAmount = _currentReloadAmount + 
                (Time.time - startTime) / ((float)_reloadTime * _ammo) ;

            int i = 0; 
            while(i <= _ammo)
            {
                if(reloadBarImage.fillAmount >= (float)(_ammo-i) / _ammo)
                {
                    _currentAmmo = _ammo - i;
                    _ammoBar.GetComponent<Image>().fillAmount = (float)(_ammo - i) / _ammo;
                    break;
                }
                i++;
            }
            yield return null;
        }
    }

    public void HitCompetition(GameObject gameObject)
    {
        Debug.Log("HitCompetition in PlayerStats");
        _specialAttackEnergy += _damage;
        SpecialAttack.instance._gage.GetComponent<Image>().fillAmount = 
            (float)_specialAttackEnergy / _specialAttackEnergyLimit;


    }
}
