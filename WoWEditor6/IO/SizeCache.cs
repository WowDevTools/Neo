using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.IO
{
    /// <summary>
    /// Provided by Apoc, see:
    /// https://gist.github.com/ApocDev/bd3947020ef89c868f6d
    /// </summary>
    public static class SizeCache<T> where T : struct
    {
        /// <summary> The size of the Type </summary>
        public static readonly int Size;

        /// <summary> The real, underlying type. </summary>
        public static readonly Type RealType;

        /// <summary> The type code </summary>
        public static TypeCode TypeCode;

        /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
        public static bool TypeRequiresMarshal;

        public static bool IsIntPtr;

        internal static readonly GetUnsafePtrDelegate GetUnsafePtr;

        static SizeCache()
        {
            TypeCode = Type.GetTypeCode(typeof(T));

            // Booleans = 1 char.
            if (typeof(T) == typeof(bool))
            {
                Size = 1;
                RealType = typeof(T);
            }
            else if (typeof(T).IsEnum)
            {
                Type underlying = typeof(T).GetEnumUnderlyingType();
                Size = Marshal.SizeOf(underlying);
                RealType = underlying;
                TypeCode = Type.GetTypeCode(underlying);
            }
            else
            {
                IsIntPtr = RealType == typeof(IntPtr);
                Size = Marshal.SizeOf(typeof(T));
                RealType = typeof(T);
            }

            // Basically, if any members of the type have a MarshalAs attributes, then we can't just pointer dereferenced. :(
            // This literally means any kind of MarshalAs. Strings, arrays, custom type sizes, etc.
            // Ideally, we want to avoid the Marshaler as much as possible. It causes a lot of overhead, and for a memory reading
            // lib where we need the best speed possible, we do things manually when possible!
            TypeRequiresMarshal =
                RealType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(
                    m => m.GetCustomAttributes(typeof(MarshalAsAttribute), true).Any());

            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod(string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")),
                typeof(void*),
                new[] { typeof(T).MakeByRefType() },
                typeof(SizeCache<>).Module);
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U);
            generator.Emit(OpCodes.Ret);
            GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
        }

        #region Nested type: GetUnsafePtrDelegate

        internal unsafe delegate void* GetUnsafePtrDelegate(ref T value);

        #endregion
    }
}
