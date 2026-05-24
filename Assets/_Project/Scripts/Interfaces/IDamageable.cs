public interface IDamageable {
    float CurrentHealth { get; }

    void ReceiveDamage(float damageAmount);
}