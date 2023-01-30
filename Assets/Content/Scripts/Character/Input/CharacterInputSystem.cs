using Fray.Input;
using System;
using UnityEngine;

namespace Fray.Character.Input
{
    public enum CharacterAction
    { Move, Look, Dodge }

    public class CharacterInputSystem : MonoBehaviour, ICharacterInputSystem
    {
        private CharacterInputActions.MainActions input;

        private Camera mainCamera;

        public bool Enabled { get; set; }

        public event Action<CharacterInputData> InputEvent = delegate { };

        private void Awake()
        {
            input = new CharacterInputActions().Main;

            mainCamera = Camera.main;
        }

        private void Start()
        {
            input.Enable();
            input.Dodge.performed += DodgeCallback;
        }

        private void DodgeCallback(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!input.enabled || !context.performed) return;
            //Vector3 worldPos = mainCamera.ScreenToWorldPoint(character.Look.ReadValue<Vector2>());
            //Vector2 dir = (worldPos - transform.position).normalized;
            InputEvent?.Invoke(new CharacterInputData(CharacterAction.Dodge, input.Movement.ReadValue<Vector2>()));
        }

        private void FixedUpdate()
        {
            var worldPos = mainCamera.ScreenToWorldPoint(input.Look.ReadValue<Vector2>());
            var dir = (worldPos - transform.position).normalized * 2;
            InputEvent?.Invoke(new CharacterInputData(CharacterAction.Look, (Vector2)worldPos));
            if (!input.enabled)
            {
                InputEvent?.Invoke(new CharacterInputData(CharacterAction.Move, Vector2.zero));
            }
            else
            {
                InputEvent?.Invoke(new CharacterInputData(CharacterAction.Move, input.Movement.ReadValue<Vector2>()));
            }
        }
    }
}