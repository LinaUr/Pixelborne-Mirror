// This interface must be implemented by enemies in order to have an AttackPatternExecutor 
public interface IEnemyAttackAndMovement : IAttack
{
    void AttackMiddle();
    void AttackUp();
    void AttackDown();

    void StartFollowPlayer();
    void StopFollowPlayer();

    float GetAttackUpDuration();
    float GetAttackMiddleDuration();
    float GetAttackDownDuration();

    bool IsPlayerInRange();
}
