using GTA.Math;

namespace MapInfoTool.Math
{
    public class BoundingBox
    {
        public Vector3 Min { get; set; }

        public Vector3 Max { get; set; }

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }
}
