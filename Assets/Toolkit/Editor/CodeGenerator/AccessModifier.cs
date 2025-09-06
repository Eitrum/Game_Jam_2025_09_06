using System;

namespace Toolkit.CodeGenerator
{
    [Flags]
    public enum AccessModifier
    {
        None = 0,
        Private = 1,
        Protected = 2,
        Internal = 4,
        Public = 8,
        Static = 16,
        Const = 32,
        Readonly = 64,
        Event = 128,
        Virtual = 256,
        Abstract = 512,
        Override = 1024,
        Sealed = 2048,
        Unsafe = 4096,
        // Parameter Specific
        This = 8192,
        Ref = 16384,
        Out = 32768,
        In = 65536,
        Partial = 131072,


        PrivateProtected = Private | Protected,
        PrivateStatic = Private | Static,
        PrivateConst = Private | Const,
        PrivateReadonly = Private | Readonly,
        PrivateProtectedStatic = PrivateProtected | Static,
        PrivateProtectedConst = PrivateProtected | Const,
        PrivateProtectedReadonly = PrivateProtected | Readonly,

        PublicStatic = Public | Static,
        PublicConst = Public | Const,
        PublicReadonly = Public | Readonly,
        PublicStaticReadonly = Public | Static | Readonly,

        ProtectedInternal = Protected | Internal,
        ProtectedStatic = Protected | Static,
        ProtectedConst = Protected | Const,
        ProtectedReadonly = Protected | Readonly,
        ProtectedInternalStatic = ProtectedInternal | Static,
        ProtectedInternalConst = ProtectedInternal | Const,
        ProtectedInternalReadonly = ProtectedInternal | Readonly,

        InternalStatic = Internal | Static,
        InternalConst = Internal | Const,
        InternalReadonly = Internal | Readonly
    }
}
