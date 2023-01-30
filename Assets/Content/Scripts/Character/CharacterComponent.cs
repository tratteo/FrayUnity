using Fray.Character.Input;
using Fray.Systems;
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

        public bool Enabled { get; private set; }

        protected AnimatorDriverSystem Animator { get; private set; }

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
            Animator = GetComponent<AnimatorDriverSystem>();
            input = GetComponent<ICharacterInputSystem>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}