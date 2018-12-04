using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlabMovement : MonoBehaviour {
    //according to https://www.youtube.com/watch?v=4R_AdDK25kQ&t=9s
    private Vector3 loweredPos;
    private Vector3 liftedPos;
    public float height = 0.5f;
    public float liftTime = 1.5f;
    public float liftDelay = 2f;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform childTransform;
    public GamePhaseManager manager;

	void Start () {
        liftedPos = transform.position + (Vector3.up * height);
        loweredPos = transform.position;
    }
	
    private float startTime;

    public void Open(int numPlebs)
    {
        StartCoroutine(OpenDoor(numPlebs));
    }

    private Vector3 currentVelocity = Vector3.zero;

    private IEnumerator OpenDoor(int numPlebs)
    {
        yield return new WaitForSeconds(liftDelay);
        startTime = Time.time;
        while (Time.time < startTime + liftTime)
        {
            childTransform.position = Vector3.SmoothDamp(childTransform.position, liftedPos, ref currentVelocity, 0.5f);
            yield return null;
        }
        yield return new WaitForSeconds(numPlebs * 0.5f);
        manager.AudioSource.PlayOneShot(manager.SlabSlam);
        while (childTransform.position.y > loweredPos.y)
        {
            childTransform.position = childTransform.position + (Vector3.down * Time.deltaTime * speed);
            yield return null;
        }
        childTransform.position = loweredPos;
    }
}
