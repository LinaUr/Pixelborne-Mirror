// This interface must be implemented by enemies in order to have an AttackPatternExecutor 
public interface IEnemyAttackAndMovement : IAttack
{
    void AttackMiddle();
    void AttackUp();
    void AttackDown();

    void StartFollowPlayer();
    void StopFollowPlayer();

    void Jump();

    void StartAutoJumping();
    void StopAutoJumping();

    float GetAttackUpDuration();
    float GetAttackMiddleDuration();
    float GetAttackDownDuration();

    bool IsPlayerInAttackRange();
    bool IsPlayerInSightRange();
    bool IsEnemyOnGround();
}
