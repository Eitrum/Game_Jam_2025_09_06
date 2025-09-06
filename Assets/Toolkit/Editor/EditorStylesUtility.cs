using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace Toolkit {
    public static class EditorStylesUtility {
        #region Override Default Stuff

        private static bool isHoldingAlt;
        private static bool isHoldingShift;
        private static bool isHoldingCtrl;

        [InitializeOnLoadMethod]
        private static void LoadGlobalEventHandler() {
            FieldInfo gEvHandler = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.NonPublic);
            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)gEvHandler.GetValue(null);
            value += OnEventTriggered;
            gEvHandler.SetValue(null, value);
        }

        private static void OnEventTriggered() {
            var ev = Event.current;
            if(ev != null && ev.isKey) {
                var isAlt = ev.alt;
                isHoldingAlt = isAlt;
                isHoldingShift = ev.shift;
                isHoldingCtrl = ev.control;
                var valueBefore = EditorStyles.label.richText;
                EditorStyles.label.richText = isAlt;
                EditorStyles.textArea.richText = isAlt;
                EditorStyles.textField.richText = isAlt;
                if(EditorStyles.label.richText != valueBefore) {
                    ToolkitEditorUtility.RepaintInspectors();
                    ToolkitEditorUtility.RepaintInspectorsDelayed();
                }
            }
        }

        public static bool IsHoldingAlt => isHoldingAlt || (Event.current?.alt ?? false);
        public static bool IsHoldingShift => isHoldingShift || (Event.current?.shift ?? false);
        public static bool IsHoldingCtrl => isHoldingCtrl || (Event.current?.control ?? false);

        #endregion

        #region Simple Labels

        public static GUIStyle Label => EditorStyles.label;
        public static GUIStyle BoldLabel => EditorStyles.boldLabel;

        private static GUIStyle rightAlignedLabel;
        public static GUIStyle RightAlignedLabel {
            get {
                if(rightAlignedLabel == null) {
                    rightAlignedLabel = new GUIStyle(Label);
                    rightAlignedLabel.alignment = TextAnchor.UpperRight;
                }
                rightAlignedLabel.richText = IsHoldingAlt;
                return rightAlignedLabel;
            }
        }

        private static GUIStyle rightAlignedGrayMiniLabel;
        public static GUIStyle RightAlignedGrayMiniLabel {
            get {
                if(rightAlignedGrayMiniLabel == null) {
                    rightAlignedGrayMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                    rightAlignedGrayMiniLabel.alignment = TextAnchor.UpperRight;
                }
                rightAlignedGrayMiniLabel.richText = IsHoldingAlt;
                return rightAlignedGrayMiniLabel;
            }
        }

        private static GUIStyle centerAlignedMiniLabel;
        public static GUIStyle CenterAlignedMiniLabel {
            get {
                if(centerAlignedMiniLabel == null) {
                    centerAlignedMiniLabel = new GUIStyle(EditorStyles.label);
                    centerAlignedMiniLabel.fontSize = EditorStyles.centeredGreyMiniLabel.fontSize;
                    centerAlignedMiniLabel.alignment = TextAnchor.UpperCenter;
                }
                centerAlignedMiniLabel.richText = IsHoldingAlt;
                return centerAlignedMiniLabel;
            }
        }

        private static GUIStyle centerAlignedLabel;
        public static GUIStyle CenterAlignedLabel {
            get {
                if(centerAlignedLabel == null) {
                    centerAlignedLabel = new GUIStyle(Label);
                    centerAlignedLabel.alignment = TextAnchor.MiddleCenter;
                }
                centerAlignedLabel.richText = IsHoldingAlt;
                return centerAlignedLabel;
            }
        }

        private static GUIStyle rightAlignedBoldLabel;
        public static GUIStyle RightAlignedBoldLabel {
            get {
                if(rightAlignedBoldLabel == null) {
                    rightAlignedBoldLabel = new GUIStyle(Label);
                    rightAlignedBoldLabel.alignment = TextAnchor.UpperRight;
                    rightAlignedBoldLabel.fontStyle = FontStyle.Bold;
                }
                rightAlignedBoldLabel.richText = IsHoldingAlt;
                return rightAlignedBoldLabel;
            }
        }

        private static GUIStyle centerAlignedBoldLabel;
        public static GUIStyle CenterAlignedBoldLabel {
            get {
                if(centerAlignedBoldLabel == null) {
                    centerAlignedBoldLabel = new GUIStyle(Label);
                    centerAlignedBoldLabel.alignment = TextAnchor.MiddleCenter;
                    centerAlignedBoldLabel.fontStyle = FontStyle.Bold;
                }
                centerAlignedBoldLabel.richText = IsHoldingAlt;
                return centerAlignedBoldLabel;
            }
        }

        private static GUIStyle italicLabel;
        public static GUIStyle ItalicLabel {
            get {
                if(italicLabel == null) {
                    italicLabel = new GUIStyle(Label);
                    italicLabel.fontStyle = FontStyle.Italic;
                }
                return italicLabel;
            }
        }

        private static GUIStyle grayItalicLabel;
        public static GUIStyle GrayItalicLabel {
            get {
                if(grayItalicLabel == null) {
                    grayItalicLabel = new GUIStyle(Label);
                    grayItalicLabel.fontStyle = FontStyle.Italic;
                    grayItalicLabel.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 0.7f);
                }
                grayItalicLabel.richText = IsHoldingAlt;
                return grayItalicLabel;
            }
        }

        #endregion

        #region Button

        private static GUIStyle richTextButtonWrapped;
        public static GUIStyle RichTextButtonWrapped {
            get {
                if(richTextButtonWrapped == null) {
                    richTextButtonWrapped = new GUIStyle("button");
                    richTextButtonWrapped.richText = true;
                    richTextButtonWrapped.wordWrap = true;
                }
                return richTextButtonWrapped;
            }
        }

        #endregion

        #region Rich Text

        private static GUIStyle richTextLabel;
        public static GUIStyle RichTextLabel {
            get {
                if(richTextLabel == null) {
                    richTextLabel = new GUIStyle(Label);
                    richTextLabel.richText = true;
                }
                return richTextLabel;
            }
        }

        private static GUIStyle richTextLabelWrapped;
        public static GUIStyle RichTextLabelWrapped {
            get {
                if(richTextLabelWrapped == null) {
                    richTextLabelWrapped = new GUIStyle(Label);
                    richTextLabelWrapped.richText = true;
                    richTextLabelWrapped.wordWrap = true;
                }
                return richTextLabelWrapped;
            }
        }

        private static GUIStyle richTextBoldLabel;
        public static GUIStyle RichTextBoldLabel {
            get {
                if(richTextBoldLabel == null) {
                    richTextBoldLabel = new GUIStyle(Label);
                    richTextBoldLabel.richText = true;
                    richTextBoldLabel.fontStyle = FontStyle.Bold;
                }
                return richTextBoldLabel;
            }
        }

        private static GUIStyle richTextRightAlignedLabel;
        public static GUIStyle RichTextRightAlignedLabel {
            get {
                if(richTextRightAlignedLabel == null) {
                    richTextRightAlignedLabel = new GUIStyle(Label);
                    richTextRightAlignedLabel.alignment = TextAnchor.UpperRight;
                    richTextRightAlignedLabel.richText = true;
                }
                return richTextRightAlignedLabel;
            }
        }

        private static GUIStyle richTextCenterAlignedLabel;
        public static GUIStyle RichTextCenterAlignedLabel {
            get {
                if(richTextCenterAlignedLabel == null) {
                    richTextCenterAlignedLabel = new GUIStyle(Label);
                    richTextCenterAlignedLabel.alignment = TextAnchor.UpperCenter;
                    richTextCenterAlignedLabel.richText = true;
                }
                return richTextCenterAlignedLabel;
            }
        }

        private static GUIStyle richTextRightAlignedBoldLabel;
        public static GUIStyle RichTextRightAlignedBoldLabel {
            get {
                if(richTextRightAlignedBoldLabel == null) {
                    richTextRightAlignedBoldLabel = new GUIStyle(Label);
                    richTextRightAlignedBoldLabel.alignment = TextAnchor.UpperRight;
                    richTextRightAlignedBoldLabel.richText = true;
                    richTextRightAlignedBoldLabel.fontStyle = FontStyle.Bold;
                }
                return richTextRightAlignedBoldLabel;
            }
        }

        private static GUIStyle richTextCenterAlignedBoldLabel;
        public static GUIStyle RichTextCenterAlignedBoldLabel {
            get {
                if(richTextCenterAlignedBoldLabel == null) {
                    richTextCenterAlignedBoldLabel = new GUIStyle(Label);
                    richTextCenterAlignedBoldLabel.alignment = TextAnchor.UpperCenter;
                    richTextCenterAlignedBoldLabel.richText = true;
                    richTextCenterAlignedBoldLabel.fontStyle = FontStyle.Bold;
                }
                return richTextCenterAlignedBoldLabel;
            }
        }

        #endregion

        #region Popup

        private static GUIStyle boldPopup;
        public static GUIStyle BoldPopup {
            get {
                if(boldPopup == null) {
                    boldPopup = new GUIStyle(EditorStyles.popup);
                    boldPopup.fontStyle = FontStyle.Bold;
                }
                boldPopup.richText = IsHoldingAlt;
                return boldPopup;
            }
        }

        #endregion

        #region Text Area

        private static GUIStyle textArea;
        public static GUIStyle TextArea {
            get {
                if(textArea == null) {
                    textArea = new GUIStyle(EditorStyles.textArea);
                }
                textArea.richText = IsHoldingAlt;
                return textArea;
            }
        }

        private static GUIStyle textAreaRichText;
        public static GUIStyle TextAreaRichText {
            get {
                if(textAreaRichText == null) {
                    textAreaRichText = new GUIStyle(EditorStyles.textArea);
                    textAreaRichText.richText = true;
                }
                return textAreaRichText;
            }
        }

        #endregion
    }
}
