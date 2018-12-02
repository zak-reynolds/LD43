namespace Assets.Scripts.Models
{
    public class Position
    {
        public enum Location { A, B, C, D, Altar }
        public Location CurrentLocation;
        public int MarchingOrder;
    }
}
