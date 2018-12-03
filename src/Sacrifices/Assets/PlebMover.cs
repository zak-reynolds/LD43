using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlebMover : MonoBehaviour {

    public Vector3 TargetPosition {
        set {
            gameObject.GetComponentInChildren<SpriteRenderer>().flipX = targetPosition.x < transform.position.x;
            targetPosition = value;
        }
    }
    private Vector3 targetPosition;
    private Vector3 currentVelocity = Vector3.zero;

    void Awake()
    {
        targetPosition = transform.position;
    }

	void Update () {
		if (transform.position != targetPosition)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f);
        }
	}
}
