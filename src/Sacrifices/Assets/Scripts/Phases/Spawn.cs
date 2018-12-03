using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Phases
{
    public class Spawn : Phase
    {
        private GameObject spawnPanel;
        private GameObject restartPanel;
        private Stack<string> uniqueNames;

        public Spawn(GamePhaseManager manager) : base(manager) {
            spawnPanel = GameObject.FindGameObjectWithTag("SpawnPanel");
            restartPanel = GameObject.FindGameObjectWithTag("RestartPanel");
            restartPanel.SetActive(false);
            var names = manager.SeedNames
                .Select(name => manager.NameModifiers.Select(mod => name + mod).ToList())
                .Aggregate((acc, cur) => acc.Concat(cur).ToList())
                .Concat(manager.SeedNames)
                .OrderBy(name => Random.Range(0, 100))
                .ToArray();
            uniqueNames = new Stack<string>(names);
        }

        public override Position.Location? Enter()
        {
            Debug.Log("Entering Spawn phase");
            if (manager.Soul > 0)
            {
                spawnPanel.SetActive(true);
            }
            else
            {
                restartPanel.SetActive(true);
                manager.ClearBattleLog();
                manager.LogBattleEvent("As you reach for the dust, your hand fades as your connection to the material plane disintigrates.");
                manager.LogBattleEvent("The nations of the mortal world rejoice as the blood god disappears from their world.");
                manager.LogBattleEvent("Congratulations! You lost!");
                manager.LogBattleEvent("Next time, try to better manage your souls. You will have to make sacrifices, but when a virgin touches an idol they will generate more power for you. Promiscuous individuals can as well, but be warned that they may not be so helpful.");
            }
            return Position.Location.A;
        }

        public override void Action(string action)
        {
            string name = uniqueNames.Pop();
            manager.SpawnPleb(name);
            manager.ClearBattleLog();
            manager.LogBattleEvent($"{name} arises from the dust.");

            manager.LineControllers[0].SpawnPleb(manager.Plebs.Last());

            manager.NextPhase();
        }

        public override void Exit()
        {
            spawnPanel.SetActive(false);
        }
    }
}
