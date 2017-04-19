using GTA;
using GTA.Math;
using GTA.Native;
using MapInfoTool.Interfaces;
using MapInfoTool.Math;
using MapInfoTool.ScriptBase.Entity_Info;

namespace MapInfoTool.ScriptBase
{
    public class EntityEditor : IUpdatable
    {
        private const float MaxMoveSpeed = 20.0f;

        private const float LerpTime = 0.8f;

        private float _moveScale = 10.0f;

        private float _currentLerpTime;

        private Vector3 _previousPos;

        public bool Active { get; private set; }

        public BaseObjectInfo CurrentEntity { get; private set; }

        private readonly InputHandler _inputHandler = new InputHandler();

        public EntityEditor()
        {
            _inputHandler.LeftStickChanged += InputHandler_LeftStickChanged;
        }

        public void Begin(BaseObjectInfo entity)
        {
            CurrentEntity = entity;
            Active = true;
        }

        private void InputHandler_LeftStickChanged(object sender, AnalogStickChangedEventArgs e)
        {
            var forward = MathHelper.RotationToDirection(CurrentEntity.Rotation);

            if (e.X > sbyte.MaxValue)
                _previousPos -= (forward.RightVector(new Vector3(0, 0, 1f)) * (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 218) * -4f) * _moveScale);
            if (e.X < sbyte.MaxValue)
                _previousPos += -forward.RightVector(new Vector3(0, 0, 1f)) * (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 2, 218) * -4f * _moveScale);
            if (e.Y != sbyte.MaxValue)
                _previousPos += forward * (Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, 8) * -5f * _moveScale);

            _currentLerpTime += 0.001f;

            if (_currentLerpTime > LerpTime)
                _currentLerpTime = LerpTime;

            CurrentEntity.Position = 
                Vector3.Lerp(CurrentEntity.Position, _previousPos, _currentLerpTime / LerpTime);
        }

        public void Update(int gameTime)
        {
            if (Active && CurrentEntity != null)
            {
                _inputHandler.Update();

                if (Game.IsControlJustPressed(0, Control.SelectNextWeapon))
                {
                    _moveScale = (_moveScale - 1f).Clamped(0, MaxMoveSpeed);

                    UI.ShowSubtitle($"Movement Speed: {_moveScale} ");
                }

                else if (Game.IsControlJustPressed(0, Control.SelectPrevWeapon))
                {
                    _moveScale = (_moveScale + 1f).Clamped(0, MaxMoveSpeed);

                    UI.ShowSubtitle($"Movement Speed: {_moveScale} ");
                }

                if (_currentLerpTime > 0)
                {
                    _currentLerpTime -= 0.001f;
                }

                _previousPos = CurrentEntity.Position;
            }
        }

        public void Exit()
        {
            CurrentEntity = null;
            Active = false;
        }
    }
}
