using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMainUI : MonoBehaviour {
    #region Singleton
    public static InGameMainUI instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }
    #endregion
    public GameObject _AttackStick_Panel;
    public GameObject _MovementStick_Panel;
    public GameObject _SkillSitck_Panel;
    public GameObject _ScoreBar_Ourside;
    public GameObject _ScoreBar_Enemy;

    public JoyStickController AttackStick { get; private set; }
    public JoyStickController MovementStick { get; private set; }
    public float _resolutionWidthRatio { get; private set; }
    public float _resolutionHeightRatio { get; private set; }
    // Use this for initialization
    void Start () {
        AttackStick = _AttackStick_Panel.GetComponent<JoyStickController>();
        MovementStick = _MovementStick_Panel.GetComponent<JoyStickController>();

        _resolutionWidthRatio = (float)Screen.currentResolution.width / 1280;
        _resolutionHeightRatio = (float)Screen.currentResolution.width / 800;
            //Debug.Log(Screen.currentResolution.width + " - " + ratio);
        //_MovementStick_Panel.GetComponent<RectTransform>().localScale *= ratio;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
