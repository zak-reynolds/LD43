using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Phases
{
    public class Spawn : Phase
    {
        private GameObject spawnPanel;
        private Stack<string> uniqueNames;

        public Spawn(GamePhaseManager manager) : base(manager) {
            spawnPanel = GameObject.FindGameObjectWithTag("SpawnPanel");
            var names = manager.SeedNames
                .Select(name => manager.NameModifiers.Select(mod => name + mod).ToList())
                .Aggregate((acc, cur) => acc.Concat(cur).ToList())
                .Concat(manager.SeedNames)
                .OrderBy(name => Random.Range(0, 100))
                .ToArray();
            uniqueNames = new Stack<string>(names);
        }

        public override void Enter()
        {
            Debug.Log("Entering Spawn phase");
            spawnPanel.SetActive(true);
        }

        public override void Action(string action)
        {
            string name = uniqueNames.Pop();
            manager.SpawnPleb(name);
            manager.ClearBattleLog();
            manager.LogBattleEvent($"{name} arises from the dust.");

            manager.NextPhase();
        }

        public override void Exit()
        {
            spawnPanel.SetActive(false);
        }
    }
}
