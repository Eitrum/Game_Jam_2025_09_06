using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Network.Old
{
    public static class Netsh
    {
        #region Classes

        public enum Result
        {
            MissingAdminPrivilege = -1,

            Unknown = 0,
            Success = 1,
        }

        public class Entry
        {
            public string ReservedUrl { get; set; }
            public List<User> Users = new List<User>();

            public override string ToString() {
                return $"Url: {ReservedUrl}\n\t{string.Join("\n\t", Users.Select(x => x.ToString()))}";
            }
        }

        public class User
        {
            public string Name = "";
            public bool Listen = true;
            public bool Delegate = false;
            public string SDDL = "";

            public override string ToString() {
                return $"Name: {Name} -- Listen ({(Listen ? "Yes" : "No")}) -- Delegate ({(Delegate ? "Yes" : "No")}) -- SDDL ({SDDL})";
            }
        }

        #endregion

        #region Run Command

        private static string RunCommand(string command) {
            return Process.Start(new ProcessStartInfo() {
                FileName = "netsh.exe",
                Verb = "runas",
                Arguments = command,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            }).StandardOutput.ReadToEnd();
        }

        private static async Task<string> RunCommandAsync(string command) {
            var task = Process.Start(new ProcessStartInfo() {
                FileName = "netsh.exe",
                Verb = "runas",
                Arguments = command,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            }).StandardOutput.ReadToEndAsync();

            return await task;
        }

        #endregion

        #region Http Add Url

        public static bool Http_AddUrl(string url)
            => Http_AddUrl(url, false);

        public static bool Http_AddUrl(string url, bool ignoreCheck) {
            var res = RunCommand($"http add urlacl url=\"{url}\" user=everyone");
            if(!res.Contains("Error"))
                return true;
            return false;
        }

        #endregion

        #region Http Delete Url

        public static bool Http_DeleteUrl(string url) {
            var res = RunCommand($"http delete urlacl url=\"{url}\"");
            if(!res.Contains("Error"))
                return true;
            return false;
        }

        #endregion

        #region Http Show

        public static bool Http_HasUrl(string url) {
            var entries = Http_ShowUrl(url);
            return entries.Count > 0;
        }

        public static List<Entry> Http_ShowUrl(string url) {
            if(url.Contains('*'))
                url = url.Replace('*', '+');
            var res = RunCommand($"http show urlacl url=\"{url}\"");
            return DecodeEntries(res);
        }

        public static List<Entry> Http_ShowAllUrl() {
            var res = RunCommand($"http show urlacl");
            return DecodeEntries(res);
        }

        private static List<Entry> DecodeEntries(string entireText) {
            var split = entireText.Split(System.Environment.NewLine);
            int index = 0;
            List<Entry> entries = new List<Entry>();
            while(index < split.Length) {
                var e = CreateEntry(split, ref index);
                if(e != null)
                    entries.Add(e);
            }
            return entries;
        }

        private static Entry CreateEntry(string[] lines, ref int index) {
            while(index < lines.Length) {
                if(!lines[index].Contains("Reserved"))
                    index++;
                else
                    break;
            }
            if(index >= lines.Length)
                return null;


            Entry e = new Entry();
            e.ReservedUrl = lines[index++].Split(':', 2)[1].Trim();
            while(index < lines.Length) {
                var u = CreateUser(lines, ref index);
                if(u == null)
                    break;
                e.Users.Add(u);
            }

            return e;
        }

        private static User CreateUser(string[] lines, ref int index) {
            if(string.IsNullOrEmpty(lines[index]))
                return null;
            User u = new User();
            u.Name = lines[index++].Trim().Remove(0, 6);
            if(lines[index].Contains("Listen"))
                u.Listen = lines[index++].EndsWith("Yes") ? true : false;
            if(lines[index].Contains("Delegate"))
                u.Delegate = lines[index++].EndsWith("Yes") ? true : false;
            if(lines[index].Contains("SDDL"))
                u.SDDL = lines[index++].Trim().Remove(0, 6);
            return u;
        }

        #endregion
    }
}
