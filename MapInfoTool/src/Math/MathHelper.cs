using System;
using GTA.Math;

namespace MapInfoTool.Math
{
    public static class MathHelper
    {
        public static double ToRadians(this float val)
        {
            return System.Math.PI / 180 * val;
        }

        public static T Clamped<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }

        /// <summary>
        /// Transforms a 3D vector by the given matrix.
        /// </summary>
        /// <param name="position">The source vector.</param><param name="matrix">The transformation matrix.</param>
        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            var num1 = (float)(position.X * (double)matrix.M11 + position.Y * (double)matrix.M21 + position.Z * (double)matrix.M31) + matrix.M41;
            var num2 = (float)(position.X * (double)matrix.M12 + position.Y * (double)matrix.M22 + position.Z * (double)matrix.M32) + matrix.M42;
            var num3 = (float)(position.X * (double)matrix.M13 + position.Y * (double)matrix.M23 + position.Z * (double)matrix.M33) + matrix.M43;
            var num4 = 1f / ((((position.X * matrix.M14) + (position.Y * matrix.M24)) + (position.Z * matrix.M34)) + matrix.M44);
            Vector3 vector3;
            vector3.X = num1 * num4;
            vector3.Y = num2 * num4;
            vector3.Z = num3 * num4;
            return vector3;
        }

        /// <summary>
        /// Transforms a Vector3 by a specified Quaternion rotation.
        /// </summary>
        /// <param name="value">The Vector3 to rotate.</param><param name="rotation">The Quaternion rotation to apply.</param>
        public static Vector3 Transform(Vector3 value, Quaternion rotation)
        {
            var num1 = rotation.X + rotation.X;
            var num2 = rotation.Y + rotation.Y;
            var num3 = rotation.Z + rotation.Z;
            var num4 = rotation.W * num1;
            var num5 = rotation.W * num2;
            var num6 = rotation.W * num3;
            var num7 = rotation.X * num1;
            var num8 = rotation.X * num2;
            var num9 = rotation.X * num3;
            var num10 = rotation.Y * num2;
            var num11 = rotation.Y * num3;
            var num12 = rotation.Z * num3;
            var num13 = (float) (value.X * (1.0 - num10 - num12) + value.Y * (num8 - (double) num6) +
                                 value.Z * (num9 + (double) num5));
            var num14 = (float) (value.X * (num8 + (double) num6) + value.Y * (1.0 - num7 - num12) +
                                 value.Z * (num11 - (double) num4));
            var num15 = (float) (value.X * (num9 - (double) num5) + value.Y * (num11 + (double) num4) +
                                 value.Z * (1.0 - num7 - num10));
            Vector3 vector3;
            vector3.X = num13;
            vector3.Y = num14;
            vector3.Z = num15;
            return vector3;
        }

        public static Quaternion QuaternionFromMatrix(Matrix m)
        {
            var q = new Quaternion
            {
                W = (float) System.Math.Sqrt(System.Math.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2,
                X = (float) System.Math.Sqrt(System.Math.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2,
                Y = (float) System.Math.Sqrt(System.Math.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2,
                Z = (float) System.Math.Sqrt(System.Math.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2
            };

            q.X *= System.Math.Sign(q.X * (m[2, 1] - m[1, 2]));
            q.Y *= System.Math.Sign(q.Y * (m[0, 2] - m[2, 0]));
            q.Z *= System.Math.Sign(q.Z * (m[1, 0] - m[0, 1]));

            return q;
        }

        public static Vector3 RightVector(this Vector3 position, Vector3 up)
        {
            position.Normalize();
            up.Normalize();
            return Vector3.Cross(position, up);
        }

        public static void ClampMagnitude(ref Vector3 vector, float maxLength)
        {
            if (vector.LengthSquared() > maxLength * maxLength)
            {
                vector = Vector3.Normalize(vector) * maxLength;
            }
        }

        public static Vector3 SmoothStep(Vector3 start, Vector3 end, float amount)
        {
            Vector3 vector;

            amount = (amount > 1.0f) ? 1.0f : ((amount < 0.0f) ? 0.0f : amount);
            amount = (amount * amount) * (3.0f - (2.0f * amount));

            vector.X = start.X + ((end.X - start.X) * amount);
            vector.Y = start.Y + ((end.Y - start.Y) * amount);
            vector.Z = start.Z + ((end.Z - start.Z) * amount);

            return vector;
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = System.Math.Max(0.0001f, smoothTime);
            var num = 2f / smoothTime;
            var num2 = num * deltaTime;
            var d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            var vector = current - target;
            var vector2 = target;
            var maxLength = maxSpeed * smoothTime;
            ClampMagnitude(ref vector, maxLength);
            target = current - vector;
            var vector3 = (currentVelocity + num * vector) * deltaTime;
            currentVelocity = (currentVelocity - num * vector3) * d;
            var vector4 = target + (vector + vector3) * d;
            if (Vector3.Dot(vector2 - current, vector4 - vector2) > 0f)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            var rotZ = rotation.Z.ToRadians();
            var rotX = rotation.X.ToRadians();
            var multiXy = System.Math.Abs(Convert.ToDouble(System.Math.Cos(rotX)));
            var res = default(Vector3);
            res.X = (float) (Convert.ToDouble(-System.Math.Sin(rotZ)) * multiXy);
            res.Y = (float) (Convert.ToDouble(System.Math.Cos(rotZ)) * multiXy);
            res.Z = (float) Convert.ToDouble(System.Math.Sin(rotX));
            return res;
        }

        public static Vector3 ToEuler(this Matrix matrix)
        {
            float yaw = 0.0f, pitch = 0.0f, roll = 0.0f;

            if (System.Math.Abs(matrix.M11) != 1.0f)
            {
                yaw = (float)System.Math.Atan2(matrix.M13, matrix.M34);
                pitch = 0;
                roll = 0;
            }

            else
            {
                yaw = (float)System.Math.Atan2(-matrix.M31, matrix.M11);
                pitch = (float)System.Math.Asin(matrix.M21);
                roll = (float)System.Math.Atan2(-matrix.M23, matrix.M22);
            }

            return new Vector3(roll, pitch, yaw);
        }
    }
}
