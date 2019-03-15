using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour {
    public GameObject _playerInfoCanvas;

    Vector3 _offset;
    Vector3 _rot;

    public CharacterInfo _characterInfo;

    #region Character Information
    //public FightType _fightType { get; private set;}
    public CharacterAttack _attackScript { get; private set; }
    public FightType _fightType { get; private set; }
    public int _health { get; private set; }
    public int _damage { get; private set; }
    public float _bulletVelocity { get; private set; }
    public float _bulletRange { get; private set; }
    public int _ammo { get; private set; }
    public float _reloadTime { get; private set; }

    public int _skillEnergyLimit { get; private set; }
    public float _skillRange { get; private set; }
    public float _skillDamage { get; private set; }
    #endregion

    #region OnGame Information
    public string _name;

    int _score;
    int _currentAmmo;
    bool _reload;
    float _currentReloadAmount;
    public bool _skillFireReady { get; private set; }
    int _skillEnergy;
    #endregion

    #region PlayerInfo Canvas
    GameObject _healthBar;
    GameObject _healthTxt;
    GameObject _ammoBar;
    GameObject _reloadBar;
    GameObject _ID;
    GameObject _scoreTxt;
    #endregion

    #region Skill UI
    public Sprite _skillBG_Before;
    public Sprite _skillBG_After;
    public Sprite _skillImg_Before;
    public Sprite _skillImg_After;
    #endregion
 

    void Start () {
        _offset = new Vector3(0, 3.5f, 0.6f);
        _rot = new Vector3(68, 0, 0);
        _score = 0;

        #region CharacterInformation Setting      
        _attackScript = GetComponent<CharacterAttack>();

        _fightType = _characterInfo._fightType;
        _health = _characterInfo._health;
        _damage = _characterInfo._damage;
        _bulletVelocity = _characterInfo._bulletVelocity;
        _bulletRange = _characterInfo._bulletRange;
        _ammo = _characterInfo._ammo;
        _reloadTime = _characterInfo._reloadTime;

        _skillEnergyLimit = _characterInfo._skillEnergyLimit;
        _skillRange = _characterInfo._skillRange;
        _skillDamage = _characterInfo._skillDamage;

       #endregion

        #region PlayerInfo Canvas Setting
        _playerInfoCanvas = Instantiate(_playerInfoCanvas);
        _playerInfoCanvas.GetComponent<RectTransform>().position =
    transform.position + _offset;
        _playerInfoCanvas.GetComponent<RectTransform>().rotation = Quaternion.Euler(_rot);

        _healthBar = _playerInfoCanvas.GetComponent<PlayerUI>().healthBar;
        _healthTxt = _playerInfoCanvas.GetComponent<PlayerUI>().healthTxt;
        _ammoBar = _playerInfoCanvas.GetComponent<PlayerUI>().ammoBar;
        _reloadBar  = _playerInfoCanvas.GetComponent<PlayerUI>().reloadBar;
        _ID = _playerInfoCanvas.GetComponent<PlayerUI>().ID;
        _scoreTxt = _playerInfoCanvas.GetComponent<PlayerUI>().scoreTxt;
        _scoreTxt.GetComponent<Text>().text = _score.ToString() ;
        #endregion

        _reload = false;
        _currentAmmo = _ammo;
        _currentReloadAmount = 1f;
        _skillEnergy = 0;
        _skillFireReady = false;
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

        _currentReloadAmount -= (float)1 / _ammo;
        if (gameObject.CompareTag("Player"))
        {
            _ammoBar.GetComponent<Image>().fillAmount = _currentReloadAmount;
            _reloadBar.GetComponent<Image>().fillAmount = _currentReloadAmount;
        }
        _reload = true;
        StopAllCoroutines();
        StartCoroutine(Reload());

        return true;
    }

    IEnumerator Reload()
    {
        float startTime = Time.time;
        Image reloadBarImage = _reloadBar.GetComponent<Image>();
        float _reloadamount = _currentReloadAmount;
        while (_currentAmmo < _ammo)
        {
            _currentReloadAmount = _reloadamount + (Time.time - startTime) / ((float)_reloadTime * _ammo);

            if (gameObject.CompareTag("Player"))
                reloadBarImage.fillAmount = _currentReloadAmount;

            int i = 0; 
            while(i <= _ammo)
            {
                if(reloadBarImage.fillAmount >= (float)(_ammo-i) / _ammo)
                {
                    _currentAmmo = _ammo - i;
                    if(gameObject.CompareTag("Player"))
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
        _skillEnergy += _damage;
        if(_skillEnergy >= _skillEnergyLimit && this.gameObject.CompareTag("Player"))
        {
            SkillCanvas.instance._gage.GetComponent<Image>().enabled = false ;
            SkillCanvas.instance._outterCircle.GetComponent<Image>().sprite =
                _skillBG_After;
            SkillCanvas.instance._innerCircle.GetComponent<Image>().sprite =
                _skillImg_After;

            SkillCanvas.instance._innerCircle.GetComponent<Image>().raycastTarget = true;
            _skillFireReady = true;
        }
        SkillCanvas.instance._gage.GetComponent<Image>().fillAmount = 
            (float)_skillEnergy / _skillEnergyLimit;
    }

    public void InitializeSkillGage()
    {
        _skillEnergy = 0;
        _skillFireReady = false;

        if (!gameObject.CompareTag("Player")) return;
        SkillCanvas.instance._gage.GetComponent<Image>().enabled = true;
        SkillCanvas.instance._gage.GetComponent<Image>().fillAmount = 0;
        SkillCanvas.instance._innerCircle.GetComponent<Image>().raycastTarget = false;

        SkillCanvas.instance._outterCircle.GetComponent<Image>().sprite =
                _skillBG_Before;
        SkillCanvas.instance._innerCircle.GetComponent<Image>().sprite =
            _skillImg_Before;
    }

    public void IncreaseScore()
    {
        _score++;
        if (!gameObject.CompareTag("Competition")) {
            InGameMainUI.instance._ScoreBar_Ourside._scoreText.text = _score.ToString();
            InGameMainUI.instance._ScoreBar_Ourside._scoreFilled[_score - 1].enabled = true;

            GameManagerScript.instance.IncreaseOurScore();
            _scoreTxt.GetComponent<Text>().text = _score.ToString();
        }
        else
        {
            InGameMainUI.instance._ScoreBar_Enemy._scoreText.text = _score.ToString();
            InGameMainUI.instance._ScoreBar_Enemy._scoreFilled[_score - 1].enabled = true;

            GameManagerScript.instance.IncreaseEnemyScore();
            _scoreTxt.GetComponent<Text>().text = _score.ToString();
        }
    }
}
