using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePhaseManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip[] RoomMusic;
    public AudioSource AudioSource;
    public AudioMixerGroup[] Mixers;
    public AudioMixer AudioMixer;

    public AudioClip StartTurn;
    public AudioClip[] Kiss;
    public AudioClip SlabSlam;
    public AudioClip Consume;
    public AudioClip Kill;

    [Header("References")]
    public Text SoulCounter;
    public Text PlebListText;
    public Text BattleLog;
    public Text RoomName;
    public GameObject[] CamButtons;
    public GameObject MoveButton;
    public GameObject DoorButton;
    public GameObject ConsumeButton;
    public GameObject ActionBar;
    public List<LineController> LineControllers;
    public CameraController CameraController;

    [Header("Game Balance")]
    public int StartingSoul = 20;

    [Header("Init")]
    public List<string> SeedNames = new List<string>(5);
    public List<string> NameModifiers = new List<string>(10);

    public enum Phase { Spawn, Fight, Advance }

    public int Soul { get; private set; }
    private List<Pleb> plebs = new List<Pleb>(50);
    public IReadOnlyList<Pleb> Plebs => plebs;
    private Dictionary<Position.Location, List<Pleb>> locationReference = new Dictionary<Position.Location, List<Pleb>>(4);
    public Dictionary<Position.Location, List<Pleb>> ReadOnlyLocationReference => locationReference;
    private Phase phase = Phase.Spawn;
    private List<string> battleEvents = new List<string>();
    public Position.Location SelectedLocation {
        get
        {
            return selectedLocation;
        }
        private set
        {
            selectedLocation = value;
            CameraController.Focus = value;
            RoomName.text =
                value == Position.Location.A ? "Blood God's Chamber" :
                value == Position.Location.B ? "Corridor of Hope" :
                value == Position.Location.C ? "Corridor of Despair" :
                value == Position.Location.D ? "Entry to the Altar" :
                "The Altar of the Blood God";
            CamButtons[0].GetComponent<Button>().interactable = true;
            CamButtons[1].GetComponent<Button>().interactable = true;
            if (value == Position.Location.A)
            {
                CamButtons[0].GetComponent<Button>().interactable = false;
            }
            if (value == Position.Location.D)
            {
                CamButtons[1].GetComponent<Button>().interactable = false;
            }
            int numPlebsInRoom = locationReference[value].Count();
            MoveButton.SetActive(numPlebsInRoom >= 2);
            DoorButton.SetActive(numPlebsInRoom >= 1);
            ConsumeButton.SetActive(numPlebsInRoom >= 1);
        }
    }
    private Position.Location selectedLocation;
    private Pleb selectedPleb;

    private Dictionary<Phase, Assets.Scripts.Phases.Phase> phases = new Dictionary<Phase, Assets.Scripts.Phases.Phase>(3);

    private string selectedName;


    void Start()
    {
        Soul = StartingSoul;
        locationReference.Add(Position.Location.A, new List<Pleb>(10));
        locationReference.Add(Position.Location.B, new List<Pleb>(10));
        locationReference.Add(Position.Location.C, new List<Pleb>(10));
        locationReference.Add(Position.Location.D, new List<Pleb>(10));
        locationReference.Add(Position.Location.Altar, new List<Pleb>(20));
        phases.Add(Phase.Spawn, new Assets.Scripts.Phases.Spawn(this));
        phases.Add(Phase.Fight, new Assets.Scripts.Phases.Fight(this));
        phases.Add(Phase.Advance, new Assets.Scripts.Phases.Advance(this));
        phases[phase].Enter();
        SelectedLocation = Position.Location.A;
        UpdateUI();

        for (int i = 0; i < 5; ++i)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = RoomMusic[i];
            audioSource.Play();
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = Mixers[i];
        }
    }

    public void UpdateUI()
    {
        SoulCounter.text = $"Soul: {Soul}";
        PlebListText.text =
            String.Join(
                "\n",
                plebs
                    .OrderBy(p => p.Position.CurrentLocation)
                    .ThenBy(p => p.Position.MarchingOrder)
                    .Select(p => $"{p.Name} - {p.Position.CurrentLocation}\nS{p.Strength} - V{p.Virginity}"));
        BattleLog.text = String.Join("\n", battleEvents);
    }

    public void DebugUpdateSelectedName(string name)
    {
        selectedName = name;
    }

    public void SpawnPleb(string name)
    {
        // Take Soul
        Soul--;

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

    public void NextPhase()
    {
        StartCoroutine(NextPhaseCo());
    }

    private IEnumerator NextPhaseCo()
    {
        phases[phase].Exit();
        foreach (var lc in LineControllers)
        {
            lc.UpdatePlebText(this);
        }
        switch (phase)
        {
            case Phase.Spawn:
                phase = Phase.Fight;
                break;
            case Phase.Fight:
                phase = Phase.Advance;
                break;
            case Phase.Advance:
                phase = Phase.Spawn;
                break;
        }
        yield return new WaitForSeconds(3.5f);
        var location = phases[phase].Enter();
        if (location != null)
        {
            CameraController.Focus = (Position.Location)location;
        }
    }

    public void LogBattleEvent(string @event)
    {
        battleEvents.Add(@event);
        UpdateUI();
    }

    public void ClearBattleLog()
    {
        battleEvents.Clear();
        AudioSource.PlayOneShot(StartTurn);
        string snapshot = plebs.Any() ? $"R{plebs.Max(p => (int)p.Position.CurrentLocation) + 1}" : "R1";
        AudioMixer.TransitionToSnapshots(new AudioMixerSnapshot[] { AudioMixer.FindSnapshot(snapshot) }, new[] { 1f }, 2f);
    }

    public void PhaseAction(string action)
    {
        phases[phase].Action(action);
    }

    public void PhaseActionComposite(string action)
    {
        phases[phase].Action($"{action}`{selectedName}");
    }

    public void PhaseActionWithRoom(string action)
    {
        phases[phase].Action($"{action}`{SelectedLocation}");
    }

    public void KillPleb(Pleb pleb)
    {
        locationReference[pleb.Position.CurrentLocation].Remove(pleb);
        plebs.Remove(pleb);

        UpdateUI();
    }

    public void BoostPleb(Pleb pleb)
    {
        pleb.Strength++;

        UpdateUI();
    }

    public void KissPleb(Pleb pleb)
    {
        pleb.Virginity--;

        UpdateUI();
    }

    public void MovePleb(Pleb pleb)
    {
        var locationPlebs = locationReference[pleb.Position.CurrentLocation];
        locationPlebs.Remove(pleb);
        locationPlebs.Insert(0, pleb);
        UpdateLocation(locationPlebs);

        UpdateUI();
    }

    public void SacrificePleb(Position.Location location)
    {
        var locationPlebs = locationReference[location];
        var sacrifice = locationPlebs.First();
        locationPlebs.RemoveAt(0);
        plebs.Remove(sacrifice);
        UpdateLocation(locationPlebs);

        UpdateUI();
    }

    public bool AdvancePleb(Position.Location location)
    {
        var locationPlebs = locationReference[location];
        if (locationPlebs.Count < 2)
        {
            Debug.LogError($"Cannot advance with fewer than two plebs at {location}");
            return false;
        }
        var nextLocation = (Position.Location)((int)location + 1);
        var nextLocationPlebs = locationReference[nextLocation];
        var advancer = locationReference[location][1];
        locationPlebs.RemoveAt(1);
        nextLocationPlebs.Add(advancer);
        advancer.Position.CurrentLocation = nextLocation;
        UpdateLocation(locationPlebs);
        UpdateLocation(nextLocationPlebs);
        LogBattleEvent($"{advancer.Name} sneaks through the door.");
        if (nextLocation == Position.Location.Altar)
        {
            AltarSacrificePleb(advancer);
        }

        UpdateUI();
        return true;
    }

    private void AltarSacrificePleb(Pleb pleb)
    {
        ClearBattleLog();
        LogBattleEvent($"{pleb.Name} steps up to the altar...");
        if (pleb.Virginity == 0)
        {
            ActionBar.SetActive(false);
            LogBattleEvent($"A crash of lightning strikes {pleb.Name}!");
            LogBattleEvent($"You feel energy coursing through your body as you are pulled to the mortal plane. This land will finally be yours for the taking!");
            LogBattleEvent($"Thanks for playing! You made it through a Jam entry to Ludum Dare 43: Sacrifices must be Made.");
            LogBattleEvent($"Created by Zak Reynolds and Clint Glenn");
        } else
        {
            LogBattleEvent($"A rush of fire rises from the floor and consumes {pleb.Name}!");
            LogBattleEvent($"The sour taste of a happy person is left in your mouth.");
        }
    }

    private void UpdateLocation(List<Pleb> locationPlebs)
    {
        for (int i = 0; i < locationPlebs.Count; ++i)
        {
            locationPlebs[i].Position.MarchingOrder = i;
        }
    }

    public Pleb GetActivePleb(Position.Location location)
    {
        return locationReference[location].First();
    }

    public void SelectActiveRoom(int location)
    {
        SelectedLocation = (Position.Location)location;
        Debug.Log($"Set SelectedLocation to {SelectedLocation}");
    }

    public void NextRoom()
    {
        if (SelectedLocation != Position.Location.D)
        {
            SelectedLocation = (Position.Location)((int)SelectedLocation + 1);
        }
    }

    public void BackRoom()
    {
        if (SelectedLocation != Position.Location.A)
        {
            SelectedLocation = (Position.Location)((int)SelectedLocation - 1);
        }
    }

    public void SelectPleb(string name)
    {
        selectedPleb = plebs.Find(p => p.Name == name);
    }

    public void UpdateSeedName0(string name) => SeedNames[0] = name;
    public void UpdateSeedName1(string name) => SeedNames[1] = name;
    public void UpdateSeedName2(string name) => SeedNames[2] = name;
    public void UpdateSeedName3(string name) => SeedNames[3] = name;
    public void UpdateSeedName4(string name) => SeedNames[4] = name;

    public void ConsumePleb(Pleb touchSacrifice)
    {
        LogBattleEvent($"{touchSacrifice.Name} disintegrates in a flash of light!");
        if (touchSacrifice.Virginity == 0)
        {
            Soul += 6 + touchSacrifice.Strength;
            if (touchSacrifice.Strength < 3)
            {
                LogBattleEvent("You feel your vitality increase as the smell of virgin blood fills the air.");
            } else
            {
                LogBattleEvent("You feel your vitality greatly increase! Strong virgin, this one.");
            }
        } else
        {
            if (touchSacrifice.Strength == 0)
            {
                LogBattleEvent("You are disgusted at this pathetic excuse for a sacrifice.");
            }
            else if (touchSacrifice.Strength < 2)
            {
                Soul += 2;
                LogBattleEvent("Although distasteful, the offering functions as a small snack.");
            }
            else
            {
                Soul += 4;
                LogBattleEvent("Strong, but not the kind of blood you are interested in. A virgin will be better");
            }
        }

        var locationPlebs = locationReference[touchSacrifice.Position.CurrentLocation];
        locationPlebs.RemoveAt(0);
        plebs.Remove(touchSacrifice);
        UpdateLocation(locationPlebs);

        UpdateUI();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
