using System;
using UnityEngine;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PasswordAttribute : PropertyAttribute
    {
        //public readonly char Character = '*';

        public PasswordAttribute() { }
        // public PasswordAttribute(char character) { this.Character = character; }
    }
}
