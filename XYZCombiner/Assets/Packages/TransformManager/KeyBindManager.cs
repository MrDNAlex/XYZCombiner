using System.Collections.Generic;
using UnityEngine;

namespace DNATransformManager
{
    /// <summary>
    /// Manager that maps KeyBinds to Actions
    /// </summary>
    public class KeyBindManager
    {
        /// <summary>
        /// Enum of Actions the Keybinds Map to
        /// </summary>
        public enum KeyBindAction
        {
            None,
            Translation,
            Rotation,
            AlignVectors,
            ReturnToOrigin,
            SaveVector,
            ClearVectors,
            RemoveObject,
            ObjectToVectorStart
        }

        /// <summary>
        /// The Default list of Keybinds associated to each action
        /// </summary>
        private readonly List<KeyCode> DefaultKeyBinds = new List<KeyCode>() { KeyCode.T, KeyCode.R, KeyCode.Q, KeyCode.O, KeyCode.S, KeyCode.C, KeyCode.Backspace, KeyCode.LeftControl };

        /// <summary>
        /// Stores the List of Keybinds Related to an action
        /// </summary>
        public Dictionary<KeyCode, KeyBindAction> ActionFromKeyCode;

        /// <summary>
        /// Initializes the Keybind Manager with the Default Settings
        /// </summary>
        public KeyBindManager()
        {
            ActionFromKeyCode = new Dictionary<KeyCode, KeyBindAction>();
            ActionFromKeyCode.Add(DefaultKeyBinds[0], KeyBindAction.Translation);
            ActionFromKeyCode.Add(DefaultKeyBinds[1], KeyBindAction.Rotation);
            ActionFromKeyCode.Add(DefaultKeyBinds[2], KeyBindAction.AlignVectors);
            ActionFromKeyCode.Add(DefaultKeyBinds[3], KeyBindAction.ReturnToOrigin);
            ActionFromKeyCode.Add(DefaultKeyBinds[4], KeyBindAction.SaveVector);
            ActionFromKeyCode.Add(DefaultKeyBinds[5], KeyBindAction.ClearVectors);
            ActionFromKeyCode.Add(DefaultKeyBinds[6], KeyBindAction.RemoveObject);
            ActionFromKeyCode.Add(DefaultKeyBinds[7], KeyBindAction.ObjectToVectorStart);
        }

        /// <summary>
        /// Modifies the KeyBinds Key and Maps it to the Action 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newKeyBind"></param>
        public void ModifyKeybind(KeyBindAction action, KeyCode newKeyBind)
        {
            foreach (var keyBind in ActionFromKeyCode)
            {
                if (keyBind.Value == action)
                {
                    ActionFromKeyCode.Remove(keyBind.Key);
                    ActionFromKeyCode.Add(newKeyBind, action);
                }
            }
        }

        /// <summary>
        /// Returns the Action from a Specific Keybind
        /// </summary>
        /// <returns></returns>
        public KeyBindAction GetAction()
        {
            foreach (var keyBind in ActionFromKeyCode)
            {
                if (Input.GetKeyDown(keyBind.Key))
                    return ActionFromKeyCode[keyBind.Key];
            }
            return KeyBindAction.None;
        }
    }
}