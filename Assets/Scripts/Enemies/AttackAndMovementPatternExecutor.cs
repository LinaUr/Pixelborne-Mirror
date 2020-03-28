﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/*
    This class can automatically execute attack and movement actions on objects that 
    have a proper implementation of the IEnemyAttackAndMovement interface.
    The Entity that is executed by this class should have an attack and sight range.
    The actions are divided into 3 pattern.
    The first pattern is the m_attackPatternStringWhileOutOfSight. It is executed if not IsPlayerInSightRange().
    The second pattern is the m_attackPatternStringWhileInSightRange. It is executed if IsPlayerInoAttackRange() and not IsPlayerInSAttackRange.
    The last pattern is the m_attackPatternStringWhileInAttackRange. It is executed if IsPlayerInAttackRange().
    The last pattern is actually a list of individual patterns. After one individual pattern has finished the next one
    is chosen randomly.

    Each pattern is provided as a string with the grammar below. It basically contains a series of actions that are looped infinitely.
    The identifications of these actions can be found below. After each action a waiting time can be specified. 
    If no waiting time is specified, the duration of that action is taken as the waiting time.

    When the attack pattern changes the currently executed action is finished and 
    then the new attack pattern starts from the beginning.
*/
public class AttackAndMovementPatternExecutor : MonoBehaviour
{
    // ATTACK PATTERN GRAMMAR:
    // ATTACK_PATTERN = ATTACK_TOKEN ATTACK_PATTERN_1 or epsilon
    // ATTACK_PATTERN_1 = |ATTACK_TOKEN or epsilon
    // ATTACK_TOKEN = ATTACK_INSTRUCTION or ATTACK_INSTRUCTION|TIMEOUT
    // TIMEOUT = float
    // ATTACK_INSTRUCTION = one of the constant strings below

    public readonly static string ATTACK_UP_IDENTIFICATION = "AU";
    public readonly static string ATTACK_MID_IDENTIFICATION = "AM";
    public readonly static string ATTACK_DOWN_IDENTIFICATION = "AD";
    public readonly static string JUMP_IDENTIFICATION = "JUMP";
    public readonly static string START_FOLLOW_PLAYER_IDENTIFICATION = "STARTF";
    public readonly static string STOP_FOLLOW_PLAYER_IDENTIFICATION = "STOPF";
    public readonly static string START_AUTO_JUMPING_IDENTIFICATION = "STARTAUTOJUMP";
    public readonly static string STOP_AUTO_JUMPING_IDENTIFICATION = "STOPAUTOJUMP";
    public readonly static string SEPERATION_IDENTIFICATION = "|";

    [SerializeField] 
    private string[] m_attackPatternStringsWhileInAttackRange;
    [SerializeField] 
    private string m_attackPatternStringWhileInSightRange;
    [SerializeField] 
    private string m_attackPatternStringWhileOutOfSight;
    [SerializeField] 
    private GameObject m_entity;

    private IEnemyAttackAndMovement m_entityAttackAndMovement;
    private List<Action> m_actions;
    private Random random = new Random();
    private int m_nextAttackPatternIndex;
    private float m_timeToWaitUntilNextAction;
    private bool m_waitingOnBeingGrounded;
    // int = actionIndex, float = waiting time
    private Tuple<int, float>[][] m_attackPatterns;
    private int m_currentAttackPatternListIndex;
    private EntityMode m_currentEntityMode = EntityMode.OUT_OF_SIGHT_RANGE;
    
    private Dictionary<string, Tuple<int, float>> m_attackPatternStringToInternalIdentifications;

    private enum EntityMode { 
        IN_SIGHT_RANGE = 0, 
        OUT_OF_SIGHT_RANGE = 1,
        IN_ATTACK_RANGE = 2, 
    };

    void Awake()
    {
        m_entityAttackAndMovement = m_entity.GetComponent<IEnemyAttackAndMovement>();
    }

    void Start()
    {
        m_actions = new List<Action>(){ 
            m_entityAttackAndMovement.AttackUp, 
            m_entityAttackAndMovement.AttackMiddle,
            m_entityAttackAndMovement.AttackDown, 
            m_entityAttackAndMovement.StartFollowPlayer,
            m_entityAttackAndMovement.StopFollowPlayer ,
            m_entityAttackAndMovement.StartAutoJumping,
            m_entityAttackAndMovement.StartAutoJumping,
            m_entityAttackAndMovement.Jump,
        };
        PrepareAttackPatternParsingDict();
        m_attackPatterns = createAttackPatterns();
        ResetAttackPattern();
    }

    Tuple<int, float>[][] createAttackPatterns()
    {
        List<Tuple<int, float>[]> attackPatternList = new List<Tuple<int, float>[]>
        {
            ParseAttackPattern(m_attackPatternStringWhileInSightRange),
            ParseAttackPattern(m_attackPatternStringWhileOutOfSight),
        };
        foreach (string attackPatternWhileInRange in m_attackPatternStringsWhileInAttackRange)
        {
            attackPatternList.Add(ParseAttackPattern(attackPatternWhileInRange));
        }
        return attackPatternList.ToArray();
    }

    void ResetAttackPattern()
    {
        m_nextAttackPatternIndex = 0;
        m_timeToWaitUntilNextAction = 0;
        m_waitingOnBeingGrounded = false;
        m_currentAttackPatternListIndex = (int) EntityMode.OUT_OF_SIGHT_RANGE;
        m_currentEntityMode = EntityMode.OUT_OF_SIGHT_RANGE;
    }

    void Update()
    {
        // Determint the new attack pattern.
        bool isPlayerInAttackRange = m_entityAttackAndMovement.IsPlayerInAttackRange();
        bool isPlayerInSightRange = m_entityAttackAndMovement.IsPlayerInSightRange();
        EntityMode oldEntityMode = m_currentEntityMode;
        if (isPlayerInAttackRange)
        {
            m_currentEntityMode = EntityMode.IN_ATTACK_RANGE;
        }
        else if(isPlayerInSightRange)
        {
            m_currentEntityMode = EntityMode.IN_SIGHT_RANGE;
        }
        else
        {
            m_currentEntityMode = EntityMode.OUT_OF_SIGHT_RANGE;
        }
        // Change to the new attack pattern if it changed.
        // Choose a random attack pattern from the in-attack-range ones if the mode changed to IN_ATTACK_RANGE.
        // The new attack pattern will start when the next action would be executed.
        if(oldEntityMode != m_currentEntityMode)
        {
            if (m_currentEntityMode != EntityMode.IN_ATTACK_RANGE)
            {
                m_currentAttackPatternListIndex = (int) m_currentEntityMode;
            }
            else
            {
                RandomlySelectNextAttackPatternInRange();
            }
            m_nextAttackPatternIndex = 0;
        }

        // Execute the next action if the last action finished.
        m_timeToWaitUntilNextAction -= Time.deltaTime;
        if(m_waitingOnBeingGrounded)
        {
            //Debug.Log("setting enemy as grounded!");
            m_waitingOnBeingGrounded = !m_entityAttackAndMovement.IsEnemyOnGround();
        }
        if (m_timeToWaitUntilNextAction < 0 && !m_waitingOnBeingGrounded)
        {
            int nextActionIndex = m_attackPatterns[m_currentAttackPatternListIndex][m_nextAttackPatternIndex].Item1;
            m_actions[nextActionIndex]();
            m_timeToWaitUntilNextAction = m_attackPatterns[m_currentAttackPatternListIndex][m_nextAttackPatternIndex].Item2;
            if (m_timeToWaitUntilNextAction < 0)
            {
                m_waitingOnBeingGrounded = true;
                //Debug.Log("waiting on enemy as grounded!");
            }
            // Go to the next action and start at the beginning if no action is left in order to loop the behavior.
            if(m_nextAttackPatternIndex >= m_attackPatterns[m_currentAttackPatternListIndex].Length - 1)
            {
                m_nextAttackPatternIndex = -1;
                if(m_currentEntityMode == EntityMode.IN_ATTACK_RANGE)
                {
                    RandomlySelectNextAttackPatternInRange();
                }
            }
            m_nextAttackPatternIndex++;
        }
    }

    void RandomlySelectNextAttackPatternInRange()
    {
        int indexOfFirstAttackPatternInRange = m_attackPatterns.Length - m_attackPatternStringsWhileInAttackRange.Length;
        int indexOfNextAttackPatternInRange = random.Next(indexOfFirstAttackPatternInRange, m_attackPatterns.Length);
        m_currentAttackPatternListIndex = indexOfNextAttackPatternInRange;
    }

    private Tuple<int, float>[] ParseAttackPattern(string attackPatternString)
    {
        List<Tuple<int, float>> newAttackPattern = new List<Tuple<int, float>>();
        string[] actions = attackPatternString.Split(SEPERATION_IDENTIFICATION.ToCharArray());
        for (int i = 0; i < actions.Length; i++)
        {
            string nextAction = i < actions.Length - 1 ? actions[i + 1] : null;
            string currentAction = actions[i];
            Debug.Log(currentAction);
            Debug.Log(i);
            Debug.Log(m_attackPatternStringToInternalIdentifications);
            int currentActionIndex = m_attackPatternStringToInternalIdentifications[actions[i]].Item1;
            float currentAnimationDuration = m_attackPatternStringToInternalIdentifications[actions[i]].Item2;
            // Test if the next action is a wait instruction.
            float currentWaitingTime = 0;
            if(nextAction != null && float.TryParse(nextAction, out currentWaitingTime))
            {
                // Consume the next action.
                i++;
            }
            else
            {
                currentWaitingTime = currentAnimationDuration;
            }
            newAttackPattern.Add(new Tuple<int, float>(currentActionIndex, currentWaitingTime));
        }
        return newAttackPattern.ToArray();
    }

    void PrepareAttackPatternParsingDict()
    {
        m_attackPatternStringToInternalIdentifications = new Dictionary<string, Tuple<int, float>>
        {
            { ATTACK_UP_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.AttackUp), 
                m_entityAttackAndMovement.GetAttackUpDuration())
            },
            { ATTACK_MID_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.AttackMiddle), 
                m_entityAttackAndMovement.GetAttackMiddleDuration())
            },
            { ATTACK_DOWN_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.AttackDown), 
                m_entityAttackAndMovement.GetAttackDownDuration())
            },
            { START_FOLLOW_PLAYER_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.StartFollowPlayer), 
                0.01f)
            },
            { STOP_FOLLOW_PLAYER_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.StopFollowPlayer), 
                0.01f)
            },
            { START_AUTO_JUMPING_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.StartAutoJumping), 
                0.01f)
            },
            { STOP_AUTO_JUMPING_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.StopAutoJumping), 
                0.01f)
            },
            { JUMP_IDENTIFICATION, new Tuple<int, float>(
                m_actions.IndexOf(m_entityAttackAndMovement.Jump), 
                -1.0f)
            },
        };
    }
}
