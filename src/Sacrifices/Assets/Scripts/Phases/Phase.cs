using Assets.Scripts.Models;

namespace Assets.Scripts.Phases
{
    public class Phase
    {
        protected GamePhaseManager manager;

        public Phase(GamePhaseManager manager)
        {
            this.manager = manager;
        }

        public virtual Position.Location? Enter() {
            return Position.Location.A;
        }
        public virtual void Action(string action) { }
        public virtual void Exit() { }
    }
}
