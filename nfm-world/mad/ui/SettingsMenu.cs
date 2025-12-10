using ImGuiNET;
using System.Numerics;
using System.IO;
using System.Globalization;

namespace NFMWorld.Mad.UI;

/// <summary>
/// Settings menu with tabs, similar to Half-Life 1 style
/// </summary>
public class SettingsMenu
{
    private bool _isOpen;

    public static DevConsoleWriter Writer = null!;
    private int _selectedTab = 0;
    
    private readonly string[] _tabNames = { "Keyboard", "Video", "Audio", "Game" };

    // Video settings
    private int _selectedRenderer = 0;
    private readonly string[] _renderers = { "SkiaSharp" };
    private int _selectedResolution = 3;
    private readonly string[] _resolutions = { "800 x 600", "1024 x 768", "1280 x 720", "1280 x 1024", "1920 x 1080", "2560 x 1440" };
    private int _selectedDisplayMode = 1;
    private readonly string[] _displayModes = { "Fullscreen", "Windowed", "Borderless" };
    private bool _vsync = false;
    private float _brightness = 0.5f;
    private float _gamma = 0.5f;

    // Audio settings
    private float _masterVolume = 1.0f;
    private float _musicVolume = 0.8f;
    private float _effectsVolume = 0.9f;
    private bool _muteAll = false;

    // Game settings (Camera)
    private float _fov = 90.0f;
    private int _followY = 0;
    private int _followZ = 0;

    // Keyboard settings
    private string _settingMessage = "";

    public bool IsOpen => _isOpen;

    public void Open()
    {
        _isOpen = true;
        
        // Load current game settings
        _fov = GetFovFromFocusPoint(Medium.FocusPoint);
        _followY = Medium.FollowYOffset;
        _followZ = Medium.FollowZOffset;
    }

    public void Close()
    {
        _isOpen = false;
    }

    public void Render()
    {
        if (!_isOpen)
            return;

        // Set window size and position
        var viewport = ImGui.GetMainViewport();
        var center = viewport.GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(570, 390), ImGuiCond.Appearing);

        ImGuiWindowFlags flags = ImGuiWindowFlags.NoCollapse;

        if (ImGui.Begin("Options", ref _isOpen, flags))
        {
            DrawTabs();
            
            ImGui.Spacing();

            // Calculate height for scrollable content area (leave room for bottom buttons)
            float bottomButtonsHeight = 60f; // Height for separator + buttons + padding
            float availableHeight = ImGui.GetContentRegionAvail().Y - bottomButtonsHeight;

            // Scrollable content area
            if (ImGui.BeginChild("SettingsContent", new Vector2(0, availableHeight)))
            {
                // Draw content based on selected tab
                switch (_selectedTab)
                {
                    case 0: DrawKeyboardTab(); break;
                    case 1: DrawVideoTab(); break;
                    case 2: DrawAudioTab(); break;
                    case 3: DrawGameTab(); break;
                }
            }
            ImGui.EndChild();

            // Static bottom section
            ImGui.Separator();
            DrawBottomButtons();

            ImGui.End();
        }
    }

    private void DrawTabs()
    {
        if (ImGui.BeginTabBar("SettingsTabs", ImGuiTabBarFlags.None))
        {
            for (int i = 0; i < _tabNames.Length; i++)
            {
                if (ImGui.BeginTabItem(_tabNames[i]))
                {
                    _selectedTab = i;
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawAudioTab()
    {
        ImGui.Text("Audio Settings");
        ImGui.Spacing();

        ImGui.Checkbox("Mute All", ref _muteAll);
        ImGui.Spacing();

        ImGui.Text("Master Volume");
        ImGui.SliderFloat("##MasterVolume", ref _masterVolume, 0.0f, 1.0f, "%.2f");
        
        ImGui.Text("Music Volume");
        ImGui.SliderFloat("##MusicVolume", ref _musicVolume, 0.0f, 1.0f, "%.2f");
        
        ImGui.Text("Effects Volume");
        ImGui.SliderFloat("##EffectsVolume", ref _effectsVolume, 0.0f, 1.0f, "%.2f");
    }

    private void DrawVideoTab()
    {
        ImGui.Text("Video Settings");
        ImGui.Spacing();

        // ImGui.Text("Renderer");
        // ImGui.Combo("##Renderer", ref _selectedRenderer, _renderers, _renderers.Length);
        
        ImGui.Text("Resolution");
        ImGui.Combo("##Resolution", ref _selectedResolution, _resolutions, _resolutions.Length);
        
        ImGui.Text("Display Mode");
        ImGui.Combo("##DisplayMode", ref _selectedDisplayMode, _displayModes, _displayModes.Length);
        
        ImGui.Spacing();
        ImGui.Checkbox("Wait for vertical sync", ref _vsync);
        
        ImGui.Spacing();
        ImGui.Text("Brightness");
        float sliderWidth = ImGui.GetContentRegionAvail().X;
        ImGui.SetNextItemWidth(sliderWidth);
        ImGui.SliderFloat("##Brightness", ref _brightness, 0.0f, 1.0f, "%.2f");
        float startX = ImGui.GetCursorPosX();
        ImGui.TextDisabled("Dark");
        ImGui.SameLine();
        ImGui.SetCursorPosX(startX + sliderWidth - ImGui.CalcTextSize("Light").X);
        ImGui.TextDisabled("Light");
        
        ImGui.Text("Gamma");
        ImGui.SetNextItemWidth(sliderWidth);
        ImGui.SliderFloat("##Gamma", ref _gamma, 0.0f, 1.0f, "%.2f");
        ImGui.TextDisabled("Low");
        ImGui.SameLine();
        ImGui.SetCursorPosX(startX + sliderWidth - ImGui.CalcTextSize("High").X);
        ImGui.TextDisabled("High");

        ImGui.Spacing();
        // ImGui.TextColored(new Vector4(1.0f, 0.8f, 0.4f, 1.0f), 
        //     "Note: changing some video options will cause the game to exit and restart.");
    }

    private void DrawKeyboardTab()
    {
        ImGui.Text("Keyboard Settings");
        ImGui.Spacing();
        ImGui.TextWrapped("Key binding configuration will be added here.");
    }

    private void DrawGameTab()
    {
        ImGui.Text("Camera Settings");
        ImGui.Spacing();
        
        ImGui.Text("Field of View");
        ImGui.SliderFloat("##FOV", ref _fov, 70.0f, 120.0f, "%.1f°");
        
        ImGui.Spacing();
        ImGui.Text("Follow Y Offset");
        ImGui.SliderInt("##FollowY", ref _followY, -160, 500);
        
        ImGui.Spacing();
        ImGui.Text("Follow Z Offset");
        ImGui.SliderInt("##FollowZ", ref _followZ, -500, 500);
        
        ImGui.Spacing();
        if (ImGui.Button("Reset Camera Defaults", new Vector2(-1, 0)))
        {
            _fov = 90.0f;
            _followY = 0;
            _followZ = 0;
        }
    }

    private void DrawBottomButtons()
    {
        float buttonWidth = 100f;
        float spacing = 10f;
        float totalWidth = buttonWidth * 3 + spacing * 2;
        
        ImGui.SetCursorPosX((ImGui.GetWindowWidth() - totalWidth) * 0.5f);

        if (ImGui.Button("OK", new Vector2(buttonWidth, 30)))
        {
            ApplySettings();
            _isOpen = false;
        }

        ImGui.SameLine(0, spacing);

        if (ImGui.Button("Cancel", new Vector2(buttonWidth, 30)))
        {
            _isOpen = false;
        }

        ImGui.SameLine(0, spacing);

        if (ImGui.Button("Apply", new Vector2(buttonWidth, 30)))
        {
            ApplySettings();
        }

        // Show message if settings were applied
        if (!string.IsNullOrEmpty(_settingMessage))
        {
            ImGui.Spacing();
            ImGui.TextColored(new Vector4(0.2f, 1.0f, 0.2f, 1.0f), _settingMessage);
        }
    }

    private void ApplySettings()
    {
        // Here you would actually apply the settings to the game
        // For now, just show a confirmation message
        _settingMessage = "Settings applied successfully!";
        
        // Apply audio settings
        if (_muteAll)
        {
            // Mute all sounds
        }
        else
        {
            // Apply volume settings
            // GameSparker.SetMasterVolume(_masterVolume);
            // GameSparker.SetMusicVolume(_musicVolume);
            // GameSparker.SetEffectsVolume(_effectsVolume);
        }

        // Apply camera settings
        Medium.FocusPoint = GetFocusPoint(_fov);
        Medium.FollowYOffset = _followY;
        Medium.FollowZOffset = _followZ;
        
        // Save config to file
        SaveConfig();
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
    
    private void SaveConfig()
    {
        try
        {
            string configPath = Path.Combine("data", "cfg", "config.cfg");
            Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
            
            using (StreamWriter cfgWriter = new StreamWriter(configPath))
            {
                cfgWriter.WriteLine("// NFM-World Configuration File");
                cfgWriter.WriteLine("// Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cfgWriter.WriteLine();
                
                // Video settings
                cfgWriter.WriteLine("// Video Settings");
                cfgWriter.WriteLine($"video_resolution {_selectedResolution}");
                cfgWriter.WriteLine($"video_displaymode {_selectedDisplayMode}");
                cfgWriter.WriteLine($"video_vsync {(_vsync ? 1 : 0)}");
                cfgWriter.WriteLine($"video_brightness {_brightness.ToString("F2", CultureInfo.InvariantCulture)}");
                cfgWriter.WriteLine($"video_gamma {_gamma.ToString("F2", CultureInfo.InvariantCulture)}");
                cfgWriter.WriteLine();
                
                // Audio settings
                cfgWriter.WriteLine("// Audio Settings");
                cfgWriter.WriteLine($"audio_mute {(_muteAll ? 1 : 0)}");
                cfgWriter.WriteLine($"audio_master {_masterVolume.ToString("F2", CultureInfo.InvariantCulture)}");
                cfgWriter.WriteLine($"audio_music {_musicVolume.ToString("F2", CultureInfo.InvariantCulture)}");
                cfgWriter.WriteLine($"audio_effects {_effectsVolume.ToString("F2", CultureInfo.InvariantCulture)}");
                cfgWriter.WriteLine();
                
                // Camera settings
                cfgWriter.WriteLine("// Camera Settings");
                cfgWriter.WriteLine($"camera_fov {_fov.ToString("F1", CultureInfo.InvariantCulture)}");
                cfgWriter.WriteLine($"camera_follow_y {_followY}");
                cfgWriter.WriteLine($"camera_follow_z {_followZ}");
            }
            
            Writer?.WriteLine($"Config saved to {configPath}", "debug");
        }
        catch (Exception ex)
        {
            Writer?.WriteLine($"Error saving config: {ex.Message}", "error");
        }
    }
    
    public void LoadConfig()
    {
        try
        {
            string configPath = Path.Combine("data", "cfg", "config.cfg");
            
            if (!File.Exists(configPath))
            {
                Writer?.WriteLine("No config file found, using defaults.", "warning");
                return;
            }
            
            string[] lines = File.ReadAllLines(configPath);
            
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//"))
                    continue;
                
                string[] parts = trimmed.Split(' ', 2);
                if (parts.Length != 2)
                    continue;
                
                string key = parts[0];
                string value = parts[1];
                
                try
                {
                    switch (key)
                    {
                        // Video settings
                        case "video_resolution":
                            _selectedResolution = int.Parse(value);
                            break;
                        case "video_displaymode":
                            _selectedDisplayMode = int.Parse(value);
                            break;
                        case "video_vsync":
                            _vsync = int.Parse(value) != 0;
                            break;
                        case "video_brightness":
                            _brightness = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "video_gamma":
                            _gamma = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        
                        // Audio settings
                        case "audio_mute":
                            _muteAll = int.Parse(value) != 0;
                            break;
                        case "audio_master":
                            _masterVolume = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "audio_music":
                            _musicVolume = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "audio_effects":
                            _effectsVolume = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        
                        // Camera settings
                        case "camera_fov":
                            _fov = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "camera_follow_y":
                            _followY = int.Parse(value);
                            break;
                        case "camera_follow_z":
                            _followZ = int.Parse(value);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Writer?.WriteLine($"Error parsing config line '{line}': {ex.Message}", "error");
                }
            }
            
            // Apply loaded camera settings immediately
            Medium.FocusPoint = GetFocusPoint(_fov);
            Medium.FollowYOffset = _followY;
            Medium.FollowZOffset = _followZ;
            
            Writer?.WriteLine($"Config loaded from {configPath}", "debug");
        }
        catch (Exception ex)
        {
            Writer?.WriteLine($"Error loading config: {ex.Message}", "error");
        }
    }
}
