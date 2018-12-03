﻿using Assets.Scripts.Models;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Phases
{
    public class Fight : Phase
    {
        private Pleb combatantA;
        private Pleb combatantB;
        private GameObject fightPanel;

        public Fight(GamePhaseManager manager) : base(manager)
        {
            fightPanel = GameObject.FindGameObjectWithTag("FightPanel");
            fightPanel.SetActive(false);
        }

        public override Position.Location? Enter()
        {
            Debug.Log("Entering Fight phase");
            fightPanel.SetActive(true);
            Pleb[] combatants = manager.ReadOnlyLocationReference
                .Values
                .Where(loc => loc.Count > 2)
                ?.OrderBy(loc => Random.Range(0, 100))
                .FirstOrDefault()
                ?.OrderBy(i => Random.Range(0, 100))
                .Take(2)
                .ToArray();
            if (combatants == null)
            {
                manager.LogBattleEvent("No one is looking at each other.");
                manager.NextPhase();
                return null;
            }
            combatantA = combatants[0];
            combatantB = combatants[1];
            manager.LogBattleEvent($"{combatantA.Name} looks at {combatantB.Name}.");
            return combatantA.Position.CurrentLocation;
        }

        public override void Action(string action)
        {
            switch (action.ToLower())
            {
                case "kill":
                    manager.LogBattleEvent("They fight!");
                    bool aKilled = Random.Range(0, 1) == 0;
                    Pleb winner = aKilled ? combatantB : combatantA;
                    Pleb loser = aKilled ? combatantA : combatantB;
                    manager.KillPleb(loser);
                    manager.BoostPleb(winner);
                    manager.LogBattleEvent($"{winner.Name} tears out {loser.Name}'s spine and eats it for its power.");

                    manager.LineControllers[(int)winner.Position.CurrentLocation].FightPlebs(winner.Position.MarchingOrder, loser.Position.MarchingOrder, winner.Position.MarchingOrder);

                    manager.AudioSource.PlayOneShot(manager.Kill);
                    break;
                case "kiss":
                    manager.LogBattleEvent("They embrace!");
                    manager.BoostPleb(combatantA);
                    manager.BoostPleb(combatantB);
                    manager.KissPleb(combatantA);
                    manager.KissPleb(combatantB);
                    manager.LogBattleEvent("Their love boosts their strength, but they seem less.. virginal.");

                    manager.LineControllers[(int)combatantA.Position.CurrentLocation].KissPlebs(combatantA.Position.MarchingOrder, combatantB.Position.MarchingOrder);

                    manager.AudioSource.PlayOneShot(manager.Kiss[Random.Range(0, manager.Kiss.Length)]);
                    break;
            }
            manager.NextPhase();
        }

        public override void Exit()
        {
            fightPanel.SetActive(false);
        }
    }
}
