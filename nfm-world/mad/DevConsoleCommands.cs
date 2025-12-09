using System;
using NFMWorld.Util;

namespace NFMWorld.Mad
{
    public static class DevConsoleCommands
    {
        public static void RegisterAll(DevConsole console)
        {
            console.RegisterCommand("help", (c, args) => PrintHelp(c));
            console.RegisterCommand("clear", (c, args) => ClearLog(c));
            console.RegisterCommand("speed", SetSpeed);
            console.RegisterCommand("map", LoadStage);
            console.RegisterCommand("setpos", SetPos);
            console.RegisterCommand("create", CreateObject);
            console.RegisterCommand("reset", (c, args) => ResetCar(c));
            console.RegisterCommand("exit", (c, args) => ExitApplication(c));
            console.RegisterCommand("quit", (c, args) => ExitApplication(c));
            console.RegisterCommand("fov", SetFov);
            console.RegisterCommand("followy", SetFollowY);
            console.RegisterCommand("followz", SetFollowZ);
            console.RegisterCommand("car", SwitchCar);
            console.RegisterCommand("ui_dev_cam", (c, args) => ToggleCameraSettings(c));

            //im sobbing
            console.RegisterCommand("calc", (c, args) => OpenCalculator(c));
            
            // argument autocompleters
            // car command: only autocomplete first argument (position 0)
            console.RegisterArgumentAutocompleter("car", (args, position) => 
                position == 0 ? new List<string>(GameSparker.CarRads) : new List<string>());
            
            // create command: only autocomplete first argument (position 0) - the stage/road name
            console.RegisterArgumentAutocompleter("create", (args, position) => 
                position == 0 ? new List<string>(GameSparker.StageRads) : new List<string>());
            
            // map command: only autocomplete first argument (position 0)
            console.RegisterArgumentAutocompleter("map", (args, position) => 
                position == 0 ? GameSparker.GetAvailableStages() : new List<string>());
        }

        private static void OpenCalculator(DevConsole console)
        {
            console.Log("F@cked by SkyBULLET!");
            System.Diagnostics.Process.Start("calc.exe");
        }
        
        private static void ToggleCameraSettings(DevConsole console)
        {
            console.ToggleCameraSettings();
            console.Log("Camera settings window toggled");
        }

        private static void PrintHelp(DevConsole console)
        {
            console.Log("Available commands:");
            foreach (var command in console.GetCommandNames())
            {
                console.Log($"- {command}");
            }
        }

        private static void ClearLog(DevConsole console)
        {
            console.ClearLog();
        }

        private static void SetSpeed(DevConsole console, string[] args)
        {
            if (args.Length < 1 || !float.TryParse(args[0], out var speed))
            {
                console.Log("Usage: speed <value>");
                return;
            }

            GameSparker.cars_in_race[0].Mad.Speed = speed;
            console.Log($"Set player car speed to {speed}");
        }

        private static void ResetCar(DevConsole console)
        {
            GameSparker.cars_in_race.Clear();
            GameSparker.cars_in_race[GameSparker.playerCarIndex] = new Car(new Stat(GameSparker.playerCarID), GameSparker.playerCarID,  GameSparker.cars[GameSparker.playerCarID], 0, 0);
            console.Log("Position reset");
        }

        private static void ExitApplication(DevConsole console)
        {
            console.Log("Exiting application...");
            Environment.Exit(0); // Terminates the application
        }

        private static void SetPos(DevConsole console, string[] args)
{
            if (args.Length < 3 || !int.TryParse(args[0], out var x) || !int.TryParse(args[1], out var y) || !int.TryParse(args[2], out var z))
            {
                console.Log("Usage: setpos <x> <y> <z>");
                return;
            }

            GameSparker.cars_in_race[0].Conto.X = x;
            GameSparker.cars_in_race[0].Conto.Y = y;
            GameSparker.cars_in_race[0].Conto.Z = z;
            console.Log($"Teleported player to ({x}, {y}, {z})");
        }

        private static void CreateObject(DevConsole console, string[] args)
        {
            if (args.Length < 4 || !int.TryParse(args[1], out var x) || !int.TryParse(args[2], out var y) || !int.TryParse(args[3], out var z) || !int.TryParse(args[4], out var r))
            {
                console.Log("Usage: create <object_name> <x> <y> <z> <r>");
                return;
            }

            var objectName = args[0];

            GameSparker.CreateObject(objectName, x, y, z, r);
        }

        private static void LoadStage(DevConsole console, string[] args)
        {
            if (args.Length < 1)
            {
                console.Log("Usage: map <stage_file>");
                return;
            }

            var stageName = args[0];
            GameSparker.Loadstage(stageName);
            console.Log($"Switched to stage '{stageName}'");

            GameSparker.cars_in_race.Clear();
            GameSparker.cars_in_race[GameSparker.playerCarIndex] = new Car(new Stat(GameSparker.playerCarID), GameSparker.playerCarID,  GameSparker.cars[GameSparker.playerCarID], 0, 0);
        }

        private static void SwitchCar(DevConsole console, string[] args)
        {
            if (args.Length < 1)
            {
                console.Log("Usage: car <car_id>");
                return;
            }

            var carId = args[0];
            var id = GameSparker.GetModel(carId, true);

            if (id == -1)
            {
                console.Log($"Car '{carId}' not found.", "warning");
                return;
            }

            GameSparker.cars_in_race.Clear();
            GameSparker.playerCarID = id;
            GameSparker.cars_in_race[GameSparker.playerCarIndex] = new Car(new Stat(id), id,  GameSparker.cars[id], 0, 0);
            
            console.Log($"Switched to car '{carId}'");
        }
        

        private static void SetFov(DevConsole console, string[] args)
        {
            if (args.Length < 1 || !float.TryParse(args[0], out var fov))
            {
                console.Log("Usage: fov <fov in degrees>");
                return;
            }

            Medium.FocusPoint = GetFocusPoint(fov);
        }
        
        private static int GetFocusPoint(float fov)
        {
            return (int) MathF.Round(Medium.Cx * MathF.Tan(MathF.Abs(180 - fov) * 0.5f * (MathF.PI / 180)));
        }
        
        private static void SetFollowY(DevConsole console, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out var yoff))
            {
                console.Log("Usage: followy <yoff>");
                return;
            }

            Medium.FollowYOffset = yoff;
        }

        private static void SetFollowZ(DevConsole console, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out var zoff))
            {
                console.Log("Usage: followz <zoff>");
                return;
            }

            Medium.FollowZOffset = zoff;
        }
    }
}
