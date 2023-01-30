using System;

namespace Fray.Character.Input
{
    public interface ICharacterInputSystem
    {
        public bool Enabled { get; set; }

        public event Action<CharacterInputData> InputEvent;
    }
}