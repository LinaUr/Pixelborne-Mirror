using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStage4 : MonoBehaviour
{
    [SerializeField]
    private int m_animationDuration = 500;
    [SerializeField]
    private int m_flashDuration = 100;
    [SerializeField]
    private int m_textPartDisplayTime = 3000;

    enum DialogueMode
    {
        NotStarted,
        Displaying,
        WaitingForTrigger,
        Flashing,
        Animation
    }

    private string m_activeCharacter;
    private int m_animationPart;
    private string[] m_animationPictures;
    private string[] m_animationPictures0 = { "OutroImages/spilled_stones_blood_3",
                                             "OutroImages/awakening" };
    private string[] m_animationPictures1 = { "OutroImages/dark_crown",
                                             "OutroImages/hit_animation",
                                             "OutroImages/dark_crown_destroyed" };
    private GameObject m_background;
    private GameObject m_backgroundPicture;
    private GameObject m_demonKing;
    private GameObject m_dialogue;
    private DialogueMode m_dialogueMode;
    private int m_dialoguePart;
    private string[] m_dialogueText;
    private string[] m_dialogueTextPart0 = { "Father!" };
    private string[] m_dialogueTextPart1 = { "My child! You are here!",
                                             "Knight! My thanks as both father and king.",
                                             "My daughter and my kingdom are safe once more.",
                                             "...",
                                             "Is that the Dark King's Crown you're carrying with you?" };
    private string[] m_dialogueTextPart2 = { "Yes, Father. We found it in his lair." };
    private string[] m_dialogueTextPart3 = { "Give it to me, Knight!",
                                             "And give me the shards of Dark Crystal you recovered." };
    private string[] m_dialogueTextPart4 = { "Marvelous!",
                                             "Let it be known that from this day, our kingdom shall bow to no demon!" };
    private string[] m_dialogueTextPart5 = { "Let it be known that today will mark the beginning of a new era!" };
    private string[] m_dialogueTextPart6 = { "Ah..!" };
    private string[] m_dialogueTextPart7 = { "Father! Are you well?" };
    private string[] m_dialogueTextPart8 = { "I... huh... just... dizzy... huh... huh..." };
    private string[] m_dialogueTextPart9 = { "Aaaaargh!" };
    private string[] m_dialogueTextPart10 = { "Father!" };
    private string[] m_dialogueTextPart11 = { "F-Father?" };
    private string[] m_dialogueTextPart12 = { "Haha... your father is gone, child.",
                                              "His body is mine now." };
    private string[] m_dialogueTextPart13 = { "No..!" };
    private string[] m_dialogueTextPart14 = { "Don't worry, child. I will have plenty of time for you later.",
                                              "But first..!" };
    private string[] m_dialogueTextPart15 = { "First, maggot, it is time for you to die!" };
    private string[] m_dialogueTextPart16 = { "O Father..." };
    private string[] m_dialogueTextPart17 = { "Please forgive me..." };
    private string[] m_dialogueTextPart18 = { "I love you, Father..." };
    private string[] m_dialogueTextPart19 = { "Hah... Hahaha..." };
    private string[] m_dialogueTextPart20 = { "What..?" };
    private string[] m_dialogueTextPart21 = { "Hahahaha!" };
    private string[] m_dialogueTextPart22 = { "Who..?",
                                              "...",
                                              "No..! The stones..!" };
    private string[] m_dialogueTextPart23 = { "At last..!" };
    private string[] m_dialogueTextPart24 = { "NO!" };
    private string[] m_dialogueTextPart25 = { "At last, I'm free once more!" };
    private string[] m_dialogueTextPart26 = { "The Dark King!" };
    private string[] m_dialogueTextPart27 = { "Indeed. The only king left standing, it seems." };
    private string[] m_dialogueTextPart28 = { "I curse you, Dark King!",
                                              "You will die for what you have done!" };
    private string[] m_dialogueTextPart29 = { "Really? Hahaha... You are mistaken, I fear.",
                                              "YOU will die.",
                                              "You will all die.",
                                              "Your whole kingdom will perish!",
                                              "And then, it shall be mine!" };
    private string[] m_dialogueTextPart30 = { "Not this day, Dark King. And not on any other.",
                                              "Let it be known that from this day, our kingdom shall bow to no demon.",
                                              "Knight!",
                                              "Destroy his crown, so that he shall stay in Hell, where he belongs, for all eternity!" };
    private int m_displayStartTime;
    private GameObject m_endboss;
    private bool m_enemiesKilled;
    private GameObject m_filterImage;
    private GameObject m_nameTag;
    public bool PlayerProgressed { get; set; }
    private GameObject m_princess;
    private int m_textPart;
    private string m_userName;

    void Start()
    {
        m_background = GameObject.Find("Background");
        m_dialogue = GameObject.Find("Speech");
        m_nameTag = GameObject.Find("NameTag");
        m_backgroundPicture = GameObject.Find("BackgroundPicture");
        m_filterImage = GameObject.Find("FilterImage");
        m_demonKing = GameObject.Find("Demon_King");
        m_endboss = GameObject.Find("Endboss");
        m_princess = GameObject.Find("Princess");
        m_demonKing.SetActive(false);
        Singleplayer.Instance.ActiveEnemies.Remove(m_demonKing);
        m_endboss.SetActive(false);
        Singleplayer.Instance.ActiveEnemies.Remove(m_endboss);
        m_dialoguePart = 0;
        GetName();
        m_dialogueText = m_dialogueTextPart0;
        m_activeCharacter = "Princess";
        PlayerProgressed = false;
        m_enemiesKilled = false;
        m_dialogueMode = DialogueMode.NotStarted;
        Singleplayer.Instance.ActiveEnemies.Remove(m_princess);
        m_princess.GetComponent<EnemyAttackAndMovement>().StartFollowPlayer();
    }

    void Update()
    {
        m_enemiesKilled = EnemiesKilled();
        switch (m_dialogueMode)
        {
            case DialogueMode.Displaying:
                if(Input.GetKeyDown("space"))
                {
                    m_displayStartTime -= m_textPartDisplayTime;
                }
                m_dialogue.GetComponent<TextMeshProUGUI>().text = m_dialogueText[m_textPart];
                if (Toolkit.CurrentTimeMillisecondsToday() - m_displayStartTime >= m_textPartDisplayTime)
                {
                    m_textPart++;
                    if (m_textPart == m_dialogueText.Length)
                    {
                        ChangePart();
                    }
                    m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
                }
                break;

            case DialogueMode.NotStarted:
                if (PlayerProgressed && m_enemiesKilled)
                {
                    ShowText();
                }
                break;

            case DialogueMode.WaitingForTrigger:
                if (PlayerProgressed && m_enemiesKilled)
                {
                    ChangePart();
                }
                break;

            case DialogueMode.Flashing:
                if (Toolkit.CurrentTimeMillisecondsToday() - m_displayStartTime >= m_flashDuration)
                    {
                        ChangePart();
                    }
                break;

            case DialogueMode.Animation:    
                if (Toolkit.CurrentTimeMillisecondsToday() - m_displayStartTime >= m_animationDuration || m_animationPart < 0)
                {
                    m_animationPart++;
                    if (m_animationPart == m_animationPictures.Length)
                    {
                        ChangePart();
                        return;
                    }
                    m_backgroundPicture.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(m_animationPictures[m_animationPart]);
                    m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
                }
                break;
        }
    }

    public bool EnemiesKilled()
    {
        bool allKilled = true;
        foreach (GameObject enemy in Singleplayer.Instance.ActiveEnemies)
        {
                allKilled = false;
        }
        return allKilled;
    }

    public void ShowText()
    {
        Singleplayer.Instance.LockPlayerInput(true);
        Singleplayer.Instance.Player.GetComponent<PlayerMovement>().ResetEntityAnimations();
        m_dialogueMode = DialogueMode.Displaying;
        m_textPart = 0;
        m_background.GetComponent<Image>().color = Color.black;
        m_nameTag.GetComponent<TextMeshProUGUI>().text = m_activeCharacter;
        m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    public void FlashViolet()
    {
        m_background.GetComponent<Image>().color = Color.clear;
        m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
        m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
        m_filterImage.GetComponent<SpriteRenderer>().enabled = true;
        m_dialogueMode = DialogueMode.Flashing;
        m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    public void Animate()
    {
        m_background.GetComponent<Image>().color = Color.clear;
        m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
        m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
        m_dialogueMode = DialogueMode.Animation;
        m_animationPart = -1;
        m_backgroundPicture.GetComponent<Image>().color = Color.white;
        m_displayStartTime = Toolkit.CurrentTimeMillisecondsToday();
    }

    public void ChangePart()
    {
        m_dialoguePart++;
        switch (m_dialoguePart)
        {
            case 1:
                m_dialogueText = m_dialogueTextPart1;
                m_activeCharacter = "King";
                ShowText();
                break;

            case 2:
                m_dialogueText = m_dialogueTextPart2;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 3:
                m_dialogueText = m_dialogueTextPart3;
                m_activeCharacter = "King";
                ShowText();
                break;

            case 4:
                m_background.GetComponent<Image>().color = Color.clear;
                m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
                m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
                PlayerProgressed = false;
                m_dialogueMode = DialogueMode.WaitingForTrigger;            //player supposed to walk to king
                Singleplayer.Instance.LockPlayerInput(false);
                break;

            case 5:
                m_dialogueText = m_dialogueTextPart4;
                ShowText();
                break;

            case 6:
                m_backgroundPicture.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("OutroImages/taking_the_crown");
                m_backgroundPicture.GetComponent<Image>().color = Color.white;
                m_dialogueText = m_dialogueTextPart5;
                ShowText();
                m_background.GetComponent<Image>().color = Color.clear;
                break;

            case 7:
                m_backgroundPicture.GetComponent<Image>().color = Color.clear;
                FlashViolet();
                break;

            case 8:
                m_filterImage.GetComponent<SpriteRenderer>().enabled = false;
                m_dialogueText = m_dialogueTextPart6;
                ShowText();
                break;

            case 9:
                m_dialogueText = m_dialogueTextPart7;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 10:
                m_dialogueText = m_dialogueTextPart8;
                m_activeCharacter = "King";
                ShowText();
                break;

            case 11:
                m_filterImage.GetComponent<SpriteRenderer>().enabled = true;
                m_dialogueText = m_dialogueTextPart9;
                ShowText();
                break;

            case 12:
                m_dialogueText = m_dialogueTextPart10;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 13:
                m_filterImage.GetComponent<SpriteRenderer>().enabled = false;
                GameObject.Find("King").SetActive(false);
                m_demonKing.SetActive(true);
                Singleplayer.Instance.ActiveEnemies.Add(m_demonKing);

                m_dialogueText = m_dialogueTextPart11;
                ShowText();
                break;

            case 14:
                m_dialogueText = m_dialogueTextPart12;
                m_activeCharacter = "King";
                ShowText();
                break;

            case 15:
                m_dialogueText = m_dialogueTextPart13;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 16:
                m_dialogueText = m_dialogueTextPart14;
                m_activeCharacter = "King";
                ShowText();
                break;

            case 17:
                m_dialogueText = m_dialogueTextPart15;
                ShowText();
                break;

            case 18:  //bossfight #1 begins here
                m_background.GetComponent<Image>().color = Color.clear;
                m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
                m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
                m_dialogueMode = DialogueMode.WaitingForTrigger;            //player supposed to kill king
                m_princess.GetComponent<EnemyAttackAndMovement>().StopFollowPlayer();
                Singleplayer.Instance.LockPlayerInput(false);
                break;

            case 19:
                m_backgroundPicture.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("OutroImages/spilled_stones");
                m_backgroundPicture.GetComponent<Image>().color = Color.white;
                m_dialogueText = m_dialogueTextPart16;
                m_activeCharacter = "Princess";
                ShowText();
                m_background.GetComponent<Image>().color = Color.clear;
                break;

            case 20:
                m_backgroundPicture.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("OutroImages/spilled_stones_blood_1");
                m_dialogueText = m_dialogueTextPart17;
                ShowText();
                m_background.GetComponent<Image>().color = Color.clear;
                break;

            case 21:
                m_backgroundPicture.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("OutroImages/spilled_stones_blood_2");
                m_dialogueText = m_dialogueTextPart18;
                ShowText();
                m_background.GetComponent<Image>().color = Color.clear;
                break;

            case 22:
                m_animationPictures = m_animationPictures0;
                Animate();
                break;

            case 23:
                m_dialogueText = m_dialogueTextPart19;
                m_activeCharacter = "???";
                ShowText();
                m_background.GetComponent<Image>().color = Color.clear;
                break;

            case 24:
                m_backgroundPicture.GetComponent<Image>().color = Color.clear;
                m_dialogueText = m_dialogueTextPart20;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 25:
                m_dialogueText = m_dialogueTextPart21;
                m_activeCharacter = "???";
                ShowText();
                break;

            case 26:
                m_dialogueText = m_dialogueTextPart22;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 27:
                m_dialogueText = m_dialogueTextPart23;
                m_activeCharacter = "???";
                ShowText();
                break;

            case 28:
                m_dialogueText = m_dialogueTextPart24;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 29:
                FlashViolet();
                break;

            case 30:
                Singleplayer.Instance.ActiveEnemies.Remove(m_demonKing);
                m_demonKing.SetActive(false);
                m_endboss.SetActive(true);
                Singleplayer.Instance.ActiveEnemies.Add(m_endboss);

                m_filterImage.GetComponent<SpriteRenderer>().enabled = false;
                m_dialogueText = m_dialogueTextPart25;
                m_activeCharacter = "Dark King";
                ShowText();
                break;

            case 31:
                m_dialogueText = m_dialogueTextPart26;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 32:
                m_dialogueText = m_dialogueTextPart27;
                m_activeCharacter = "Dark King";
                ShowText();
                break;

            case 33:
                m_dialogueText = m_dialogueTextPart28;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 34:
                m_dialogueText = m_dialogueTextPart29;
                m_activeCharacter = "Dark King";
                ShowText();
                break;

            case 35: //bossfight #2 begins here
                m_background.GetComponent<Image>().color = Color.clear;
                m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
                m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
                m_dialogueMode = DialogueMode.WaitingForTrigger;            //player supposed to kill dark king
                Singleplayer.Instance.LockPlayerInput(false);
                break;

            case 36:
                m_dialogueText = m_dialogueTextPart30;
                m_activeCharacter = "Princess";
                ShowText();
                break;

            case 37:
                m_background.GetComponent<Image>().color = Color.clear;
                m_dialogue.GetComponent<TextMeshProUGUI>().text = "";
                m_nameTag.GetComponent<TextMeshProUGUI>().text = "";
                PlayerProgressed = false;
                m_dialogueMode = DialogueMode.WaitingForTrigger;            //player supposed to walk to crown
                int playerLayer = LayerMask.NameToLayer("Player");
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
                Singleplayer.Instance.LockPlayerInput(false);
                break;

            case 38:
                Singleplayer.Instance.LockPlayerInput(true);
                m_animationPictures = m_animationPictures1;
                m_backgroundPicture.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(m_animationPictures[0]);
                Animate();
                break;

            case 39:
                Singleplayer.Instance.ReachedEndOfStage();
                break;
        }
    }

    public void GetName()
    {
        m_userName = Environment.UserName;
        m_dialogueTextPart1[1] = "Knight " + m_userName + "! My thanks as both father and king.";
        m_dialogueTextPart3[0] = "Give it to me, Knight " + m_userName + "!";
        m_dialogueTextPart15[0] = "First, maggot " + m_userName + ", it is time for you to die!";
        m_dialogueTextPart30[2] = "Knight " + m_userName + "!";
    }
}

