using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Toolkit {
    public static class LoggingExtensions {

        #region Filter Util

        [System.Flags]
        public enum Filter {
            None = 0,
            Variables = 1 << 0,
            Properties = 1 << 1,
            Methods = 1 << 2,
            // Children = 1 << 3,
            Private = 1 << 4,
            Public = 1 << 5,
            Static = 1 << 6,

            AllData = Variables | Properties | Private  | Public,
            All = Variables | Properties | Methods | Private | Public,
            AllMethods = Methods | Private | Public
        }

        private static System.Reflection.BindingFlags ToBindingFlags(this Filter filter) {
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance;
            if(filter.HasFlag(Filter.Public))
                flags |= System.Reflection.BindingFlags.Public;
            if(filter.HasFlag(Filter.Private))
                flags |= System.Reflection.BindingFlags.NonPublic;
            if(filter.HasFlag(Filter.Static))
                flags |= System.Reflection.BindingFlags.Static;
            return flags;
        }

        public enum RecursiveMode {
            None,
            Variables = 1,
            Properties = 2,
            All = 3,
        }

        #endregion

        #region FormatLog

        public static string FormatLog<Tag>(this Tag script, string msg) {
            return (script is Component comp && comp) ?
                $"[{typeof(Tag).FullName}] - ({comp.name}) {msg}" :
                $"[{typeof(Tag).FullName}] - {msg}";
        }

        public static string FormatLog<Tag>(this Tag script, string msg, ColorTableType color)
            => FormatLog(script, msg, ColorTable.GetColor(color));

        public static string FormatLog<Tag>(this Tag script, string msg, Color32 color)
            => FormatLog(script, msg, (Color)color);

        public static string FormatLog<Tag>(this Tag script, string msg, Color color) {
            return (script is Component comp && comp) ?
                $"<color=#{color.ToHex24()}>[{typeof(Tag).FullName}]</color> - ({comp.name}) {msg}" :
                $"<color=#{color.ToHex24()}>[{typeof(Tag).FullName}]</color> - {msg}";
        }

        // 1 items
        public static string FormatLog<Tag, T0>(this Tag script, T0 item0)
            => FormatLog(script, $"{item0}");

        public static string FormatLog<Tag, T0>(this Tag script, T0 item0, ColorTableType color)
            => FormatLog(script, $"{item0}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0>(this Tag script, T0 item0, Color color)
            => FormatLog(script, $"{item0}", color);

        // 2 items
        public static string FormatLog<Tag, T0, T1>(this Tag script, T0 item0, T1 item1)
            => FormatLog(script, $"{item0} | {item1}");

        public static string FormatLog<Tag, T0, T1>(this Tag script, T0 item0, T1 item1, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1>(this Tag script, T0 item0, T1 item1, Color color)
            => FormatLog(script, $"{item0} | {item1}", color);

        // 3 items
        public static string FormatLog<Tag, T0, T1, T2>(this Tag script, T0 item0, T1 item1, T2 item2)
            => FormatLog(script, $"{item0} | {item1} | {item2}");

        public static string FormatLog<Tag, T0, T1, T2>(this Tag script, T0 item0, T1 item1, T2 item2, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1} | {item2}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1, T2>(this Tag script, T0 item0, T1 item1, T2 item2, Color color)
            => FormatLog(script, $"{item0} | {item1} | {item2}", color);

        // 4 items
        public static string FormatLog<Tag, T0, T1, T2, T3>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3}");

        public static string FormatLog<Tag, T0, T1, T2, T3>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1, T2, T3>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, Color color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3}", color);

        // 5 items
        public static string FormatLog<Tag, T0, T1, T2, T3, T4>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4}");

        public static string FormatLog<Tag, T0, T1, T2, T3, T4>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1, T2, T3, T4>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, Color color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4}", color);

        // 6 items
        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5}");

        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, Color color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5}", color);

        // 7 items
        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5, T6>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5} | {item6}");

        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5, T6>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5} | {item6}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5, T6>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, Color color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5} | {item6}", color);

        // 8 items
        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5, T6, T7>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5} | {item6} | {item7}");

        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5, T6, T7>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, ColorTableType color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5} | {item6} | {item7}", ColorTable.GetColor(color));

        public static string FormatLog<Tag, T0, T1, T2, T3, T4, T5, T6, T7>(this Tag script, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, Color color)
            => FormatLog(script, $"{item0} | {item1} | {item2} | {item3} | {item4} | {item5} | {item6} | {item7}", color);

        #endregion

        #region FormatData

        public static string FormatLogWithData<Tag>(this Tag script, string msg, Filter filter = Filter.AllData) {
            return (script is Component comp && comp) ?
                $"[{typeof(Tag).FullName}] - ({comp.name}) {msg}\n{FormatData(script, filter)}" :
                $"[{typeof(Tag).FullName}] - {msg}\n{FormatData(script, filter)}";
        }

        public static string FormatLogWithData<Tag>(this Tag script, string msg, ColorTableType color, Filter filter = Filter.AllData)
            => FormatLogWithData(script, msg, ColorTable.GetColor(color), filter);

        public static string FormatLogWithData<Tag>(this Tag script, string msg, Color32 color, Filter filter = Filter.AllData)
            => FormatLogWithData(script, msg, (Color)color, filter);

        public static string FormatLogWithData<Tag>(this Tag script, string msg, Color color, Filter filter = Filter.AllData) {
            return (script is Component comp && comp) ?
                $"<color=#{color.ToHex24()}>[{typeof(Tag).FullName}]</color> - ({comp.name}) {msg}\n{FormatData(script, filter)}" :
                $"<color=#{color.ToHex24()}>[{typeof(Tag).FullName}]</color> - {msg}\n{FormatData(script, filter)}";
        }

        public static string FormatData<Tag>(this Tag script, Filter filter = Filter.AllData) {
            StringBuilder sb = new StringBuilder();
            try {
                var t = typeof(Tag);
                if(filter.HasFlag(Filter.Variables)) {
                    sb.AppendLine("---variables---");
                    var fields = t.GetFields(filter.ToBindingFlags());
                    foreach(var f in fields)
                        sb.AppendLine($"{f.Name} = {f.GetValue(script)}");
                }
                if(filter.HasFlag(Filter.Properties)) {
                    sb.AppendLine("---properties---");
                    var prop = t.GetProperties(filter.ToBindingFlags() | System.Reflection.BindingFlags.GetProperty);
                    foreach(var f in prop)
                        sb.AppendLine($"{f.Name} = {f.GetValue(script)}");
                }
                if(filter.HasFlag(Filter.Methods)) {
                    sb.AppendLine("---methods---");
                    var methods = t.GetMethods(filter.ToBindingFlags());
                    foreach(var m in methods)
                        sb.AppendLine($"{m.Name} {m.ReturnType} = ({string.Join(", ", m.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}"))})");
                }
            }
            catch(Exception) { }
            return sb.ToString();
        }

        #endregion
    }
}
