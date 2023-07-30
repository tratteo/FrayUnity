using Fray.Character.Input;
using Fray.Systems;
using Fray.Systems.Animation;
using UnityEngine;

namespace Fray.Character
{
    [RequireComponent(typeof(ICharacterInputSystem))]
    [RequireComponent(typeof(AnimatorDriverSystem))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(StaminaSystem))]
    public abstract class CharacterComponent : MonoBehaviour
    {
        private ICharacterInputSystem input;

        public Rigidbody2D Rigidbody { get; private set; }

        public StaminaSystem Stamina { get; private set; }

        public HealthSystem Health { get; private set; }

        public bool Enabled { get; private set; }

        protected ICharacterInputSystem Input => input;

        protected virtual void OnEnable()
        {
            Enabled = true;
            input.InputEvent += OnInput;
        }

        protected virtual void OnDisable()
        {
            Enabled = false;
            input.InputEvent -= OnInput;
        }

        protected virtual void OnInput(CharacterInputData data)
        { }

        protected virtual void Awake()
        {
            Stamina = GetComponent<StaminaSystem>();
            Health = GetComponent<HealthSystem>();
            input = GetComponent<ICharacterInputSystem>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}