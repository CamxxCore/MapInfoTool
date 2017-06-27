using System;
using GTA;
using GTA.Math;
using MapInfoTool.Memory;

namespace MapInfoTool.ScriptBase.Entity_Info
{
    public class PropObjectInfo : BaseObjectInfo
    {
        private readonly Prop _prop;

        private readonly string _modelName;

        public sealed override string ModelName => _modelName ?? "Unknown object.";

        public sealed override unsafe IntPtr MemoryAddress => new IntPtr(_prop.MemoryAddress);

        public override Vector3 Position
        {
            get
            {
                return _prop.Position;
            }

            set
            {
                _prop.Position = value;
            }
        }

        public override Vector3 Rotation => _prop.Rotation;

        public override Matrix Matrix => MemoryAccess.GetEntityMatrix(MemoryAddress);

        public override bool IsOnScreen => _prop.IsOnScreen;

        public PropObjectInfo(Prop obj)
        {
            _prop = obj;
            _modelName = MemoryAccess.GetEntityName(MemoryAddress);
            UiLabel.Color = System.Drawing.Color.FromArgb(190, UserConfig.ObjectColour);   
            UiLabel.SetText(0, "NAME: " + ModelName);
            UiLabel.SetText(1, "POSITION:  " + $"{_prop.Position.X}, {_prop.Position.Y}, {_prop.Position.Z}");
            UiLabel.SetText(2, "YMAP:  " + MemoryAccess.GetEntityMapDataName(MemoryAddress) + ".ymap");
            UiLabel.SetText(3, "Memory Address: 0x" + MemoryAddress.ToString("X"));
        }
    }
}
