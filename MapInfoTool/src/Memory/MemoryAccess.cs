using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using GTA.Math;

namespace MapInfoTool.Memory
{
    public class CBuildingWrapped
    {
        public IntPtr Address { get; set; }

        public int ModelHash { get; set; }

        public string ModelName { get; set; }

        public Vector3 Position { get; set; }

        public CMatrix Matrix { get; set; }
    }

    public class MemoryAccess
    {
        private static IntPtr _cBuildingPoolPtr, _mapDataStorePtr, _getConstStringForHashPtr;

        private static GetConstStringForHashFunc _getStringForHash;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr GetEntityObbFunc(IntPtr entity, ref CEntityBounds bounds);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetConstStringForHashFunc(int unk, uint hashKey);

        private const int CBuildingClassSize = 0xD0;

        public static void MainInit()
        {
            #region SetupCBuildingPool

            var pattern = new Pattern("\x48\x8B\x05\x00\x00\x00\x00\x8B\x70\x10", "xxx????xxx");

            var result = pattern.Get();

            if (result != IntPtr.Zero)
            {
                var rip = result.ToInt64() + 7;
                var value = Marshal.ReadInt32(IntPtr.Add(result, 3));
                _cBuildingPoolPtr = new IntPtr(rip + value);
            }

            #endregion

            #region SetupMapDataStore

            pattern = new Pattern("\x0F\xB7\x04\x79\x66\x83\xE0\x3F", "xxxxxxxx");

            result = pattern.Get(0x13);

            if (result != IntPtr.Zero)
            {
                var rip = result.ToInt64() + 7;
                var value = Marshal.ReadInt32(IntPtr.Add(result, 3));
                _mapDataStorePtr = new IntPtr(rip + value);
            }

            #endregion

            #region SetupGetConstString

            pattern = new Pattern("\x33\xD2\x41\x0F\x28\xC2", "xxxxxx");

            result = pattern.Get(0x35);

            if (result != IntPtr.Zero)
            {
                var rip = result.ToInt64() + 5;
                var value = Marshal.ReadInt32(IntPtr.Add(result, 1));
                _getConstStringForHashPtr = new IntPtr(rip + value);
            }

            #endregion

            _getStringForHash = Marshal.GetDelegateForFunctionPointer<GetConstStringForHashFunc>(_getConstStringForHashPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetEntityPosition(IntPtr entity)
        {
            return (Vector3)Marshal.PtrToStructure(entity + 0x90, typeof(Vector3));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CMatrix GetEntityMatrix(IntPtr entity)
        {
            return (CMatrix)Marshal.PtrToStructure(entity + 0x60, typeof(CMatrix));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEntityPosition(IntPtr entity, Vector3 position)
        {
            Marshal.StructureToPtr(position, entity + 0x90, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEntityMatrix(IntPtr entity, CMatrix matrix)
        {
            Marshal.StructureToPtr(matrix, entity + 0x60, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEntityEulerRotation(IntPtr entity, Quaternion rotation)
        {
            var origMatrix = GetEntityMatrix(entity);

            var matrix = Matrix.RotationQuaternion(rotation);

            matrix.M41 = origMatrix.M41;
            matrix.M42 = origMatrix.M42;
            matrix.M43 = origMatrix.M43;

            SetEntityMatrix(entity, matrix);
        }

        public static void GetEntityObb(IntPtr entity, out Vector3 min, out Vector3 max)
        {
            var vfTable = Marshal.ReadIntPtr(entity);

            var fnAddress = Marshal.ReadIntPtr(vfTable + 0x1B8);

            var fn = Marshal.GetDelegateForFunctionPointer<GetEntityObbFunc>(fnAddress);

            var bounds = new CEntityBounds();

            fn(entity, ref bounds);

            min = bounds.Min;

            max = bounds.Max;
        }

        public static string GetEntityMapDataName(IntPtr entity)
        {
            var psoFileIndex = Marshal.ReadInt32(entity + 0xC8) >> 8 & 0xFFFF;

            var itemSize = Marshal.ReadInt32(_mapDataStorePtr + 0x4C);

            var assetItemPtr = Marshal.ReadIntPtr(_mapDataStorePtr + 0x38) + psoFileIndex * itemSize;

            var fileHash = (uint)Marshal.ReadInt32(assetItemPtr + 0xC);

            var result = _getStringForHash(1, fileHash);

            return Marshal.PtrToStringAnsi(result);
        }

        public static IEnumerable<string> GetEntityTextureNames(IntPtr entity)
        {
            var ptr = Marshal.ReadIntPtr(entity + 0x48); //drawHandler

            if (ptr == IntPtr.Zero) yield break;

            ptr = Marshal.ReadIntPtr(ptr + 0x8); //drawable instance

            if (ptr == IntPtr.Zero) yield break;

            ptr = Marshal.ReadIntPtr(ptr + 0x10); //shader group

            ptr = Marshal.ReadIntPtr(ptr + 0x8); //texture dictionary          

            var itemStart = Marshal.ReadIntPtr(ptr + 0x30); //items

            int numItems = Marshal.ReadInt16(ptr + 0x38);

            for (int i = 0; i < numItems; i++)
            {
                ptr = Marshal.ReadIntPtr(itemStart + i * 8);

                yield return Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(ptr + 0x28));
            }
        }

        public static string GetEntityName(IntPtr address)
        {
            // probably a better way to get this...
            if ((Marshal.ReadByte(address + 0xC0) & 0x40) != 0)
            {
                var fragInst = Marshal.ReadIntPtr(address + 0x30);

                if (fragInst != IntPtr.Zero)
                {
                    var fragType = Marshal.ReadIntPtr(fragInst + 0x78);

                    if (fragType != IntPtr.Zero)
                    {
                        var str = Marshal.ReadIntPtr(fragType + 0x58);

                        if (str != IntPtr.Zero)
                        {
                            var result = Marshal.PtrToStringAnsi(str);

                            return result?.Substring(result.IndexOf('/') + 1) ?? "";
                        }
                    }
                }
            }

            var drawHandler = Marshal.ReadIntPtr(address + 0x48);

            if (drawHandler == IntPtr.Zero) return null;
            {
                var gtaDrawable = Marshal.ReadIntPtr(drawHandler + 0x8);

                if (gtaDrawable == IntPtr.Zero) return null;

                var result = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(gtaDrawable + 0xA8));

                if (result != null)
                    return result.Substring(0, result.Length - 4);
            }

            return null;
        }

        public static IEnumerable<CBuildingWrapped> GetCBuildings()
        {
            return GetCBuildings(false);
        }

        public static IEnumerable<CBuildingWrapped> GetCBuildings(Vector3 center, float range)
        {
            return GetCBuildings(center, range, false);
        }

        public static IEnumerable<CBuildingWrapped> GetCBuildings(bool getNonDrawables)
        {
            var baseAddr = Marshal.ReadIntPtr(_cBuildingPoolPtr);

            var itemStart = Marshal.ReadIntPtr(baseAddr);

            var count = Marshal.ReadInt32(baseAddr + 0x10);

            for (int i = 0; i < count - 1; i++)
            {
                var currentAddr = itemStart + i * CBuildingClassSize;

                var drawHandler = Marshal.ReadIntPtr(currentAddr + 0x48);

                if (drawHandler == IntPtr.Zero && !getNonDrawables) continue;

                var info = new CBuildingWrapped
                {
                    Address = currentAddr,
                    Position = (Vector3) Marshal.PtrToStructure(currentAddr + 0x90, typeof(Vector3)),
                    Matrix = (CMatrix) Marshal.PtrToStructure(currentAddr + 0x60, typeof(CMatrix)),
                    ModelName = GetEntityName(currentAddr)
                };

                var modelInfo = Marshal.ReadIntPtr(currentAddr + 0x20);

                if (modelInfo != IntPtr.Zero)
                    info.ModelHash = Marshal.ReadInt32(modelInfo + 0x18);

                yield return info;
            }
        }

        public static IEnumerable<CBuildingWrapped> GetCBuildings(Vector3 center, float radius, bool getNonDrawables)
        {
            var baseAddr = Marshal.ReadIntPtr(_cBuildingPoolPtr);

            var itemStart = Marshal.ReadIntPtr(baseAddr);

            var count = Marshal.ReadInt32(baseAddr + 0x10);

            for (int i = 0; i < count; i++)
            {
                var currentAddr = itemStart + i * CBuildingClassSize;

                var drawHandler = Marshal.ReadIntPtr(currentAddr + 0x48);

                if (drawHandler == IntPtr.Zero && !getNonDrawables) continue;

                var info = new CBuildingWrapped();

                var matrix = (CMatrix)Marshal.PtrToStructure(currentAddr + 0x60, typeof(CMatrix));

                var position = (Vector3)Marshal.PtrToStructure(currentAddr + 0x90, typeof(Vector3));

                if (center.DistanceToSquared(position) > radius * radius) continue;

                info.Address = currentAddr;

                info.Matrix = matrix;

                info.Position = position;

                info.ModelName = GetEntityName(currentAddr);

                var modelInfo = Marshal.ReadIntPtr(currentAddr + 0x20);

                if (modelInfo != IntPtr.Zero)
                {
                    info.ModelHash = Marshal.ReadInt32(modelInfo + 0x18);
                }

                yield return info;
            }
        }
    }
}
