using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace CGAL_OBB_Wrapper
{
    public class PolygonMeshProcessing
    {
        public static List<Point3d> ObbAsPoint3d(Mesh mesh)
        {
            //Clean and combine the mesh
            Mesh m = mesh.DuplicateMesh();
            m.Vertices.UseDoublePrecisionVertices = true;
            m.Faces.ConvertQuadsToTriangles();
            m.Vertices.CombineIdentical(true, true);
            m.Vertices.CullUnused();
            m.Weld(Math.PI);
            m.FillHoles();
            m.RebuildNormals();

            if(!m.IsValid || !m.IsClosed){
                return null;
            }

            //Extract info for vertices
            //The xyz coordinates of vertices of a mesh
            double[] vertXYZArray = new double[m.Vertices.Count * 3];
            for(int i = 0; i < m.Vertices.Count; i++){
                vertXYZArray[i * 3 + 0] = m.Vertices.Point3dAt(i).X;
                vertXYZArray[i * 3 + 1] = m.Vertices.Point3dAt(i).Y;
                vertXYZArray[i * 3 + 2] = m.Vertices.Point3dAt(i).Z;
            }

            var vertCount = (ulong)m.Vertices.Count;

            int[] faceIndexArray = m.Faces.ToIntArray(true);
            var facesCount = (ulong)m.Faces.Count;

            IntPtr obb_xyz_pointer = IntPtr.Zero;

            UnsafeNativeMethods.OrientedBoundingBoxBySurfaceMesh(
                vertXYZArray, 
                vertCount, 
                faceIndexArray, 
                facesCount, 
                ref obb_xyz_pointer);
            
            double[] obb_xyz_array = new double[8 * 3];
            Marshal.Copy(obb_xyz_pointer, obb_xyz_array, 0, 8 * 3);

            List<Point3d> points = new List<Point3d>();

            for(int i = 0; i < obb_xyz_array.Length; i+=3){
                points.Add(new Point3d(
                    obb_xyz_array[i + 0],
                    obb_xyz_array[i + 1],
                    obb_xyz_array[i + 2]));
            }

            UnsafeNativeMethods.ReleaseDoubleArray(obb_xyz_pointer);

            return points;

        }

        public static Box ObbAsBox(Mesh mesh){
            var pts = ObbAsPoint3d(mesh);
            var plane = new Plane(pts[0], pts[1], pts[2]);
            var box = new Box(plane, pts);

            return box;
        }

        public static Brep ObbAsBrep(Mesh mesh){
            return PolygonMeshProcessing.ObbAsBox(mesh).ToBrep();
        }

    }

}

