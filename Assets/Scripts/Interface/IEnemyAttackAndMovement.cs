// This interface must be implemented by enemies in order to have an AttackPatternExecutor 
/// <summary></summary>
public interface IEnemyAttackAndMovement 
{
    /// <summary>Determines whether [is player in attack range].</summary>
    /// <returns>
    ///   <c>true</c> if [is player in attack range]; otherwise, <c>false</c>.</returns>
    bool IsPlayerInAttackRange();
    /// <summary>Determines whether [is player in sight range].</summary>
    /// <returns>
    ///   <c>true</c> if [is player in sight range]; otherwise, <c>false</c>.</returns>
    bool IsPlayerInSightRange();
    /// <summary>Determines whether [is enemy on ground].</summary>
    /// <returns>
    ///   <c>true</c> if [is enemy on ground]; otherwise, <c>false</c>.</returns>
    bool IsEnemyOnGround();

    /// <summary>Gets the duration of the attack down.</summary>
    /// <returns></returns>
    float GetAttackDownDuration();
    /// <summary>Gets the duration of the attack up.</summary>
    /// <returns></returns>
    float GetAttackUpDuration();
    /// <summary>Gets the duration of the attack middle.</summary>
    /// <returns></returns>
    float GetAttackMiddleDuration();

    /// <summary>Attacks down.</summary>
    void AttackDown();
    /// <summary>Attacks the middle.</summary>
    void AttackMiddle();
    /// <summary>Attacks up.</summary>
    void AttackUp();

    /// <summary>Starts the follow player.</summary>
    void StartFollowPlayer();
    /// <summary>Stops the follow player.</summary>
    void StopFollowPlayer();

    /// <summary>Jumps this instance.</summary>
    void Jump();

    /// <summary>Starts the automatic jumping.</summary>
    void StartAutoJumping();
    /// <summary>Stops the automatic jumping.</summary>
    void StopAutoJumping();
}
