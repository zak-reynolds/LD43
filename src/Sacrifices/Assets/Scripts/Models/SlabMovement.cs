using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlabMovement : MonoBehaviour {
    //according to https://www.youtube.com/watch?v=4R_AdDK25kQ&t=9s
    private Vector3 loweredPos;
    private Vector3 liftedPos;
    private Vector3 nextPos;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform childTransform;
    [SerializeField]
    private Transform transformLifted;

	// Use this for initialization
	void Start () {
        loweredPos = childTransform.localPosition;
        liftedPos = transformLifted.localPosition; 
        nextPos = liftedPos;
	}
	
	// Update is called once per frame
	void Update () {
        move();
	}

    private void move()
    {
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPos, speed * Time.deltaTime);
    }
}
