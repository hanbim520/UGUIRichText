using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDGame.UI.RichText;

public class RichTextTest : MonoBehaviour {
    public RichText text; 
	// Use this for initialization
	void Start () {
        text.onClick.AddListener((string param) => {
            Debug.Log("param: " + param);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
