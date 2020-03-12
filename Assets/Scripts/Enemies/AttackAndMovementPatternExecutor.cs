using System;
using System.Collections.Generic;
using UnityEngine;

//This class can automatically execute attack and movement actions on objects that 
//have a proper implementation of the IEnemyAttackAndMovement interface.
//The Entity that is executed by this class should have an attack and sight range.
//The actions are divided into 3 pattern.
//The first pattern is the m_attackPatternStringWhileOutOfSight. It is executed if not IsPlayerInSightRange().
//The second pattern is the m_attackPatternStringWhileInSightRange. It is executed if IsPlayerInoAttackRange() and not IsPlayerInSAttackRange.
//The last pattern is the m_attackPatternStringWhileInAttackRange. It is executed if IsPlayerInAttackRange().

//Each pattern is provided as a string with the grammar below. It basically contains a series of actions that are looped infinitely.
//The identifications of these actions can be found below. After each action a waiting time can be specified. 
//If no waiting time is specified, the duration of that action is taken as the waiting time.

//When the attack pattern changes the currently executed action is finished and 
//then the new attack pattern starts from the beginning.
public class AttackAndMovementPatternExecutor : MonoBehaviour
{
    // ATTACK PATTERN GRAMMAR:
    // ATTACK_PATTERN = ATTACK_TOKEN ATTACK_PATTERN_1 or epsilon
    // ATTACK_PATTERN_1 = |ATTACK_TOKEN or epsilon
    // ATTACK_TOKEN = ATTACK_INSTRUCTION or ATTACK_INSTRUCTION|TIMEOUT
    // TIMEOUT = float
    // ATTACK_INSTRUCTION = one of the constant strings below

    public const string ATTACK_UP_IDENTIFICATION = "AU";
    public const string ATTACK_MID_IDENTIFICATION = "AM";
    public const string ATTACK_DOWN_IDENTIFICATION = "AD";
    public const string START_FOLLOW_PLAYER_IDENTIFICATION = "STARTF";
    public const string STOP_FOLLOW_PLAYER_IDENTIFICATION = "STOPF";
    public const string START_AUTO_JUMPING_IDENTIFICATION = "STARTAUTOJUMP";
    public const string STOP_AUTO_JUMPING_IDENTIFICATION = "STOPAUTOJUMP";
    public readonly string SEPERATION_IDENTIFICATION = "|";

    [SerializeField] 
    private GameObject m_entity;
    [SerializeField] 
    private string m_attackPatternStringWhileInAttackRange;
    [SerializeField] 
    private string m_attackPatternStringWhileInSightRange;
    [SerializeField] 
    private string m_attackPatternStringWhileOutOfSight;

    private IEnemyAttackAndMovement m_entityAttackAndMovement;
    private List<Action> m_actions;
    
    private EntityMode m_currentEntityMode = EntityMode.OUT_OF_SIGHT_RANGE;
    private int m_currentAttackPatternListIndex;
    private int m_nextAttackPatternIndex;
    private float m_timeToWaitUntilNextAction;
    // int = actionIndex, float = waiting time
    private Tuple<int, float>[][] m_attackPatternList;
    
    private Dictionary<string, Tuple<int, float>> m_attackPatternStringToInternalIdentifications;

    private enum EntityMode { 
        IN_ATTACK_RANGE = 0, 
        IN_SIGHT_RANGE = 1, 
        OUT_OF_SIGHT_RANGE = 2,
    };

    void Awake()
    {
        m_entityAttackAndMovement = m_entity.GetComponent<IEnemyAttackAndMovement>();
    }

    void Start()
    {
        m_actions = new List<Action>()
        {
            m_entityAttackAndMovement.AttackUp, 
            m_entityAttackAndMovement.AttackMiddle,
            m_entityAttackAndMovement.AttackDown, 
            m_entityAttackAndMovement.StartFollowPlayer,
            m_entityAttackAndMovement.StopFollowPlayer ,
            m_entityAttackAndMovement.StartAutoJumping,
            m_entityAttackAndMovement.StartAutoJumping,
        };
        PrepareAttackPatternParsingDict();
        m_attackPatternList = new Tuple<int, float>[][]
        {
            ParseAttackPattern(m_attackPatternStringWhileInAttackRange),
            ParseAttackPattern(m_attackPatternStringWhileInSightRange),
            ParseAttackPattern(m_attackPatternStringWhileOutOfSight),
        };
        ResetAttackPattern();
    }

    void ResetAttackPattern()
    {
        m_nextAttackPatternIndex = 0;
        m_timeToWaitUntilNextAction = 0;
        m_currentAttackPatternListIndex = (int) EntityMode.OUT_OF_SIGHT_RANGE;
        m_currentEntityMode = EntityMode.OUT_OF_SIGHT_RANGE;
    }

    void Update()
    {
        // Determine the new attack pattern.
        bool isPlayerInAttackRange = m_entityAttackAndMovement.IsPlayerInAttackRange();
        bool isPlayerInSightRange = m_entityAttackAndMovement.IsPlayerInSightRange();
        EntityMode oldEntityMode = m_currentEntityMode;

        if (isPlayerInAttackRange)
        {
            m_currentEntityMode = EntityMode.IN_ATTACK_RANGE;
        }
        else if (isPlayerInSightRange)
        {
            m_currentEntityMode = EntityMode.IN_SIGHT_RANGE;
        }
        else
        {
            m_currentEntityMode = EntityMode.OUT_OF_SIGHT_RANGE;
        }

        // Change to the new attack pattern if it changed.
        // The new attack pattern will start when the next action would be executed.
        if (oldEntityMode != m_currentEntityMode)
        {
            m_currentAttackPatternListIndex = (int)m_currentEntityMode;
            m_nextAttackPatternIndex = 0;
        }

        // Execute the next action if the last action finished.
        m_timeToWaitUntilNextAction -= Time.deltaTime;
        if (m_timeToWaitUntilNextAction < 0)
        {
            int nextActionIndex = m_attackPatternList[m_currentAttackPatternListIndex][m_nextAttackPatternIndex].Item1;
            m_actions[nextActionIndex]();
            m_timeToWaitUntilNextAction = m_attackPatternList[m_currentAttackPatternListIndex][m_nextAttackPatternIndex].Item2;

            // Go to the next action and start at the beginning if no action is left in order to loop the behavior.
            m_nextAttackPatternIndex = m_nextAttackPatternIndex < m_attackPatternList[m_currentAttackPatternListIndex].Length - 1 ? m_nextAttackPatternIndex + 1 : 0;
        }
    }

    private Tuple<int, float>[] ParseAttackPattern(string attackPatternString)
    {
        List<Tuple<int, float>> newAttackPattern = new List<Tuple<int, float>>();
        string[] actions = attackPatternString.Split(SEPERATION_IDENTIFICATION.ToCharArray());

        for (int i = 0; i < actions.Length; i++)
        {
            float currentAnimationDuration = 0.01f;
            float currentWaitingTime = 0;
            int currentActionIndex = -1;
            string currentAction = actions[i];
            string nextAction = i < actions.Length - 1 ? actions[i + 1] : null;
            currentActionIndex = m_attackPatternStringToInternalIdentifications[actions[i]].Item1;
            currentAnimationDuration = m_attackPatternStringToInternalIdentifications[actions[i]].Item2;

            // Test if the next action is a wait instruction.
            if (nextAction != null && float.TryParse(nextAction, out currentWaitingTime))
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
        };
    }
}
