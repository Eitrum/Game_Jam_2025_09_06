using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Toolkit.IO {
    public static class IOUtility {

        #region Fix Name

        public static string GetValidFileName(string fileName) {
            return fileName
                .Replace('\\', '-')
                .Replace('/', '-')
                .Replace('?', '-')
                .Replace('<', '-')
                .Replace('>', '-')
                .Replace('|', '-')
                .Replace('.', '-')
                .Replace('*', '-')
                .Replace(':', '-');
        }

        #endregion

        #region Path 

        public static bool PathExists(string path, bool createDirectories) {
            path = Path.GetDirectoryName(path);
            if(Directory.Exists(path))
                return true;

            if(!createDirectories)
                return false;

            return Directory.CreateDirectory(path).Exists;
        }

        public static bool PathExistsOrCreate(string path) {
            path = Path.GetDirectoryName(path);
            if(Directory.Exists(path)) {
                return true;
            }

            return Directory.CreateDirectory(path).Exists;
        }

        public static string GetDirectory(string path) => Path.GetDirectoryName(path);
        public static string GetFileName(string path) => Path.GetFileName(path);
        public static string GetFileName(string path, bool withoutFileExtension) {
            if(withoutFileExtension)
                return Path.GetFileNameWithoutExtension(path);
            return Path.GetFileName(path);
        }

        public static void ReplaceFileExtension(ref string path, string newExtension) {
            path = Path.ChangeExtension(path, newExtension);
        }

        public static string[] GetDirectorySplit(string path) => GetPathSplit(GetDirectory(path));
        public static string[] GetPathSplit(string path) => path.Split('\\', '/');

        #endregion

        public static class File {

            #region Write All Bytes

            public static bool TryWrite(string path, byte[] bytes, bool createDirectories = true) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    System.IO.File.WriteAllBytes(path, bytes);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            public static async System.Threading.Tasks.Task<bool> TryWriteAsync(string path, byte[] bytes, bool createDirectories = true, System.Threading.CancellationToken cancellationToken = default) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    await System.IO.File.WriteAllBytesAsync(path, bytes, cancellationToken);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            #endregion

            #region Write All Lines

            public static bool TryWrite(string path, string[] lines, bool createDirectories = true) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    System.IO.File.WriteAllLines(path, lines);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            public static bool TryWrite(string path, IEnumerable<string> lines, bool createDirectories = true) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    System.IO.File.WriteAllLines(path, lines);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            public static async System.Threading.Tasks.Task<bool> TryWriteAsync(string path, string[] lines, bool createDirectories = true, System.Threading.CancellationToken cancellationToken = default) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    await System.IO.File.WriteAllLinesAsync(path, lines, cancellationToken);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            public static async System.Threading.Tasks.Task<bool> TryWriteAsync(string path, IEnumerable<string> lines, bool createDirectories = true, System.Threading.CancellationToken cancellationToken = default) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    await System.IO.File.WriteAllLinesAsync(path, lines, cancellationToken);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            #endregion

            #region Write All Text

            public static bool TryWrite(string path, string text, bool createDirectories = true) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    System.IO.File.WriteAllText(path, text);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            public static async System.Threading.Tasks.Task<bool> TryWriteAsync(string path, string text, bool createDirectories = true, System.Threading.CancellationToken cancellationToken = default) {
                try {
                    if(!PathExists(path, createDirectories))
                        return false;

                    await System.IO.File.WriteAllTextAsync(path, text, cancellationToken);
                    return true;
                }
                catch(System.Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }

            #endregion
        }
    }
}
