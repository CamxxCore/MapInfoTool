using System;
using GTA;
using GTA.Math;
using GTA.Native;
using MapInfoTool.Interfaces;
using MapInfoTool.Math;

namespace MapInfoTool.Helpers
{
    public class FreeCamera : IUpdatable, IDisposable
    {
        public bool IsActive { get; private set; }

        public bool DisableControl { get; set; }

        private const float LerpTime = 0.8f;

        private const float RotationSpeed = 1.54f;

        private readonly Camera _mainCamera;

        public Vector3 Position => _mainCamera.Position;

        public Vector3 Rotation => _mainCamera.Rotation;

        public Vector3 Direction => MathHelper.RotationToDirection(_mainCamera.Rotation);

        public readonly InputHandler InputHandler;

        private float _currentLerpTime;

        private Vector3 _previousPos;

        private Vector3 _desiredPos;

        private Vector3 _lastFocus;

        private float _moveScale = 1.0f;

        public FreeCamera(Vector3 position, Vector3 rotation)
        {
            InputHandler = new InputHandler();
            InputHandler.LeftStickChanged += LeftStickChanged;
            InputHandler.RightStickChanged += RightStickChanged;
            InputHandler.LeftStickPressed += LeftStickPressed;
            _mainCamera = World.CreateCamera(position, rotation, 50f);
            _previousPos = position;
        }

        public FreeCamera() : this(Vector3.Zero, Vector3.Zero)
        { }  

        private void LeftStickChanged(object sender, AnalogStickChangedEventArgs e)
        {
            if (_mainCamera.IsInterpolating || !IsActive || DisableControl) return;

            if (e.X > sbyte.MaxValue)
                _previousPos -= Direction.RightVector(new Vector3(0, 0, 1f)) * (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 218) * -3f) * _moveScale;
            if (e.X < sbyte.MaxValue)
                _previousPos += -Direction.RightVector(new Vector3(0, 0, 1f)) * (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 218) * -3f) * _moveScale;
            if (e.Y != sbyte.MaxValue)
                _previousPos += Direction * Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, 8) * -5f * _moveScale;

            _currentLerpTime += 0.2f;

            if (_currentLerpTime > LerpTime)
                _currentLerpTime = LerpTime;

            _desiredPos = Vector3.Lerp(_mainCamera.Position, _previousPos, 0.2f);
        }

        public void PointAt(Vector3 lookAt)
        {
            _mainCamera.PointAt(lookAt);
        }

        private void RightStickChanged(object sender, AnalogStickChangedEventArgs e)
        {
            if (_mainCamera.IsInterpolating || !IsActive || DisableControl) return;

            _mainCamera.Rotation += new Vector3(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 221) * -10f, 0,
            Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 220) * -11f) * RotationSpeed;
        }

        private void LeftStickPressed(object sender, ButtonPressedEventArgs e)
        {
            if (_mainCamera.IsInterpolating || !IsActive || DisableControl) return;

            _previousPos += Direction * Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 230) * -5f;
        }

        public void EnterCameraView(Vector3 position)
        {
            _desiredPos = position;
            IsActive = true;
            World.RenderingCamera = _mainCamera;
            _mainCamera.Position = position;
        }

        public void ExitCameraView()
        {
            Function.Call(Hash.CLEAR_FOCUS);

            World.RenderingCamera = null;

            IsActive = false;
        }

        public void LookAt(Vector3 position)
        {
            _mainCamera.PointAt(position);
        }

        public void ClearLookAt()
        {
            _mainCamera.StopPointing();
        }

        public void Update(int gameTime)
        {
            if (IsActive)
            {
                DisableControlsOnFrame();

                _moveScale = Game.IsControlPressed(0, Control.CreatorMenuToggle) ? System.Math.Min(_moveScale + 0.1f, 10.0f) : 1.0f;

                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

                Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 18);

                Function.Call(Hash._SET_FOCUS_AREA,
                     Position.X, Position.Y, Position.Z,
                     _lastFocus.X, _lastFocus.Y, _lastFocus.Z);

                if (Position.DistanceTo(_lastFocus) > 50.0f)
                {
                    Function.Call(Hash._SET_FOCUS_AREA,
                        Position.X, Position.Y, Position.Z,
                        _lastFocus.X, _lastFocus.Y, _lastFocus.Z);

                    _lastFocus = Position;
                }

                _previousPos = Position;

                _mainCamera.Position = MathHelper.SmoothStep(_mainCamera.Position, _desiredPos, 0.7f);

                InputHandler.Update();
            }

            if (_currentLerpTime > 0)
                _currentLerpTime -= 0.01f;
        }

        private static void DisableControlsOnFrame()
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.VehicleCinCam, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MultiplayerInfo, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MeleeAttackLight, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MeleeAttackAlternate, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MeleeAttack2, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.Phone, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.VehicleLookBehind, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendRs, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendLs, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendX, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayShowhotkey, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayTools, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ScriptPadDown, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.FrontendDown, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.PhoneDown, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.HUDSpecial, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.SniperZoomOutSecondary, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.CharacterWheel, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayNewmarker, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayStartStopRecording, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayStartStopRecordingSecondary, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.ReplayPause, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveUpDown, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveLeftRight, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveLeftOnly, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveRightOnly, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveUpOnly, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, (int)Control.MoveDownOnly, true);
        }

        public void Dispose()
        {
            _mainCamera?.Destroy();
        }
    }
}
