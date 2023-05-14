namespace Runtime.Player
{
    public interface INameable
    {
        string Name { get; }
    }
    public interface IControllable
    {
        void Move();
        void Crouch();
        void Jump();
        void Sprint();
    }
}