using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private string m_attackPattern;
    [SerializeField] 
    private string m_initialMovementPattern;
    [SerializeField] 
    private string m_movementWhenInRangePattern;
    [SerializeField] 
    private GameObject m_entity;
    private IEnemyAttackAndMovement m_entityAttackAndMovement;
    private Action[] m_actions;
    private float[] m_waitingTimeBetweenActions;
    private int[] m_orderOfActions;
    private int m_nextOrderOfActionIndex;
    private float m_timeToWaitUntilNextAction;

    void Awake()
    {
        m_entityAttackAndMovement = m_entity.GetComponent<IEnemyAttackAndMovement>();
        m_actions = new Action[]{ m_entityAttackAndMovement.AttackUp, m_entityAttackAndMovement.AttackMiddle,
            m_entityAttackAndMovement.AttackDown, m_entityAttackAndMovement.StartFollowPlayer,
            m_entityAttackAndMovement.StopFollowPlayer };
        m_nextOrderOfActionIndex = 0;
        m_timeToWaitUntilNextAction = 0;
    }

    void Start()
    {
        parseAttackPattern();
    }

    void Update()
    {
        m_timeToWaitUntilNextAction -= Time.deltaTime;
        if (m_timeToWaitUntilNextAction < 0)
        {
            int nextActionIndex = m_orderOfActions[m_nextOrderOfActionIndex];
            m_actions[nextActionIndex]();
            m_timeToWaitUntilNextAction = m_waitingTimeBetweenActions[m_nextOrderOfActionIndex];
            // Go to the next action and start at the beginning if no action is left in order to loop the behavior
            m_nextOrderOfActionIndex = m_nextOrderOfActionIndex < m_orderOfActions.Length - 1 ? m_nextOrderOfActionIndex + 1 : 0;
        }
    }

    private void parseAttackPattern()
    {
        List<int> orderOfActions = new List<int>();
        List<float> waitingTimeBetweenActions = new List<float>();
        string[] actions = m_attackPattern.Split(SEPERATION_IDENTIFICATION.ToCharArray());
        for (int i = 0; i < actions.Length; i++)
        {
            string nextAction = i < actions.Length - 1 ? actions[i + 1] : null;
            int action = -1;
            float currentAnimationDuration = 0.01f;
            switch(actions[i])
            {
                case ATTACK_UP_IDENTIFICATION:
                    action = 0;
                    currentAnimationDuration = m_entityAttackAndMovement.GetAttackUpDuration();
                    break;
                case ATTACK_MID_IDENTIFICATION:
                    action = 1;
                    currentAnimationDuration = m_entityAttackAndMovement.GetAttackMiddleDuration();
                    break;
                case ATTACK_DOWN_IDENTIFICATION:
                    action = 2;
                    currentAnimationDuration = m_entityAttackAndMovement.GetAttackDownDuration();
                    break;
                case START_FOLLOW_PLAYER_IDENTIFICATION:
                    action = 3;
                    currentAnimationDuration = 0.01f;
                    break;
                case STOP_FOLLOW_PLAYER_IDENTIFICATION:
                    action = 4;
                    currentAnimationDuration = 0.01f;
                    break;
                default:
                    throw new System.ArgumentException($"The Action number {i} from the action pattern could not be parsed");
            }
            orderOfActions.Add(action);
            // Test if the next action is a wait instruction
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
            waitingTimeBetweenActions.Add(currentWaitingTime);
        }
        m_orderOfActions = orderOfActions.ToArray();
        m_waitingTimeBetweenActions = waitingTimeBetweenActions.ToArray();
    }
}
