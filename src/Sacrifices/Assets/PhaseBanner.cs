using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseBanner : MonoBehaviour {
    public string UpperText = "Spawn Phase";
    public Text phaseText;
    public Image image1;
    public Image image2;
    public GamePhaseManager Manager;

    void Start()
    {
        image1.enabled = false;
        image2.enabled = false;
        phaseText.CrossFadeAlpha(0, 0.01f, true);
    }
	public void Activate()
    {
        StartCoroutine(ActivateCo());
    }
    private IEnumerator ActivateCo()
    {
        phaseText.text = $"{UpperText}\nSoul Remaining: {Manager.Soul}";
        image1.enabled = true;
        phaseText.enabled = true;
        image1.fillAmount = 0;
        float startTime = Time.time;
        while (Time.time < startTime + 0.5f)
        {
            image1.fillAmount += Time.deltaTime * 2;
            yield return null;
        }
        phaseText.CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(2.5f);
        phaseText.CrossFadeAlpha(0, 0.2f, false);
        startTime = Time.time;
        image1.enabled = false;
        image2.enabled = true;
        image2.fillAmount = 1;
        while (Time.time < startTime + 0.5f)
        {
            image2.fillAmount -= Time.deltaTime * 2;
            yield return null;
        }
        image2.enabled = false;
        phaseText.enabled = false;
    }
}
