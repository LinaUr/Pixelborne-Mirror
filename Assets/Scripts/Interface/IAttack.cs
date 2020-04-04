// Basic interface that defines all necessary methods to determine
// if an entity got hit and what damage the hit deals.
/// <summary></summary>
public interface IAttack
{
    /// <summary>Determines whether [is facing right].</summary>
    /// <returns>
    ///   <c>true</c> if [is facing right]; otherwise, <c>false</c>.</returns>
    bool IsFacingRight();
    /// <summary>Gets the attack damage.</summary>
    /// <returns></returns>
    int GetAttackDamage();
    /// <summary>Gets the attack direction.</summary>
    /// <returns></returns>
    int GetAttackDirection();
}
