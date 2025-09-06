using System;
using System.Collections.Generic;

namespace Toolkit.CodeGenerator {

    public interface ICode {
        string GetCode(int indent);
    }

    public interface IFile {
        string FileName { get; }
        string Path { get; }
        string GetCode();
    }

    public interface IContainer : ICode {

    }

    public interface IAttribute : ICode {
        int Count { get; }
    }

    public interface IClass : IContainer {
        AccessModifier AccessModifier { get; }
        string Name { get; }
        string[] Inherits { get; }
    }

    public interface IStruct : IContainer {
        AccessModifier AccessModifier { get; }
        string Name { get; }
        string[] Inherits { get; }
    }

    public interface IEnum : IContainer {
        AccessModifier AccessModifier { get; }
        string Name { get; }
        string Type { get; }
        IVariable[] Variables { get; }
    }

    // namespace interface

    public interface IUsingDirective : ICode {
        string Namespace { get; }
        bool IsStatic { get; }
    }

    public interface INamespace : IContainer, ICode {
        string Namespace { get; }
        IContainer[] Containers { get; }
    }

    public interface IRegion : IContainer {
        string Name { get; }
    }

    public interface IIfPreprocessorDirective : IContainer {
        IComparison Comparison { get; }
    }

    // Variables

    public interface IVariable : ICode {
        AccessModifier AccessModifier { get; }
        string Type { get; }
        string Name { get; }
        string DefaultValue { get; }
        IReadOnlyList<IAttribute> Attributes { get; }
    }

    public interface IProperty : ICode {
        AccessModifier AccessModifier { get; }
        string Type { get; }
        string Name { get; }
        IBlock Get { get; }
        IBlock Set { get; }
    }

    public interface IEventProperty {
        string Type { get; }
        string Name { get; }
        AccessModifier AccessModifier { get; }
        IBlock Add { get; }
        IBlock Remove { get; }
    }

    // Method Body Only

    public interface IMethod : ICode {
        AccessModifier AccessModifier { get; }
        string Type { get; }
        string Name { get; }
        IVariable[] Parameters { get; }
        IBlock Block { get; }
    }

    public interface IBlock : ICode {
        bool IsSingleLine { get; }
    }

    public interface IIfStatement : IBlock {
        IComparison Comparison { get; }
    }

    public interface IComparison {

    }

    public interface IUsingStatement {

    }
}
