using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic Interface that defines all necessary methods to determine
// if an entity got hit and what damage the hit deals.
public interface IAttack
{
    int GetAttackDirection();
    int GetAttackDamage();

    bool IsFacingRight();
}
