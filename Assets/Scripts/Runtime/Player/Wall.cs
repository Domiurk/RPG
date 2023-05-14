using UnityEngine;

namespace Runtime.Player
{
    public class Wall : MonoBehaviour, ITakeDamage
    {
        [SerializeField] private Health _health = new Health(100);

        public void TakeDamage(IDamage damage)
            => _health.TakeDamage(damage);
    }

    [System.Serializable]
    public class Health : IAttribute, ITakeDamage
    {
        public string Name => "Health";
        public float MaxValue { get; }
        public float Value => _value;
        [SerializeField] private float _value;

        public Health(float maxValue)
        {
            MaxValue = maxValue;
            _value = maxValue;
        }

        public void TakeDamage(IDamage damage)
        {
            if(Value - damage.Damage > 0)
                _value -= damage.Damage;
            else
                _value = 0;
        }
    }
}