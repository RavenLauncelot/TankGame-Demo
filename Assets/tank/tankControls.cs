//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/tank/tankControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @TankControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @TankControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""tankControls"",
    ""maps"": [
        {
            ""name"": ""Tank"",
            ""id"": ""23bef39c-de90-4a55-84a1-f538f5001d7a"",
            ""actions"": [
                {
                    ""name"": ""turret"",
                    ""type"": ""Value"",
                    ""id"": ""7709eb63-eff3-401d-8422-a6f89b4af8f9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""fire"",
                    ""type"": ""Button"",
                    ""id"": ""e62185dd-7665-48fb-b61b-289238f94604"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""movement"",
                    ""type"": ""Value"",
                    ""id"": ""df26f8da-4e9a-4b0e-8ffd-a0e94997ea05"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""zoomin"",
                    ""type"": ""Button"",
                    ""id"": ""7e1129f1-c993-46e9-bddf-d6ee97c9bc90"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a2dab8a9-b9f2-47fa-ae2d-b26201f36c4f"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false)"",
                    ""groups"": """",
                    ""action"": ""turret"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e0fdca1-b7a3-4089-8129-2166d78cc9a7"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77cbe723-96c3-46f6-ad07-c3111c815299"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4fd59b52-2565-41ee-89e9-b87e59e57b25"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""keyboard"",
                    ""id"": ""d17a0d2d-c57a-470c-b489-70a5688a9286"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9a0380ad-8ab2-447b-8544-76c9918ff8cc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""07eb3488-bc2a-4ff3-9872-345711e8cb29"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8a45eaee-0ac5-4481-bb45-e60cc1a9e939"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""465a8a14-84ff-48e7-8d73-8973a8ad331f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e12ad2b9-5d22-488f-aca7-492ff4242dda"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""zoomin"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57be393e-ef30-4775-ac1a-e3163c7d9add"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""zoomin"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Tank
        m_Tank = asset.FindActionMap("Tank", throwIfNotFound: true);
        m_Tank_turret = m_Tank.FindAction("turret", throwIfNotFound: true);
        m_Tank_fire = m_Tank.FindAction("fire", throwIfNotFound: true);
        m_Tank_movement = m_Tank.FindAction("movement", throwIfNotFound: true);
        m_Tank_zoomin = m_Tank.FindAction("zoomin", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Tank
    private readonly InputActionMap m_Tank;
    private List<ITankActions> m_TankActionsCallbackInterfaces = new List<ITankActions>();
    private readonly InputAction m_Tank_turret;
    private readonly InputAction m_Tank_fire;
    private readonly InputAction m_Tank_movement;
    private readonly InputAction m_Tank_zoomin;
    public struct TankActions
    {
        private @TankControls m_Wrapper;
        public TankActions(@TankControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @turret => m_Wrapper.m_Tank_turret;
        public InputAction @fire => m_Wrapper.m_Tank_fire;
        public InputAction @movement => m_Wrapper.m_Tank_movement;
        public InputAction @zoomin => m_Wrapper.m_Tank_zoomin;
        public InputActionMap Get() { return m_Wrapper.m_Tank; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TankActions set) { return set.Get(); }
        public void AddCallbacks(ITankActions instance)
        {
            if (instance == null || m_Wrapper.m_TankActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_TankActionsCallbackInterfaces.Add(instance);
            @turret.started += instance.OnTurret;
            @turret.performed += instance.OnTurret;
            @turret.canceled += instance.OnTurret;
            @fire.started += instance.OnFire;
            @fire.performed += instance.OnFire;
            @fire.canceled += instance.OnFire;
            @movement.started += instance.OnMovement;
            @movement.performed += instance.OnMovement;
            @movement.canceled += instance.OnMovement;
            @zoomin.started += instance.OnZoomin;
            @zoomin.performed += instance.OnZoomin;
            @zoomin.canceled += instance.OnZoomin;
        }

        private void UnregisterCallbacks(ITankActions instance)
        {
            @turret.started -= instance.OnTurret;
            @turret.performed -= instance.OnTurret;
            @turret.canceled -= instance.OnTurret;
            @fire.started -= instance.OnFire;
            @fire.performed -= instance.OnFire;
            @fire.canceled -= instance.OnFire;
            @movement.started -= instance.OnMovement;
            @movement.performed -= instance.OnMovement;
            @movement.canceled -= instance.OnMovement;
            @zoomin.started -= instance.OnZoomin;
            @zoomin.performed -= instance.OnZoomin;
            @zoomin.canceled -= instance.OnZoomin;
        }

        public void RemoveCallbacks(ITankActions instance)
        {
            if (m_Wrapper.m_TankActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ITankActions instance)
        {
            foreach (var item in m_Wrapper.m_TankActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_TankActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public TankActions @Tank => new TankActions(this);
    public interface ITankActions
    {
        void OnTurret(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnZoomin(InputAction.CallbackContext context);
    }
}
