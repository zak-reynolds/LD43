using Assets.Scripts.Models;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Phases
{
    public class Advance : Phase
    {
        private GameObject advancePanel;
        private GameObject advancePanelRoom;
        private GameObject advancePanelPick;
        private GameObject advancePanelPleb;
        public Advance(GamePhaseManager manager) : base(manager)
        {
            advancePanel = GameObject.FindGameObjectWithTag("AdvancePanel");
            advancePanelRoom = GameObject.FindGameObjectWithTag("AdvancePanel-Room");
            advancePanelPick = GameObject.FindGameObjectWithTag("AdvancePanel-Pick");
            advancePanelPleb = GameObject.FindGameObjectWithTag("AdvancePanel-Pleb");
            advancePanel.SetActive(false);
            advancePanelRoom.SetActive(false);
            advancePanelPick.SetActive(false);
            advancePanelPleb.SetActive(false);
        }

        public override void Enter()
        {
            Debug.Log("Entering Advance phase");
            advancePanel.SetActive(true);
            advancePanelRoom.SetActive(true);
            advancePanelPick.SetActive(false);
            advancePanelPleb.SetActive(false);
            if (!manager.ReadOnlyLocationReference.Values.Any(loc => loc.Count > 1))
            {
                manager.LogBattleEvent("The plebs stand, biding their time.");
                manager.NextPhase();
                return;
            }
        }

        public override void Action(string action)
        {
            var actions = action.Split('`');
            var actionType = actions[0];
            var actionTarget = actions[1];
            switch (actionType.ToLower())
            {
                case "move":
                    var target = manager.Plebs.Where(p => p.Name == actionTarget).First();
                    manager.LogBattleEvent($"{target.Name} hops to the front of the line.");
                    manager.MovePleb(target);
                    break;
                case "sacrifice":
                    var targetLocation = (Position.Location)Enum.Parse(typeof(Position.Location), actionTarget);
                    var sacrifice = manager.GetActivePleb(targetLocation);
                    manager.LogBattleEvent($"{sacrifice.Name} valiantly holds the door!");
                    for (int i = 0; i < sacrifice.Strength; ++i)
                    {
                        manager.AdvancePleb(targetLocation);
                    }
                    manager.SacrificePleb(targetLocation);
                    manager.LogBattleEvent($"{sacrifice.Name} can no longer hold on and is brutally crushed under the door's weight.");
                    break;
            }
            manager.NextPhase();
        }

        public override void Exit()
        {
            advancePanel.SetActive(false);
        }
    }
}
