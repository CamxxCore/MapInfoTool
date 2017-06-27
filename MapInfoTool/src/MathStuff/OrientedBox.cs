using System;
using GTA.Math;

namespace MapInfoTool.MathStuff
{
    /// <summary>
    /// Bounding volume using an oriented bounding box.
    /// </summary>
    public struct OrientedBox : IEquatable<OrientedBox>
    {
        public const int CornerCount = 8;

        public readonly Vector3 Center;
        public readonly Vector3 HalfExtent;
        public readonly Quaternion Orientation;

        /// <summary>
        /// Create an oriented box with the given center, half-extents, and orientation.
        /// </summary>
        public OrientedBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
        {
            Center = center;
            HalfExtent = halfExtents;
            Orientation = orientation;
        }

        /// <summary>
        /// Create an oriented box from an axis-aligned box.
        /// </summary>
        public static OrientedBox CreateFromBoundingBox(BoundingBox box)
        {
            Vector3 mid = (box.Min + box.Max) * 0.5f;
            Vector3 halfExtent = (box.Max - box.Min) * 0.5f;
            return new OrientedBox(mid, halfExtent, Quaternion.Identity);
        }

        /// <summary>
        /// Transform the given bounding box by a rotation around the origin followed by a translation 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="translation"></param>
        /// <returns>A new bounding box, transformed relative to this one</returns>
        public OrientedBox Transform(Quaternion rotation, Vector3 translation)
        {
            return new OrientedBox(MathHelper.Transform(Center, rotation) + translation,
                                            HalfExtent,
                                            Orientation * rotation);
        }

        /// <summary>
        /// Transform the given bounding box by a uniform scale and rotation around the origin followed
        /// by a translation
        /// </summary>
        /// <returns>A new bounding box, transformed relative to this one</returns>
        public OrientedBox Transform(float scale, Quaternion rotation, Vector3 translation)
        {
            return new OrientedBox(MathHelper.Transform(Center * scale, rotation) + translation,
                                            HalfExtent * scale,
                                            Orientation * rotation);
        }

        public bool Equals(OrientedBox other)
        {
            return (Center == other.Center && HalfExtent == other.HalfExtent && Orientation == other.Orientation);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is OrientedBox)) return false;

            var other = (OrientedBox) obj;

            return (Center == other.Center && 
                HalfExtent == other.HalfExtent && 
                Orientation == other.Orientation);
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ HalfExtent.GetHashCode() ^ Orientation.GetHashCode();
        }

        public static bool operator ==(OrientedBox a, OrientedBox b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(OrientedBox a, OrientedBox b)
        {
            return !Equals(a, b);
        }

        public override string ToString()
        {
            return "{Center:" + Center +
                   " Extents:" + HalfExtent +
                   " Orientation:" + Orientation + "}";
        }

        /// <summary>
        /// Returns true if this box contains the given point.
        /// </summary>
        public bool Contains(ref Vector3 point)
        {
            // Transform the point into box-local space and check against
            // our extents.
            Quaternion qinv = -Orientation;

            Vector3 plocal = MathHelper.Transform(point - Center, qinv);

            return System.Math.Abs(plocal.X) <= HalfExtent.X &&
                   System.Math.Abs(plocal.Y) <= HalfExtent.Y &&
                   System.Math.Abs(plocal.Z) <= HalfExtent.Z;
        }

        /// <summary>
        /// Return the 8 corner positions of this bounding box.
        ///
        ///     ZMax    ZMin
        ///    0----1  4----5
        ///    |    |  |    |
        ///    |    |  |    |
        ///    3----2  7----6
        ///
        /// The ordering of indices is a little strange to match what BoundingBox.GetCorners() does.
        /// </summary>
        public Vector3[] GetCorners()
        {
            var corners = new Vector3[CornerCount];
            GetCorners(corners, 0);
            return corners;
        }

        /// <summary>
        /// Return the 8 corner positions of this bounding box.
        ///
        ///     ZMax    ZMin
        ///    0----1  4----5
        ///    |    |  |    |
        ///    |    |  |    |
        ///    3----2  7----6
        ///
        /// The ordering of indices is a little strange to match what BoundingBox.GetCorners() does.
        /// </summary>
        /// <param name="corners">Array to fill with the eight corner positions</param>
        /// <param name="startIndex">Index within corners array to start writing positions</param>
        public void GetCorners(Vector3[] corners, int startIndex)
        {
            var m = Matrix.RotationQuaternion(Orientation);

            var left = new Vector3(-m.M11, -m.M12, -m.M13);
            var back = new Vector3(m.M31, m.M32, m.M33);
            var up = new Vector3(m.M21, m.M22, m.M23);

            var hX = left * HalfExtent.X;
            var hY = up * HalfExtent.Y;
            var hZ = back * HalfExtent.Z;

            var i = startIndex;
            corners[i++] = Center - hX + hY + hZ;
            corners[i++] = Center + hX + hY + hZ;
            corners[i++] = Center + hX - hY + hZ;
            corners[i++] = Center - hX - hY + hZ;
            corners[i++] = Center - hX + hY - hZ;
            corners[i++] = Center + hX + hY - hZ;
            corners[i++] = Center + hX - hY - hZ;
            corners[i++] = Center - hX - hY - hZ;
        }
    }
}
