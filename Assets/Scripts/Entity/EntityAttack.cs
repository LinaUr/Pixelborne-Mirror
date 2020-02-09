using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

// This class manages the basic attack functionality of an entity.
public class EntityAttack : MonoBehaviour
{
    [SerializeField]
    private int m_attackDamage = 1;
    [SerializeField] 
    private GameObject m_playerSword;
    private float m_attackDirection;
    private double m_attackDuration;
    private int m_currentAttackAnimationParameter = 0; 
    private double m_lastTimeAttacked = -10000;
    private SpriteRenderer m_swordRenderer;
    private Collider2D m_blackSwordCollider;
    private PlayerMovement m_playerMovement;

    public GameObject PlayerSword { 
        get 
        { 
            return m_playerSword; 
        } 
        set
        { 
            m_playerSword = value; 
        }
    }


}
