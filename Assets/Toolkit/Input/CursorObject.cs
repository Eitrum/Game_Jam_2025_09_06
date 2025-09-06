using UnityEngine;

namespace Toolkit.InputSystem {
    [CreateAssetMenu(menuName = "Toolkit/Input/Cursor")]
    public class CursorObject : ScriptableObject {

        #region Variables

        [SerializeField] private Texture2D texture;
        [SerializeField] private Vector2 hotSpotNormalized = Vector2.zero;

        [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

        #endregion

        #region Properties

        public Texture2D Texture => texture;
        public Vector2 HotSpot {
            get {
                if(texture == null)
                    return Vector2.zero;
                return new Vector2(texture.width * hotSpotNormalized.x, texture.height * hotSpotNormalized.y);
            }
        }
        public Vector2 HotSpotNormalized => hotSpotNormalized;

        public CursorMode Mode => cursorMode;

        #endregion

        #region Apply Without CursorSystem

        public void ApplyWithoutStack() {
            Cursor.SetCursor(texture, hotSpotNormalized, cursorMode);
        }

        #endregion
    }
}
