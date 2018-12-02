namespace Assets.Scripts.Phases
{
    public class Phase
    {
        protected GamePhaseManager manager;

        public Phase(GamePhaseManager manager)
        {
            this.manager = manager;
        }

        public virtual void Enter() { }
        public virtual void Action(string action) { }
        public virtual void Exit() { }
    }
}
