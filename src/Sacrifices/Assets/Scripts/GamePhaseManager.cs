using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GamePhaseManager : MonoBehaviour {
    [Header("References")]
    public Text SoulCounter;
    public Text PlebListText;

    [Header("Game Balance")]
    public int StartingSoul = 20;

    private int soul;
    private List<Pleb> plebs;
    private Dictionary<Position.Location, List<Pleb>> locationReference;

    void Start()
    {
        soul = StartingSoul;
        plebs = new List<Pleb>(50);
        locationReference = new Dictionary<Position.Location, List<Pleb>>(4);
        locationReference.Add(Position.Location.A, new List<Pleb>(10));
        locationReference.Add(Position.Location.B, new List<Pleb>(10));
        locationReference.Add(Position.Location.C, new List<Pleb>(10));
        locationReference.Add(Position.Location.D, new List<Pleb>(10));
    }

    public void UpdateUI()
    {
        SoulCounter.text = $"Soul: {soul}";
        PlebListText.text =
            String.Join(
                "\n",
                plebs
                    .OrderBy(p => p.Position.CurrentLocation)
                    .ThenBy(p => p.Position.MarchingOrder)
                    .Select(p => $"{p.Position.CurrentLocation} - {p.Position.MarchingOrder} - {p.Name}"));
    }

    public void SpawnPleb(string name)
    {
        // Take Soul
        soul--;

        // Create Pleb
        var newPleb = new Pleb
        {
            Name = name,
            Position = new Position
            {
                CurrentLocation = Position.Location.A,
                MarchingOrder = locationReference[Position.Location.A].Count
            }
        };
        locationReference[Position.Location.A].Add(newPleb);
        plebs.Add(newPleb);
        UpdateUI();
    }
}
