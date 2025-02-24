using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CGAL_OBB_Wrapper
{

    internal class UnsafeNativeMethods
    {

        [DllImport("libCGAL.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void OrientedBoundingBoxBySurfaceMesh(
            [MarshalAs(UnmanagedType.LPArray)]double[] vert_xyz_array, ulong vert_count, /* input - mesh vertices */
            [MarshalAs(UnmanagedType.LPArray)]int[] face_index_array, ulong faces_count, /* input - mesh faces */
            ref IntPtr obb_pts_xyz
        );

        [DllImport("libCGAL.Native.dylib", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDoubleArray(IntPtr arr);
    }
}