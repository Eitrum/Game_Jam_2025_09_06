using System;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty {
        byte TypeId { get; }
        string Name { get; }
        bool IsName(string name);
    }

    public interface ITMLProperty_Xml{
        string GetFormattedXml();
    }

    public interface ITMLProperty_Json {
        void WriteToAsJson(System.Text.StringBuilder sb);
    }

    public interface ITMLProperty_Binary {
        void WriteToAsBinary(Toolkit.IO.IBuffer buffer);
    }
}
