// This interface must be implemented by enemies in order to have an AttackPatternExecutor 
public interface IEnemyAttackAndMovement : IAttack
{
    bool IsPlayerInAttackRange();
    bool IsPlayerInSightRange();

    float GetAttackDownDuration();
    float GetAttackUpDuration();
    float GetAttackMiddleDuration();

    void AttackDown();
    void AttackMiddle();
    void AttackUp();

    void StartFollowPlayer();
    void StopFollowPlayer();

    void StartAutoJumping();
    void StopAutoJumping();
}
