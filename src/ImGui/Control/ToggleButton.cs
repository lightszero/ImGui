﻿namespace ImGui
{
    internal class ToggleButton
    {
        public static bool DoControl(Rect rect, Content content, bool value, string id)
        {
            var result = value;
            var hovered = rect.Contains(Form.current.GetMousePos());

            //control logic
            var uiState = Form.current.uiState;
            uiState.KeepAliveId(id);
            if (hovered)
            {
                uiState.SetHoverId(id);

                if (Input.Mouse.LeftButtonPressed)
                {
                    uiState.ActiveId = id;
                }

                if (uiState.ActiveId == id && Input.Mouse.LeftButtonReleased)
                {
                    result = !value;
                    uiState.SetActiveId(GUIState.None);
                }
            }

            // ui representation
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
                if (uiState.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUI.Active;
                }
            }
            if (result)
            {
                state = GUI.Active;
            }

            // ui painting
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Button[state]);
            }
            
            return result;
        }
    }
}