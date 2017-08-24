using System;
using System.Runtime.InteropServices;

namespace DepthSenseWarper
{
    /// \struct Vertex DepthSense.hxx
    /// A point in space as defined by its integer coordinates
    ///   
    /// The Vertex struct holds the position of a point in space as
    /// defined by its 3D integer coordinates.
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        /// the x value
        [MarshalAs(UnmanagedType.I2)]
        public Int16 x;
        /// the y value
        [MarshalAs(UnmanagedType.I2)]
        public Int16 y;
        /// the z value
        [MarshalAs(UnmanagedType.I2)]
        public Int16 z;
    };


    /// \struct FPVertex DepthSense.hxx
    /// A point in space as defined by its floating point coordinates
    ///   
    /// The FPVertex struct holds the position of a point in space as
    /// defined by its 3D floating point coordinates.
    [StructLayout(LayoutKind.Sequential)]
    public struct FPVertex
    {
        /// the x value
        [MarshalAs(UnmanagedType.R4)]
        public float x;
        /// the y value
        [MarshalAs(UnmanagedType.R4)]
        public float y;
        /// the z value
        [MarshalAs(UnmanagedType.R4)]
        public float z;
    };

    /// \struct UV DepthSense.hxx
    /// UV coordinates
    ///   
    /// The UV struct holds the UV coordinates of a point of a UV map.
    [StructLayout(LayoutKind.Sequential)]
    public struct UV
    {
        /// the u value
        [MarshalAs(UnmanagedType.R4)]
        public float u;
        /// the v value
        [MarshalAs(UnmanagedType.R4)]
        public float v;
    };

    public class Warper
    {
        //public const string dllname = @"C:\Users\rice.yang\Documents\Visual Studio 2015\Projects\DepthSense\Debug\Coord3D.dll";
        public const string dllname = @"Coord3D.dll";

        [DllImport(dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "Initialize")]
        public static extern void Initialize();

        [DllImport(dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "Finalize")]
        public static extern void Finalization();

        [DllImport(dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "DoNothing")]
        public static extern void DoNothing();

        [DllImport(dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "SizeofStruct")]
        public static extern int SizeofStruct([In] String structName);
    }

    public abstract class NewReceivedData
    {
        protected static void AllocAndCopy<T>(IntPtr src, ref T[] dst, int size)
        {
            int typeSize = Marshal.SizeOf(typeof(T));

            // get nothing
            if (src == IntPtr.Zero)
                return;
            // new allocate
            else if (dst == null)
                dst = new T[size];
            // re-allocate
            else if (size != dst.Length)
                Array.Resize(ref dst, size);
            // else: don't allocate

            GCHandle hDst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            try
            {
                IntPtr dstPtr = hDst.AddrOfPinnedObject();
                byte[] buf = new byte[size * typeSize];

                Marshal.Copy(src, buf, 0, buf.Length);
                Marshal.Copy(buf, 0, dstPtr, buf.Length);
            }
            finally
            {
                hDst.Free();
            }
        }

        //[DllImport("msvcrt.dll", EntryPoint = "memcpy", SetLastError = false)]
        //static extern IntPtr CopyMemory(IntPtr dst, IntPtr src, int count);
    }

    namespace ColorExport
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NewSampleReceivedDataPtr
        {
            /// The color map. If \c captureConfiguration::compression is DepthSense::COMPRESSION_TYPE_MJPEG, the output format is BGR, otherwise the output format is YUY2.
            public IntPtr colorMap;
            public int ncolorMap;
            /// The compressed data. If \c captureConfiguration::compression is DepthSense::COMPRESSION_TYPE_MJPEG, this array contains the compmressed data.
            public IntPtr compressedData;
            public int ncompressedData;
            /// the camera configuration that was in effect at the time of capture
            //public DepthSense::ColorNode::Configuration captureConfiguration;
            /// the time of capture of the sample, expressed in ?s
            public UInt64 timeOfCapture;
            /// the time of arrival of the sample in the library, expressed in ?s
            public UInt64 timeOfArrival;
            /// the number of dropped samples since the last \c newSampleReceived event was raised
            public UInt32 droppedSampleCount;
            /// the number of dropped samples since the streaming was started
            public UInt32 cumulativeDroppedSampleCount;
        };

        public class NewSampleReceivedData
        {
            /// The color map. If \c captureConfiguration::compression is DepthSense::COMPRESSION_TYPE_MJPEG, the output format is BGR, otherwise the output format is YUY2.
            public Byte[] colorMap;
            /// The compressed data. If \c captureConfiguration::compression is DepthSense::COMPRESSION_TYPE_MJPEG, this array contains the compmressed data.
            public Byte[] compressedData;
            /// the camera configuration that was in effect at the time of capture
            //public DepthSense::ColorNode::Configuration captureConfiguration;
            /// the time of capture of the sample, expressed in ?s
            public UInt64 timeOfCapture;
            /// the time of arrival of the sample in the library, expressed in ?s
            public UInt64 timeOfArrival;
            /// the number of dropped samples since the last \c newSampleReceived event was raised
            public UInt32 droppedSampleCount;
            /// the number of dropped samples since the streaming was started
            public UInt32 cumulativeDroppedSampleCount;
        };

        public class NewReceivedData: DepthSenseWarper.NewReceivedData
        {
            public NewSampleReceivedData data = new NewSampleReceivedData();
            public bool IsNodeSet()
            {
                return dllIsNodeSet();
            }

            NewSampleReceivedDataPtr prev = new NewSampleReceivedDataPtr();
            public void GetData()
            {
                LockData();
                try
                {
                    NewSampleReceivedDataPtr dataPtr = dllGetData();

                    if (dataPtr.timeOfCapture != prev.timeOfCapture || dataPtr.timeOfArrival != prev.timeOfArrival)
                    {
                        AllocAndCopy(dataPtr.colorMap, ref data.colorMap, dataPtr.ncolorMap);
                        AllocAndCopy(dataPtr.compressedData, ref data.compressedData, dataPtr.ncompressedData);
                        data.timeOfCapture = dataPtr.timeOfCapture;
                        data.timeOfArrival = dataPtr.timeOfArrival;
                        data.droppedSampleCount = dataPtr.droppedSampleCount;
                        data.cumulativeDroppedSampleCount = dataPtr.cumulativeDroppedSampleCount;
                    }
                    prev = dataPtr;
                }
                finally
                {
                    UnlockData();
                }
            }

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, 
                EntryPoint = "GetColorData")]
            static extern NewSampleReceivedDataPtr dllGetData();

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "LockColorMutex")]
            static extern void LockData();

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "UnlockColorMutex")]
            static extern void UnlockData();

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "IsColorNodeSet")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool dllIsNodeSet();
        }
    }

    namespace DepthExport
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NewSampleReceivedDataPtr
        {
            /// the confidence map
            public IntPtr confidenceMap;
            public int nconfidenceMap;
            /// The phase map. This map represents the radial phase ([0 - 2π[) with respect to the center of the depth camera. Valid values lie in the range [0 - 32767]. Saturated pixels are given the special value \c -32767.
            public IntPtr phaseMap;
            public int nphaseMap;
            /// The depth map in fixed point format. This map represents the cartesian depth of each pixel, expressed in millimeters. Valid values lies in the range [0 - 31999]. Saturated pixels are given the special value \c 32002.
            public IntPtr depthMap;
            public int ndepthMap;
            /// The depth map in floating point format. This map represents the cartesian depth of each pixel, expressed in meters. Saturated pixels are given the special value \c -2.0.
            public IntPtr depthMapFloatingPoint;
            public int ndepthMapFloatingPoint;
            /// The vertices in fixed point format. This map represents the cartesian 3D coordinates of each pixel, expressed in millimeters. Saturated pixels are given the special value \c 32002.
            public IntPtr vertices;
            public int nvertices;
            /// The vertices in floating point format. This map represents the cartesian 3D coordinates of each pixel, expressed in meters. Saturated pixels are given the special value \c -2.0.
            public IntPtr verticesFloatingPoint;
            public int nverticesFloatingPoint;
            /// The UV mapping. This map represents the normalized coordinates of each pixel in the color map. Invalid pixels are given the special value \c -FLT_MAX.
            public IntPtr uvMap;
            public int nuvMap;
            /// The acceleration of the camera when the frame was captured. The sampling frequency of this value is 1 Hz.
            //public DepthSense::DepthNode::Acceleration acceleration;
            /// the system model parameters that were in effect at the time of capture
            //public DepthSense::StereoCameraParameters stereoCameraParameters;
            /// the camera configuration that was in effect at the time of capture
            //public DepthSense::DepthNode::Configuration captureConfiguration;
            /// the time of capture of the sample, expressed in ?s
            public UInt64 timeOfCapture;
            /// the time of arrival of the sample in the library, expressed in ?s
            public UInt64 timeOfArrival;
            /// the number of dropped samples since the last \c newSampleReceived event was raised
            public UInt32 droppedSampleCount;
            /// the number of dropped samples since the streaming was started
            public UInt32 cumulativeDroppedSampleCount;
        };

        public class NewSampleReceivedData
        {
            /// the confidence map
            public Int16[] confidenceMap = null;
            /// The phase map. This map represents the radial phase ([0 - 2π[) with respect to the center of the depth camera. Valid values lie in the range [0 - 32767]. Saturated pixels are given the special value \c -32767.
            public Int16[] phaseMap = null;
            /// The depth map in fixed point format. This map represents the cartesian depth of each pixel, expressed in millimeters. Valid values lies in the range [0 - 31999]. Saturated pixels are given the special value \c 32002.
            public Int16[] depthMap = null;
            /// The depth map in floating point format. This map represents the cartesian depth of each pixel, expressed in meters. Saturated pixels are given the special value \c -2.0.
            public float[] depthMapFloatingPoint = null;
            /// The vertices in fixed point format. This map represents the cartesian 3D coordinates of each pixel, expressed in millimeters. Saturated pixels are given the special value \c 32002.
            public Vertex[] vertices = null;
            /// The vertices in floating point format. This map represents the cartesian 3D coordinates of each pixel, expressed in meters. Saturated pixels are given the special value \c -2.0.
            public FPVertex[] verticesFloatingPoint = null;
            /// The UV mapping. This map represents the normalized coordinates of each pixel in the color map. Invalid pixels are given the special value \c -FLT_MAX.
            public UV[] uvMap = null;
            /// The acceleration of the camera when the frame was captured. The sampling frequency of this value is 1 Hz.
            //public DepthSense::DepthNode::Acceleration acceleration;
            /// the system model parameters that were in effect at the time of capture
            //public DepthSense::StereoCameraParameters stereoCameraParameters;
            /// the camera configuration that was in effect at the time of capture
            //public DepthSense::DepthNode::Configuration captureConfiguration;
            /// the time of capture of the sample, expressed in ?s
            public UInt64 timeOfCapture = 0;
            /// the time of arrival of the sample in the library, expressed in ?s
            public UInt64 timeOfArrival = 0;
            /// the number of dropped samples since the last \c newSampleReceived event was raised
            public UInt32 droppedSampleCount = 0;
            /// the number of dropped samples since the streaming was started
            public UInt32 cumulativeDroppedSampleCount = 0;
        };

        public class NewReceivedData : DepthSenseWarper.NewReceivedData
        {
            public NewSampleReceivedData data = new NewSampleReceivedData();
            public bool IsNodeSet()
            {
                return dllIsNodeSet();
            }

            NewSampleReceivedDataPtr prev = new NewSampleReceivedDataPtr();
            public void GetData()
            {
                LockData();
                try
                {
                    NewSampleReceivedDataPtr dataPtr = dllGetData();

                    if (dataPtr.timeOfCapture != prev.timeOfCapture || dataPtr.timeOfArrival != prev.timeOfArrival)
                    {
                        AllocAndCopy(dataPtr.confidenceMap, ref data.confidenceMap, dataPtr.nconfidenceMap);
                        AllocAndCopy(dataPtr.phaseMap, ref data.phaseMap, dataPtr.nphaseMap);
                        AllocAndCopy(dataPtr.depthMap, ref data.depthMap, dataPtr.ndepthMap);
                        AllocAndCopy(dataPtr.depthMapFloatingPoint, ref data.depthMapFloatingPoint, dataPtr.ndepthMapFloatingPoint);
                        AllocAndCopy(dataPtr.vertices, ref data.vertices, dataPtr.nvertices);
                        AllocAndCopy(dataPtr.verticesFloatingPoint, ref data.verticesFloatingPoint, dataPtr.nverticesFloatingPoint);
                        AllocAndCopy(dataPtr.uvMap, ref data.uvMap, dataPtr.nuvMap);
                        data.timeOfCapture = dataPtr.timeOfCapture;
                        data.timeOfArrival = dataPtr.timeOfArrival;
                        data.droppedSampleCount = dataPtr.droppedSampleCount;
                        data.cumulativeDroppedSampleCount = dataPtr.cumulativeDroppedSampleCount;
                    }
                    prev = dataPtr;
                }
                finally
                {
                    UnlockData();
                }
            }
            
            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, 
                EntryPoint = "GetDepthData")]
            static extern NewSampleReceivedDataPtr dllGetData();

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "LockDepthMutex")]
            static extern void LockData();

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "UnlockDepthMutex")]
            static extern void UnlockData();

            [DllImport(Warper.dllname, CallingConvention = CallingConvention.Winapi, EntryPoint = "IsDepthNodeSet")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool dllIsNodeSet();
        }

    }
}
