using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    Timer t;
    // Use this for initialization
    void Start () {
      t = TimerManager.GetInstance().CreateTimer(2f, TimerType.Once);
        t.OnComplete += () => {
            Debug.Log("finished");
           // TimerManager.GetInstance().RemoveTimer(t.timerId );
        };
        t.Start();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(t);
    }
}
