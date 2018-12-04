﻿using Assets.Scripts.Models;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Phases
{
    public class Advance : Phase
    {
        private GameObject advancePanel;
        private GameObject advancePanelPick;
        private GameObject advancePanelPleb;
        public Advance(GamePhaseManager manager) : base(manager)
        {
            advancePanel = GameObject.FindGameObjectWithTag("AdvancePanel");
            advancePanelPick = GameObject.FindGameObjectWithTag("AdvancePanel-Pick");
            advancePanelPleb = GameObject.FindGameObjectWithTag("AdvancePanel-Pleb");
            advancePanel.SetActive(false);
            advancePanelPick.SetActive(true);
            advancePanelPleb.SetActive(false);
        }

        public override Position.Location? Enter()
        {
            Debug.Log("Entering Advance phase");
            advancePanel.SetActive(true);
            advancePanelPick.SetActive(true);
            advancePanelPleb.SetActive(false);
            if (!manager.ReadOnlyLocationReference.Values.Any(loc => loc.Count > 1))
            {
                manager.LogBattleEvent("The plebs stand, biding their time.");
                manager.NextPhase();
            }
            return manager.SelectedLocation;
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

                    manager.LineControllers[(int)target.Position.CurrentLocation].MovePleb(target);
                    break;
                case "sacrifice":
                    var targetLocation = (Position.Location)Enum.Parse(typeof(Position.Location), actionTarget);
                    var sacrifice = manager.GetActivePleb(targetLocation);
                    manager.LogBattleEvent($"{sacrifice.Name} valiantly holds the door!");
                    int numAdvanced = 0;
                    for (int i = 0; i < sacrifice.Strength; ++i)
                    {
                        numAdvanced += manager.AdvancePleb(targetLocation) ? 1 : 0;
                    }
                    manager.LineControllers[(int)sacrifice.Position.CurrentLocation].HoldTheDoor(sacrifice, numAdvanced);
                    manager.SacrificePleb(targetLocation);
                    manager.LogBattleEvent($"{sacrifice.Name} can no longer hold on and is brutally crushed under the door's weight.");
                    manager.AudioSource.PlayOneShot(manager.SlabSlam);
                    break;
                case "consume":
                    var touchTargetLocation = (Position.Location)Enum.Parse(typeof(Position.Location), actionTarget);
                    var touchSacrifice = manager.GetActivePleb(touchTargetLocation);
                    manager.LogBattleEvent($"{touchSacrifice.Name} feels compelled to walk to the idol and place a hand on it.");
                    manager.ConsumePleb(touchSacrifice);

                    manager.LineControllers[(int)touchSacrifice.Position.CurrentLocation].ConsumePleb();
                    manager.AudioSource.PlayOneShot(manager.Consume);
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
