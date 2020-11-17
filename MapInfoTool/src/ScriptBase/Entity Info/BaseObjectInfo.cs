using System;
using GTA;
using GTA.Math;
using MapInfoTool.Interfaces;
using MapInfoTool.MathStuff;

namespace MapInfoTool.ScriptBase.Entity_Info
{
    /// <summary>
    /// Represents an object/ entity that is drawing information on the screen.
    /// </summary>
    public abstract class BaseObjectInfo : IGameObject, IDrawable
    {
        /// <summary>
        /// Entity UI overlay.
        /// </summary>
        public EntityInfoLabel UiLabel { get; }

        /// <summary>
        /// Position of the entity.
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// Rotation of the entity.
        /// </summary>
        public abstract Vector3 Rotation { get; }

        /// <summary>
        /// Object rotation matrix.
        /// </summary>
        public abstract Matrix Matrix { get; }

        /// <summary>
        /// Memory address of the entity.
        /// </summary>
        public abstract IntPtr MemoryAddress { get; }

        /// <summary>
        /// The name of the entity model.
        /// </summary>
        public abstract string ModelName { get; }

        /// <summary>
        /// The entity model instance.
        /// </summary>
        public Model Model => new Model(ModelName);

        public Vector3 DrawOffset { get; set; }

        /// <summary>
        /// The world-relative midpoint of the entity model.
        /// </summary>
        public Vector3 MidPoint
        {
            get
            {
                Vector3 min, max;

                Model.GetDimensions(out min, out max);

                return Position + (min + max) * 0.5f;
            }
        }

        /// <summary>
        /// Axis- aligned bounding box of the entity.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                Vector3 min, max;

                Model.GetDimensions(out min, out max);

                return new BoundingBox(min, max);
            }
        }

        /// <summary>
        /// Whether then entity is on the screen.
        /// </summary>
        public virtual bool IsOnScreen
        {
            get
            {
                var screenPos = Utility.WorldToScreen(MidPoint);
                return screenPos.X != 0 && screenPos.Y != 0;
            }
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        protected BaseObjectInfo()
        {
            UiLabel = new EntityInfoLabel();
        }

        /// <summary>
        /// Draw the entity information.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="stackedIdx"></param>
        public virtual void Draw(float distance, int stackedIdx)
        {
            UiLabel.DrawAtPosition(MidPoint + DrawOffset, distance, stackedIdx);
        }

        /// <summary>
        /// Draw the entity information.
        /// </summary>
        /// <param name="distance"></param>
        public virtual void Draw(float distance)
        {
            Draw(distance, 0);
        }

        /// <summary>
        /// Draw the entity information.
        /// </summary>
        public virtual void Draw()
        {
            Draw(0.0f);
        }
    }
}
