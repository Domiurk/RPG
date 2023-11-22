namespace DevionGames
{
    public class Slide : ChangeHeight
    {
        public override bool CanStart()
        {
            return base.CanStart() && Controller.RelativeInput.z*Controller.SpeedMultiplier > 1f;
        }

    }
}