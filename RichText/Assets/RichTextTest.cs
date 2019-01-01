using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDGame.UI.RichText;

public class RichTextTest : MonoBehaviour {
    public RichText text; 
	// Use this for initialization
	void Start () {
        text.onClick.AddListener((string tag)=> {
            Debug.Log("Clicked: " + tag);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
