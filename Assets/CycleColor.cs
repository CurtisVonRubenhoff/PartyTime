using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleColor : MonoBehaviour {

  [SerializeField]
  Camera thisCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    float h, s, v;

    var curr = thisCamera.backgroundColor;
    Color.RGBToHSV(curr, out h, out s, out v);
    h += .001f;
    if (h > 1.0f) h = 0f;

    thisCamera.backgroundColor = Color.HSVToRGB(h, s, v);
	}
}
