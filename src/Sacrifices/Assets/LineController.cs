using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour {

    public GameObject plebPrototype;
    public float speed = 8;
    public LineController next;

    private List<GameObject> plebGobs = new List<GameObject>(10);
    private float plebSpacing = 2f;

    public void SpawnPleb(object pleb)
    {
        Pleb p = (Pleb)pleb;
        GameObject plebGob = Instantiate(plebPrototype, transform.position + new Vector3(-plebSpacing * plebGobs.Count, 0), Quaternion.identity, transform);
        plebGob.name = p.Name;
        plebGob.GetComponentInChildren<TextMesh>().text = $"{p.Name}\nSTR: 0\nVIRGIN";
        plebGobs.Add(plebGob);
    }

    public void TransferPleb()
    {
        var gob = plebGobs[0];
        plebGobs.RemoveAt(0);
        next.plebGobs.Add(gob);
        Debug.Log($"Transferring {gob.name}");
        StartCoroutine(MovePleb(
            next.plebGobs.Count - 1,
            gob,
            next.gameObject.transform
        ));
    }

    public void FightPlebs(int a, int b, int winner)
    {
        StartCoroutine(FightPlebsCo(a, b, winner));
    }
    private IEnumerator FightPlebsCo(int a, int b, int winner)
    {
        var animA = plebGobs[a].GetComponentInChildren<Animator>();
        var animB = plebGobs[b].GetComponentInChildren<Animator>();
        animA.SetTrigger("Fight");
        animB.SetTrigger("Fight");
        yield return new WaitForSeconds(1);
        animA.SetTrigger(winner == a ? "Stop" : "Kill");
        animA.SetTrigger(winner == b ? "Stop" : "Kill");
        yield return new WaitForSeconds(2);
        int loser = winner == a ? b : a;
        Destroy(plebGobs[loser]);
        plebGobs.RemoveAt(loser);
        RefreshOrder();
    }

    public void KissPlebs(int a, int b)
    {
        StartCoroutine(KissPlebsCo(a, b));
    }

    public void MovePleb(Pleb target)
    {
        int i = plebGobs.FindIndex(g => g.name == target.Name);
        var gob = plebGobs[i];
        plebGobs.RemoveAt(i);
        plebGobs.Insert(0, gob);
        RefreshOrder();
    }

    private IEnumerator KissPlebsCo(int a, int b)
    {
        var animA = plebGobs[a].GetComponentInChildren<Animator>();
        var animB = plebGobs[b].GetComponentInChildren<Animator>();
        animA.SetTrigger("Kiss");
        animB.SetTrigger("Kiss");
        yield return new WaitForSeconds(1);
        animA.SetTrigger("Stop");
        animB.SetTrigger("Stop");
    }


    public void ConsumePleb()
    {
        StartCoroutine(ConsumePlebCo());
    }
    private IEnumerator ConsumePlebCo()
    {
        var anim = plebGobs[0].GetComponentInChildren<Animator>();
        anim.SetTrigger("Consume");
        yield return new WaitForSeconds(2);
        Destroy(plebGobs[0]);
        plebGobs.RemoveAt(0);
    }


    public void HoldTheDoor(Pleb p, int moversCount)
    {
        StartCoroutine(HoldTheDoorCo(p, moversCount));
    }
    private IEnumerator HoldTheDoorCo(Pleb p, int moversCount)
    {
        Debug.Log("Holding the door");
        RefreshOrder(-1);
        yield return new WaitForSeconds(2);
        var gob = plebGobs[0];
        plebGobs.RemoveAt(0);
        var anim = gob.GetComponentInChildren<Animator>();
        anim.SetTrigger("Lift");
        yield return new WaitForSeconds(1);
        for (int i = 0; i < moversCount; ++i)
        {
            TransferPleb();
        }
        yield return new WaitForSeconds(p.Strength * 0.5f);
        anim.SetTrigger("Kill");
        yield return new WaitForSeconds(p.Strength * 0.5f);
        Destroy(anim.gameObject);
        RefreshOrder();
    }

    public void RefreshOrder(int offset = 0)
    {
        Debug.Log($"Refreshing order offset {offset}");
        for (int i = 0; i < plebGobs.Count; ++i)
        {
            StartCoroutine(MovePleb(i + offset, plebGobs[i], transform));
        }
    }

    private IEnumerator MovePleb(int targetPosition, GameObject plebGob, Transform targetLine)
    {
        Debug.Log($"Moving {plebGob.name} to {targetLine.gameObject.name} / {targetPosition}");
        var sprite = plebGob.GetComponentInChildren<SpriteRenderer>();
        var anim = plebGob.GetComponentInChildren<Animator>();
        anim.SetTrigger("Walk");
        plebGob.GetComponent<PlebMover>().TargetPosition = targetLine.position + new Vector3(-plebSpacing * targetPosition, 0, 0);
        yield return new WaitForSeconds(1.1f);
        anim.SetTrigger("Stop");
        sprite.flipX = false;
        Debug.Log($"Moved {plebGob.name} to {targetLine.gameObject.name} / {targetPosition}");
    }
}
