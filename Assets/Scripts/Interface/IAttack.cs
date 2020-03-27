// Basic interface that defines all necessary methods to determine
// if an entity got hit and what damage the hit deals.
public interface IAttack
{
    bool IsFacingRight();
    int GetAttackDamage();
    int GetAttackDirection();
}
