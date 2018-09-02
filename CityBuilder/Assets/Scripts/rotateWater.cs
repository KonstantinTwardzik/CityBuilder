using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateWater : MonoBehaviour {

    public GameObject water;
    public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        water.transform.Rotate(0, speed * Time.deltaTime, 0);
	}
}
