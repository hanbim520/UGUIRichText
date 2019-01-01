using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Button loadDrawCall;
    public Button loadBloodBar;
	// Use this for initialization
	void Start () {

        loadDrawCall.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DrawCallTest");
        });

        loadBloodBar.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BloodBarTest");
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
