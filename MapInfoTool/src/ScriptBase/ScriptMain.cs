using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using MapInfoTool.Helpers;
using MapInfoTool.Memory;
using MapInfoTool.ScriptBase.Entity_Info;
using Control = GTA.Control;

namespace MapInfoTool.ScriptBase
{
    public class ScriptMain : Script
    {
        private readonly Ped _player = Game.Player.Character;

        private readonly FreeCamera _camera = new FreeCamera();

        private readonly EntityEditor _editor = new EntityEditor();

        private readonly BaseObjectInfo[] _visibleObjects = new BaseObjectInfo[100];

        private BaseObjectInfo _targetObject;

        private readonly FastUiArray _uiList = new FastUiArray("Nearby");

        private readonly Dictionary<string, Func<string[], string>> _commands =
            new Dictionary<string, Func<string[], string>>();

        private Vector3 _cameraEnterPos, _cameraLastPos;

        private readonly FrontendInput _input = new FrontendInput();

        private readonly FrontendOutput _output = new FrontendOutput();

        private bool _consoleActive;

        private readonly bool _capsLock = false;

        public ScriptMain()
        {
            AddCommands();
            MemoryAccess.MainInit();
            KeyDown += KeyPressed;
            Tick += OnTick;
            UserConfig.LoadValues();
        }

        private void AddCommands()
        {
            _commands.Add("showtextures", delegate
            {
                if (_targetObject == null) return "Error: an object must first be selected.";

                var index = 0;

                foreach (var tx in MemoryAccess.GetEntityTextureNames(_targetObject.MemoryAddress))
                {
                    _output.AddLine($"Texture {index + 1}: {tx}");

                    index++;
                }

                return index <= 0 ? "No nested textures found!" : string.Empty;
            });

            _commands.Add("reloadconfig", delegate
            {
                UserConfig.LoadValues();

                return "success";
            });

            _commands.Add("?", delegate
            {
                _output.AddLine("---------- Available commands --------------");

                _output.AddLine("reloadconfig -> Reloads values in the config file.");

                _output.AddLine("showtextures -> Show texture names for the selected object.");

                return string.Empty;
            });

            _commands.Add("help", delegate
            {
                _output.AddLine("---------- Available commands --------------");

                _output.AddLine("reloadconfig -> Reloads values in the config file.");

                _output.AddLine("showtextures -> Show texture names for the selected object.");

                return string.Empty;
            });
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (_consoleActive)
            {
                GetConsoleInput(e);
            }

            else if (_camera.IsActive)
            {
                if (e.KeyCode != Keys.T) return;

                _consoleActive = !_consoleActive;

                _input.Show();

                _output.Show();

                _output.DisableFadeOut();
            }

            else if (e.KeyCode == UserConfig.ActivationKey)
            {
                _camera.EnterCameraView(_cameraEnterPos != Vector3.Zero ? _cameraEnterPos : _player.Position);
            }
        }

        /// <summary>
        /// Get keyboard input for the console.
        /// </summary>
        /// <param name="e"></param>
        private void GetConsoleInput(KeyEventArgs e)
        {
            var key = System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)e.KeyCode);

            var keyChar = KeyInterop.GetCharFromKey(key, e.Shift);

            if ((e.Modifiers & Keys.Shift) != 0)
            {
                switch (keyChar)
                {
                    case ',': keyChar = '<'; break;
                    case '.': keyChar = '>'; break;
                    case '/': keyChar = '?'; break;
                    case ';': keyChar = ':'; break;
                    case '\'': keyChar = '"'; break;
                    case '\\': keyChar = '|'; break;
                    case '[': keyChar = '{'; break;
                    case ']': keyChar = '}'; break;
                    case '1': keyChar = '!'; break;
                    case '2': keyChar = '@'; break;
                    case '3': keyChar = '#'; break;
                    case '4': keyChar = '$'; break;
                    case '5': keyChar = '%'; break;
                    case '6': keyChar = '^'; break;
                    case '7': keyChar = '&'; break;
                    case '8': keyChar = '*'; break;
                    case '9': keyChar = '('; break;
                    case '0': keyChar = ')'; break;
                    case '-': keyChar = '_'; break;
                    case '=': keyChar = '+'; break;
                    case '`': keyChar = '~'; break;
                    default: keyChar = char.ToUpper(keyChar); break;
                }
            }

            else if (char.IsLetterOrDigit(keyChar))
            {
                if (_capsLock)
                    keyChar = char.ToUpper(keyChar);
            }

            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        _input.AddChar(' ');
                        return;

                    case Keys.Back:
                        if (_input.GetText().Length < 1)
                        {
                            _input.Hide();
                            _output.Hide();
                            _output.EnableFadeOut();
                            _consoleActive = false;
                        }

                        _input.RemoveLastChar();

                        return;

                    case Keys.Enter:
                        var text = _input.GetText();

                        _output.AddLine(text);

                        _output.EnableFadeOut();

                        _input.Hide();

                        _input.Clear();

                        _consoleActive = false;

                        ExecuteCommandString(text);

                        return;
                    default:  return;             
                }
            }

            if (keyChar != ' ')
            {
                _input.AddChar(keyChar);
            }
        }   

        /// <summary>
        /// Execute a console command by its string alias defined in this.commands
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal bool ExecuteCommandString(string cmd)
        {
            if (cmd.Length <= 0) return false;

            var stringArray = Regex.Split(cmd,
                "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            var command = stringArray[0].ToLower();

            Func<string[], string> func;

            if (!_commands.TryGetValue(command, out func)) return false;

            var args = stringArray.Skip(1).ToArray();

            var text = func?.Invoke(args);

            if (text?.Length > 0)
            {
                _output.AddLine(text);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateInput()
        {
            if (_editor.Active)
            {
                if (!Game.IsControlJustPressed(0, Control.ScriptRRight)) return;

                _editor.Exit();

                _camera.DisableControl = false;
            }

            else if (_camera.IsActive)
            {
                if (Game.IsControlJustPressed(0, Control.Attack))
                {
                    if (_targetObject == null) return;

                    _editor.Begin(_targetObject);

                    _camera.DisableControl = true;
                }

                else if (Game.IsControlJustPressed(0, Control.ScriptRRight))
                {
                    GameHelper.FadeScreenOut(1100);

                    Wait(1100);

                    if (_consoleActive)
                    {
                        _input.Hide();

                        _consoleActive = false;
                    }

                    _camera.ClearLookAt();
                    _camera.ExitCameraView();
                    _cameraEnterPos = _camera.Position;

                    Wait(100);

                    GameHelper.FadeScreenIn(800);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateVisibleObjects()
        {
            var closest = float.MaxValue;

            _targetObject = null;

            for (int i = 0; i < 100; i++)
            {
                if (_visibleObjects[i] == null || !_visibleObjects[i].IsOnScreen) continue;

                var cullEnt = false;

                for (int v = i - 1; v > -1; v--)
                {
                    if (_visibleObjects[v]?.Position.DistanceTo(_visibleObjects[i].Position) < 1.0f)
                    {
                        cullEnt = true;
                        break;
                    }
                }

                if (cullEnt)
                {
                    continue;
                }

                var position = _visibleObjects[i].MidPoint;

                if (UserConfig.DrawAllObjects)
                {
                    _visibleObjects[i].Draw(_camera.Position.DistanceTo(position));
                }

                var heading = Vector3.Normalize(position - _camera.Position);

                var dot = System.Math.Abs(Vector3.Dot(heading, _camera.Direction) - 2f);

                if (dot > closest) continue;

                closest = dot;
                _targetObject = _visibleObjects[i];
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            var gameTime = Game.GameTime;

            if (!_camera.IsActive) return;

            Function.Call(Hash.SET_PED_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

            Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

            Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

            Function.Call((Hash)0x90B6DA738A9A25DA, 0.0f);

            if (_camera.Position != _cameraLastPos)
            {
                _uiList.Clear();

                Array.Clear(_visibleObjects, 0, _visibleObjects.Length);

                if (UserConfig.ShowBuildings)
                {
                    CollectNearbyBuildings();
                }

                if (UserConfig.ShowObjects)
                {
                    CollectNearbyProps();
                }

                _cameraLastPos = _camera.Position;
            }

            if (!_editor.Active)
            {
                UpdateVisibleObjects();

                _uiList.Draw();
            }

            else
            {
                _editor.Update(gameTime);
            }

            if (_targetObject != null)
            {
                if (!UserConfig.DrawAllObjects)
                {
                    _targetObject.Draw(_camera.Position.DistanceTo(_targetObject.Position));
                }

                var bb = _targetObject.BoundingBox;

                GameHelper.DrawBox(_targetObject.Position, bb.Min, bb.Max, Color.Red);

                UI.ShowSubtitle(_targetObject.ModelName, 1);
            }

            _camera.Update(gameTime);

            _input.Update(gameTime);

            _output.Update(gameTime);

            UpdateInput();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectNearbyProps()
        {
            var props = World.GetNearbyProps(_camera.Position, UserConfig.ObjectSearchRadius).ToArray();

            for (int i = 0; i < System.Math.Min(props.Length, System.Math.Min(50, UserConfig.MaxObjectsOnScreen)); i++)
            {
                _visibleObjects[i + 50] = new PropObjectInfo(props[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CollectNearbyBuildings()
        {
            var count = 0;
            foreach (var building in MemoryAccess.GetCBuildings(_camera.Position, UserConfig.BuildingSearchRadius).OrderBy(x => x.Position.DistanceTo(_camera.Position)))
            {
                if (count > System.Math.Min(49, UserConfig.MaxBuildingsOnScreen)) break;

                _visibleObjects[count++] = new BuildingObjectInfo(building);

                if (UserConfig.ShowNearbyList && count < FastUiArray.NumTextLines)
                {
                    _uiList.SetText(count, building.ModelName, 
                        string.Format("{0:0.###}", building.Position.DistanceTo(_camera.Position)));
                }
            }
        }

        protected override void Dispose(bool A_0)
        {
            if (_camera.IsActive)
                _camera.ExitCameraView();
            _camera.Dispose();
            base.Dispose(A_0);
        }
    }
}
