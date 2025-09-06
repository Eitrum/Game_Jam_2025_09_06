using System;

namespace Toolkit
{
    public interface IVerify
    {
        /// <summary>
        /// A method to verify so everything works properly.
        /// </summary>
        /// <param name="error">The error message to be displayed.</param>
        /// <returns>Returns whether it works or not, false if not working properly.</returns>
        bool Verify(out string error);
    }
}
