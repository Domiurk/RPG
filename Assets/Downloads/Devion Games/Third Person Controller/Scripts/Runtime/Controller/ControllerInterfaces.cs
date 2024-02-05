namespace DevionGames
{

    public interface IControllerEventHandler
    {
    }

    public interface IControllerGrounded : IControllerEventHandler
    {
        void OnControllerGrounded(bool grounded);
    }

    public interface IControllerAim : IControllerEventHandler
    {
        void OnControllerAim(bool aim);
    }
}