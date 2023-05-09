namespace Runtime
{
    public interface ITakeDamage
    {
        void TakeDamage(IDamage damage);
    }

    public interface IDamage
    {
        float Damage { get; }
    }

    public interface IAttribute : IName
    {
        float MaxValue { get; }
        float Value { get; }
    }

    public interface ILevel
    {
        IExperience Experience { get; }
    }

    public interface IExperience
    {
        float CurrentValue { get; }
        float MaxValue { get; }
    }

    public interface IName
    {
        string Name { get; }
    }
}