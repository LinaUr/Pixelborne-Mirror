// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/InputMaster.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class InputMaster : InputActionAssetReference
{
    public InputMaster()
    {
    }
    public InputMaster(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_Jump = m_Player.GetAction("Jump");
        m_Player_Movement = m_Player.GetAction("Movement");
        m_Player_Attack = m_Player.GetAction("Attack");
        m_Player_AttackDirection = m_Player.GetAction("AttackDirection");
        m_Player_Record = m_Player.GetAction("Record");
        m_Player_Roll = m_Player.GetAction("Roll");
        m_Player_PauseGame = m_Player.GetAction("PauseGame");
        // ButtonNavigation
        m_ButtonNavigation = asset.GetActionMap("ButtonNavigation");
        m_ButtonNavigation_Left = m_ButtonNavigation.GetAction("Left");
        m_ButtonNavigation_Up = m_ButtonNavigation.GetAction("Up");
        m_ButtonNavigation_Right = m_ButtonNavigation.GetAction("Right");
        m_ButtonNavigation_Down = m_ButtonNavigation.GetAction("Down");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player = null;
        m_Player_Jump = null;
        m_Player_Movement = null;
        m_Player_Attack = null;
        m_Player_AttackDirection = null;
        m_Player_Record = null;
        m_Player_Roll = null;
        m_Player_PauseGame = null;
        m_ButtonNavigation = null;
        m_ButtonNavigation_Left = null;
        m_ButtonNavigation_Up = null;
        m_ButtonNavigation_Right = null;
        m_ButtonNavigation_Down = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Player
    private InputActionMap m_Player;
    private InputAction m_Player_Jump;
    private InputAction m_Player_Movement;
    private InputAction m_Player_Attack;
    private InputAction m_Player_AttackDirection;
    private InputAction m_Player_Record;
    private InputAction m_Player_Roll;
    private InputAction m_Player_PauseGame;
    public struct PlayerActions
    {
        private InputMaster m_Wrapper;
        public PlayerActions(InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump { get { return m_Wrapper.m_Player_Jump; } }
        public InputAction @Movement { get { return m_Wrapper.m_Player_Movement; } }
        public InputAction @Attack { get { return m_Wrapper.m_Player_Attack; } }
        public InputAction @AttackDirection { get { return m_Wrapper.m_Player_AttackDirection; } }
        public InputAction @Record { get { return m_Wrapper.m_Player_Record; } }
        public InputAction @Roll { get { return m_Wrapper.m_Player_Roll; } }
        public InputAction @PauseGame { get { return m_Wrapper.m_Player_PauseGame; } }
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
    }
    public PlayerActions @Player
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new PlayerActions(this);
        }
    }
    // ButtonNavigation
    private InputActionMap m_ButtonNavigation;
    private InputAction m_ButtonNavigation_Left;
    private InputAction m_ButtonNavigation_Up;
    private InputAction m_ButtonNavigation_Right;
    private InputAction m_ButtonNavigation_Down;
    public struct ButtonNavigationActions
    {
        private InputMaster m_Wrapper;
        public ButtonNavigationActions(InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left { get { return m_Wrapper.m_ButtonNavigation_Left; } }
        public InputAction @Up { get { return m_Wrapper.m_ButtonNavigation_Up; } }
        public InputAction @Right { get { return m_Wrapper.m_ButtonNavigation_Right; } }
        public InputAction @Down { get { return m_Wrapper.m_ButtonNavigation_Down; } }
        public InputActionMap Get() { return m_Wrapper.m_ButtonNavigation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(ButtonNavigationActions set) { return set.Get(); }
    }
    public ButtonNavigationActions @ButtonNavigation
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new ButtonNavigationActions(this);
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get

        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.GetControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
