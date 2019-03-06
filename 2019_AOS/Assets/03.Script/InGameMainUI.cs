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

    public JoyStickController AttackStick;
    public JoyStickController MovementStick;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
