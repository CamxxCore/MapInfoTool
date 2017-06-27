using System;
using GTA;
using GTA.Math;
using MapInfoTool.MathStuff;

namespace MapInfoTool.Interfaces
{
    /// <summary>
    /// Generic interface for a game object.
    /// </summary>
    internal interface IGameObject
    {       
        /// <summary>
        /// The world position of the object.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// The name of the entity model.
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// If the entity exists inside the current view frustum.
        /// </summary>
        bool IsOnScreen { get; }

        /// <summary>
        /// Instance of the entity model.
        /// </summary>
        Model Model { get; }  

        /// <summary>
        /// Instance of the entity (axis-aligned) bounding box.
        /// </summary>
        BoundingBox BoundingBox { get; }

        /// <summary>
        /// Pointer to the object in memory.
        /// </summary>
        IntPtr MemoryAddress { get; }
    }
}
