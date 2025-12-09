using System;
using ImGuiNET;
using NFMWorld.Mad;

namespace NFMWorld.Mad.UI
{
    public class DebugCameraWindow
    {
        private bool _isOpen = false;
        private float _fov = 90.0f;
        private int _followY = 0;
        private int _followZ = 0;

        public bool IsOpen => _isOpen;

        public void Toggle()
        {
            _isOpen = !_isOpen;
            
            if (_isOpen)
            {
                // init values from Medium
                _fov = GetFovFromFocusPoint(Medium.FocusPoint);
                _followY = Medium.FollowYOffset;
                _followZ = Medium.FollowZOffset;
            }
        }

        public void Render()
        {
            if (!_isOpen) return;

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(300, 205), ImGuiCond.Always);
            
            bool isOpen = _isOpen;
            if (ImGui.Begin("Debug Camera Settings", ref isOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoNavInputs | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.Spacing();
                
                ImGui.Text("Field of View:");
                if (ImGui.SliderFloat("##FOV", ref _fov, 70.0f, 120.0f, "%.1f°"))
                {
                    Medium.FocusPoint = GetFocusPoint(_fov);
                }
                
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                
                // Follow Y Offset
                ImGui.Text("Follow Y Offset:");
                if (ImGui.SliderInt("##FollowY", ref _followY, -500, 500))
                {
                    Medium.FollowYOffset = _followY;
                }
                
                ImGui.Spacing();
                
                // Follow Z Offset
                ImGui.Text("Follow Z Offset:");
                if (ImGui.SliderInt("##FollowZ", ref _followZ, -500, 500))
                {
                    Medium.FollowZOffset = _followZ;
                }
                
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                
                // Reset button
                if (ImGui.Button("Reset to Defaults", new System.Numerics.Vector2(-1, 0)))
                {
                    _fov = 90.0f;
                    _followY = 0;
                    _followZ = 0;
                    Medium.FocusPoint = GetFocusPoint(_fov);
                    Medium.FollowYOffset = _followY;
                    Medium.FollowZOffset = _followZ;
                }
            }
            ImGui.End();
            
            _isOpen = isOpen;
        }
        
        private float GetFovFromFocusPoint(int focusPoint)
        {
            if (Medium.Cx == 0) return 90.0f;
            float tanValue = (float)focusPoint / Medium.Cx;
            float halfAngle = MathF.Atan(tanValue) * (180.0f / MathF.PI);
            return 180.0f - (halfAngle * 2.0f);
        }
        
        private int GetFocusPoint(float fov)
        {
            return (int)MathF.Round(Medium.Cx * MathF.Tan(MathF.Abs(180 - fov) * 0.5f * (MathF.PI / 180)));
        }
    }
}
