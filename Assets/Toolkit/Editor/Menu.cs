using System;
using UnityEngine;

namespace Toolkit {
    public static class Menu {

        #region Cached

        private static System.Reflection.MethodInfo setHotkeyMethod;
        private static System.Reflection.MethodInfo getHotkeyMethod;

        private static System.Reflection.MethodInfo addMenuItemMethod;
        private static System.Reflection.MethodInfo removeMenuItemMethod;
        private static System.Reflection.MethodInfo menuItemExistsMethod;
        private static System.Reflection.MethodInfo extractSubmenusMethod;
        private static System.Reflection.MethodInfo rebuildAllMenusMethod;
        private static System.Reflection.MethodInfo onMenuChangedMethod;

        #endregion

        #region Callback

        public static event Action OnMenuUpdated;

        #endregion

        #region Enabled

        public static bool GetEnabled(string menuPath) {
            return UnityEditor.Menu.GetEnabled(menuPath);
        }

        #endregion

        #region Checked

        public static void SetChecked(string menuPath, bool isChecked) {
            UnityEditor.Menu.SetChecked(menuPath, isChecked);
        }

        public static bool GetChecked(string menuPath) {
            return UnityEditor.Menu.GetChecked(menuPath);
        }

        #endregion

        #region Hotkey

        public static void SetHotkey(string menuPath, string hotkey) {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "SetHotkey", ref setHotkeyMethod))
                return;
            setHotkeyMethod?.InvokeStatic(menuPath, hotkey);
        }

        public static string GetHotkey(string menuPath) {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "GetHotkey", ref getHotkeyMethod))
                return string.Empty;
            return getHotkeyMethod?.InvokeStatic<string, string>(menuPath);
        }

        #endregion

        #region Add MenuItem

        public static void AddMenuItem(string name, Action execute, Func<bool> validate = null) {
            AddMenuItem(name, string.Empty, false, 0, execute, validate);
        }

        public static void AddMenuItem(string name, int priority, Action execute, Func<bool> validate = null) {
            AddMenuItem(name, string.Empty, false, priority, execute, validate);
        }

        public static void AddMenuItem(string name, string shortcut, bool @checked, int priority, Action execute, Func<bool> validate) {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "AddMenuItem", ref addMenuItemMethod))
                return;
            addMenuItemMethod.InvokeStatic(name, shortcut, @checked, priority, execute, validate);
        }

        #endregion

        #region Remove MenuItem

        public static void RemoveMenuItem(string name) {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "RemoveMenuItem", ref removeMenuItemMethod))
                return;
            removeMenuItemMethod.InvokeStatic(name);
        }

        #endregion

        #region Exists

        public static bool MenuItemExists(string menuItem) {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "MenuItemExists", ref menuItemExistsMethod))
                return false;
            return menuItemExistsMethod.InvokeStatic<string, bool>(menuItem);
        }

        #endregion

        #region Extract

        public static bool ExtractSubmenus(string menu, out string[] submenus) {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "ExtractSubmenus", ref extractSubmenusMethod)) {
                submenus = null;
                return false;
            }
            try {
                submenus = extractSubmenusMethod.InvokeStatic<string, string[]>(menu);
                return submenus != null;
            }
            catch {
                submenus = null;
                return false;
            }
        }

        #endregion

        #region Rebuild

        public static void RebuildAllMenus() {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "RebuildAllMenus", ref rebuildAllMenusMethod)) {
                return;
            }
            rebuildAllMenusMethod.InvokeStatic();
            OnMenuUpdated?.Invoke();
        }

        #endregion

        #region Changed

        public static void OnMenuChanged() {
            if(!UnityInternalUtility.TryCacheMethod(typeof(UnityEditor.Menu), "OnMenuChanged", ref onMenuChangedMethod)) {
                return;
            }
            onMenuChangedMethod.InvokeStatic();
        }

        #endregion
    }
}
