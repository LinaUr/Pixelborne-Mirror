// This interface must be implemented by enemies in order to have an AttackPatternExecutor 
public interface IEnemyAttackAndMovement 
{
    bool IsPlayerInAttackRange();
    bool IsPlayerInSightRange();
    bool IsEnemyOnGround();

    float GetAttackDownDuration();
    float GetAttackUpDuration();
    float GetAttackMiddleDuration();

    void AttackDown();
    void AttackMiddle();
    void AttackUp();

    void StartFollowPlayer();
    void StopFollowPlayer();

    void Jump();

    void StartAutoJumping();
    void StopAutoJumping();
}
