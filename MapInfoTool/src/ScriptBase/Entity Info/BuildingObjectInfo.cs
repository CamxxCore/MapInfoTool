using System;
using GTA.Math;
using MapInfoTool.MathStuff;
using MapInfoTool.Memory;

namespace MapInfoTool.ScriptBase.Entity_Info
{
    public class BuildingObjectInfo : BaseObjectInfo
    {
        private readonly CBuildingWrapped _building;

        public override string ModelName => _building.ModelName ?? "Unknown building.";

        public override Vector3 Position
        {
            get
            {
                return MemoryAccess.GetEntityPosition(_building.Address);
            }

            set
            {
                MemoryAccess.SetEntityPosition(_building.Address, value);
            }
        }

        public override Matrix Matrix => _building.Matrix;

        public override Vector3 Rotation => Matrix.ToEuler();

        public sealed override IntPtr MemoryAddress => _building.Address;

        public BuildingObjectInfo(CBuildingWrapped cbuilding)
        {
            _building = cbuilding;
            UiLabel.Color = System.Drawing.Color.FromArgb(190, UserConfig.BuildingColour);
            UiLabel.SetText(0, "NAME: " + (_building.ModelName ?? "Unknown"));
            UiLabel.SetText(1, "POSITION:  " + $"{_building.Position.X}, {_building.Position.Y}, {_building.Position.Z}");
            UiLabel.SetText(2, "YMAP:  " + MemoryAccess.GetEntityMapDataName(_building.Address) + ".ymap");
            UiLabel.SetText(3, "Memory Address: 0x" + MemoryAddress.ToString("X"));
        }
    }
}
