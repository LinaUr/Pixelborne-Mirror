// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Menu/ButtonNavigationInputMaster.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class ButtonNavigationInputMaster : InputActionAssetReference
{
    public ButtonNavigationInputMaster()
    {
    }
    public ButtonNavigationInputMaster(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // ButtonNavigation
        m_ButtonNavigation = asset.GetActionMap("ButtonNavigation");
        m_ButtonNavigation_Up = m_ButtonNavigation.GetAction("Up");
        m_ButtonNavigation_Down = m_ButtonNavigation.GetAction("Down");
        // TEST
        m_TEST = asset.GetActionMap("TEST");
        m_TEST_WHATEVER = m_TEST.GetAction("WHATEVER");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_ButtonNavigation = null;
        m_ButtonNavigation_Up = null;
        m_ButtonNavigation_Down = null;
        m_TEST = null;
        m_TEST_WHATEVER = null;
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
    // ButtonNavigation
    private InputActionMap m_ButtonNavigation;
    private InputAction m_ButtonNavigation_Up;
    private InputAction m_ButtonNavigation_Down;
    public struct ButtonNavigationActions
    {
        private ButtonNavigationInputMaster m_Wrapper;
        public ButtonNavigationActions(ButtonNavigationInputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Up { get { return m_Wrapper.m_ButtonNavigation_Up; } }
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
    // TEST
    private InputActionMap m_TEST;
    private InputAction m_TEST_WHATEVER;
    public struct TESTActions
    {
        private ButtonNavigationInputMaster m_Wrapper;
        public TESTActions(ButtonNavigationInputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @WHATEVER { get { return m_Wrapper.m_TEST_WHATEVER; } }
        public InputActionMap Get() { return m_Wrapper.m_TEST; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(TESTActions set) { return set.Get(); }
    }
    public TESTActions @TEST
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new TESTActions(this);
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
