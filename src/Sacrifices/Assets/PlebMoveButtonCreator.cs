using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlebMoveButtonCreator : MonoBehaviour {

    public GameObject MoveButtonPrefab;
    public GamePhaseManager manager;

    private List<GameObject> existingButtons = new List<GameObject>();
    	
    public void CreateButtons()
    {
        foreach (var gob in existingButtons)
        {
            Destroy(gob);
        }
        existingButtons.Clear();
        var plebs = manager.Plebs
            .Where(p => p.Position.CurrentLocation == manager.SelectedLocation)
            .OrderBy(p => p.Position.MarchingOrder)
            .Skip(1);
        foreach (var pleb in plebs)
        {
            var gob = Instantiate(MoveButtonPrefab, gameObject.transform);
            gob.GetComponent<Button>().onClick.AddListener(() => manager.PhaseAction($"move`{pleb.Name}"));
            gob.GetComponent<Button>().onClick.AddListener(() => manager.MoveButton.SetActive(false));
            gob.GetComponentInChildren<Text>().text = pleb.Name;
            existingButtons.Add(gob);
        }
    }
}
