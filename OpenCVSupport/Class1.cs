//using ImageLib.Loader;
//using OpenCvSharp;
//using System;

////[assembly: InitLoader(typeof(OpenCVSupport.Class1))]
//namespace OpenCVSupport
//{

//    //public class Class1 : LibInitializer
//    //{
//    //    public override void FormatInitialize(Registers registers)
//    //    {
//    //        base.FormatInitialize(registers);
//    //        registers.RegisterFormat(CVFormate.Instance);
//    //    }

//    //    public override void Initialize(Registers registers)
//    //    {

//    //    }
//    //}

//    //public interface ICVMethod
//    //{
//    //    Mat Invoke(Mat input);
//    //}
//    //public abstract class CVMethod : BaseMethod<Mat, Mat>, ICVMethod
//    //{
//    //}

//    public static class CVFormate //: ImageTypeInfo<Mat, Mat, ICVMethod>
//    {
//        //public static CVFormate Instance { get; } = new CVFormate();

//        public static Mat CreateImage(Type elementType, int imageWidth, int imageHeight, int layerCount)
//        {
//            MatType? matType = null;
//            if (elementType == typeof(byte))
//            {
//                matType = MatType.CV_8UC(layerCount);
//            }
//            else if (elementType == typeof(ushort))
//            {
//                matType = MatType.CV_16UC(layerCount);
//            }
//            else if (elementType == typeof(int))
//            {
//                matType = MatType.CV_32SC(layerCount);
//            }
//            else if (elementType == typeof(sbyte))
//            {
//                matType = MatType.CV_8SC(layerCount);
//            }
//            else if (elementType == typeof(float))
//            {
//                matType = MatType.CV_32FC(layerCount);
//            }
//            else if (elementType == typeof(double))
//            {
//                matType = MatType.CV_64FC(layerCount);
//            }
//            return new Mat(imageHeight, imageWidth, matType.Value);
//        }

//        public override Mat CreateLayer(Type elementType, int layerWidth, int layerHeight)
//        {
//            return CreateImage(elementType, layerWidth, layerHeight, 1);
//        }

//        public override Array GetElementPointFromImage(Mat image, int x, int y)
//        {
//            int depth = image.Depth();
//            int channels = image.Channels();
//            //if (channels == 1) return GetElementPointFromLayer(image, x, y);
//            var meth = image.GetType().GetMethod(nameof(Mat.Get)).MakeGenericMethod(GetElementVecType(depth, channels));
//            return GetArrayFromStructVec(meth.Invoke(image, new object[] { x, y }));
//        }

//        public override object GetElementPointFromLayer(Mat layer, int x, int y) => layer.Depth() switch
//        {
//            0 => layer.Get<byte>(x, y),
//            1 => layer.Get<sbyte>(x, y),
//            2 => layer.Get<short>(x, y),
//            3 => layer.Get<ushort>(x, y),
//            4 => layer.Get<int>(x, y),
//            5 => layer.Get<float>(x, y),
//            6 => layer.Get<double>(x, y),
//            _ => null,
//        };

//        public override Type GetElementTypeFromImage(Mat image) =>
//            image.Depth() switch
//            {
//                0 => typeof(byte),
//                1 => typeof(sbyte),
//                2 => typeof(ushort),
//                3 => typeof(short),
//                4 => typeof(int),
//                5 => typeof(float),
//                6 => typeof(double),
//                _ => typeof(object),
//            };

//        public override Type GetElementTypeFromLayer(Mat layer) => GetElementTypeFromImage(layer);

//        public override Mat GetLayer(Mat image, int layerIndex)
//        {
//            return image.Split()[layerIndex];
//        }

//        public override (int width, int height) GetSizeImage(Mat image)
//        {
//            var size = image.Size();
//            return (size.Width, size.Height);
//        }

//        public override (int width, int height) GetSizeLayer(Mat layer)
//        {
//            var size = layer.Size();
//            return (size.Width, size.Height);
//        }

//        public override Mat Merge(Mat[] layers)
//        {
//            Mat mat = new Mat();
//            Cv2.Merge(layers, mat);
//            return mat;
//        }

//        public override void SetElementPointToImage(Mat image, Array element, int x, int y)
//        {
//            int depth = GetDepthFromType(element.GetType().GetElementType());
//            switch (element.Length)
//            {
//                case 1:
//                    switch (depth)
//                    {
//                        case 0:
//                            image.Set(x, y, (byte)element.GetValue(0));
//                            break;
//                        case 1:
//                            image.Set(x, y, (sbyte)element.GetValue(0));
//                            break;
//                        case 2:
//                            image.Set(x, y, (ushort)element.GetValue(0));
//                            break;
//                        case 3:
//                            image.Set(x, y, (short)element.GetValue(0));
//                            break;
//                        case 4:
//                            image.Set(x, y, (int)element.GetValue(0));
//                            break;
//                        case 5:
//                            image.Set(x, y, (float)element.GetValue(0));
//                            break;
//                        case 6:
//                            image.Set(x, y, (double)element.GetValue(0));
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case 2:
//                    switch (depth)
//                    {
//                        case 0:
//                            image.Set(x, y, (Vec2b)GetStructVec(element));
//                            break;
//                        case 2:
//                            image.Set(x, y, (Vec2w)GetStructVec(element));
//                            break;
//                        case 3:
//                            image.Set(x, y, (Vec2s)GetStructVec(element));
//                            break;
//                        case 4:
//                            image.Set(x, y, (Vec2i)element.GetValue(0));
//                            break;
//                        case 5:
//                            image.Set(x, y, (Vec2f)element.GetValue(0));
//                            break;
//                        case 6:
//                            image.Set(x, y, (Vec2d)element.GetValue(0));
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case 3:
//                    switch (depth)
//                    {
//                        case 0:
//                            image.Set(x, y, (Vec3b)GetStructVec(element));
//                            break;
//                        case 2:
//                            image.Set(x, y, (Vec3w)GetStructVec(element));
//                            break;
//                        case 3:
//                            image.Set(x, y, (Vec3s)GetStructVec(element));
//                            break;
//                        case 4:
//                            image.Set(x, y, (Vec3i)element.GetValue(0));
//                            break;
//                        case 5:
//                            image.Set(x, y, (Vec3f)element.GetValue(0));
//                            break;
//                        case 6:
//                            image.Set(x, y, (Vec3d)element.GetValue(0));
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case 4:
//                    switch (depth)
//                    {
//                        case 0:
//                            image.Set(x, y, (Vec4b)GetStructVec(element));
//                            break;
//                        case 2:
//                            image.Set(x, y, (Vec4w)GetStructVec(element));
//                            break;
//                        case 3:
//                            image.Set(x, y, (Vec4s)GetStructVec(element));
//                            break;
//                        case 4:
//                            image.Set(x, y, (Vec4i)element.GetValue(0));
//                            break;
//                        case 5:
//                            image.Set(x, y, (Vec4f)element.GetValue(0));
//                            break;
//                        case 6:
//                            image.Set(x, y, (Vec4d)element.GetValue(0));
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case 6:
//                    switch (depth)
//                    {
//                        case 0:
//                            image.Set(x, y, (Vec6b)GetStructVec(element));
//                            break;
//                        case 2:
//                            image.Set(x, y, (Vec6w)GetStructVec(element));
//                            break;
//                        case 3:
//                            image.Set(x, y, (Vec6s)GetStructVec(element));
//                            break;
//                        case 4:
//                            image.Set(x, y, (Vec6i)element.GetValue(0));
//                            break;
//                        case 5:
//                            image.Set(x, y, (Vec6f)element.GetValue(0));
//                            break;
//                        case 6:
//                            image.Set(x, y, (Vec6d)element.GetValue(0));
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//            }
//        }

//        public override void SetElementPointToLayer(Mat layer, object element, int x, int y)
//        {
//            switch (layer.Depth())
//            {
//                case 0:
//                    layer.Set(x, y, (byte)element);
//                    break;
//                case 1:
//                    layer.Set(x, y, (sbyte)element);
//                    break;
//                case 2:
//                    layer.Set(x, y, (ushort)element);
//                    break;
//                case 3:
//                    layer.Set(x, y, (short)element);
//                    break;
//                case 4:
//                    layer.Set(x, y, (int)element);
//                    break;
//                case 5:
//                    layer.Set(x, y, (float)element);
//                    break;
//                case 6:
//                    layer.Set(x, y, (double)element);
//                    break;
//                default:
//                    break;
//            }
//        }

//        public override Mat[] Split(Mat image) => image.Split();

//        public int GetDepthFromType(Type depthType)
//        {
//            if (depthType == typeof(byte)) return 0;
//            else if (depthType == typeof(sbyte)) return 1;
//            else if (depthType == typeof(ushort)) return 2;
//            else if (depthType == typeof(short)) return 3;
//            else if (depthType == typeof(int)) return 4;
//            else if (depthType == typeof(float)) return 5;
//            else if (depthType == typeof(double)) return 6;
//            else return -1;
//        }


//        public int GetDepthFromTypeExt(Type depthType)
//        {
//            if (depthType == typeof(byte)) return 0;
//            else if (depthType == typeof(sbyte)) return 1;
//            else if (depthType == typeof(ushort)) return 2;
//            else if (depthType == typeof(short)) return 3;
//            else if (depthType == typeof(int)) return 4;
//            else if (depthType == typeof(float)) return 5;
//            else if (depthType == typeof(double)) return 6;
//            else if (depthType == typeof(Vec2b)) return 0;
//            else if (depthType == typeof(Vec2w)) return 2;
//            else if (depthType == typeof(Vec2s)) return 3;
//            else if (depthType == typeof(Vec2i)) return 4;
//            else if (depthType == typeof(Vec2f)) return 5;
//            else if (depthType == typeof(Vec2d)) return 6;
//            else if (depthType == typeof(Vec3b)) return 0;
//            else if (depthType == typeof(Vec3w)) return 2;
//            else if (depthType == typeof(Vec3s)) return 3;
//            else if (depthType == typeof(Vec3i)) return 4;
//            else if (depthType == typeof(Vec3f)) return 5;
//            else if (depthType == typeof(Vec3d)) return 6;
//            else if (depthType == typeof(Vec4b)) return 0;
//            else if (depthType == typeof(Vec4w)) return 2;
//            else if (depthType == typeof(Vec4s)) return 3;
//            else if (depthType == typeof(Vec4i)) return 4;
//            else if (depthType == typeof(Vec4f)) return 5;
//            else if (depthType == typeof(Vec4d)) return 6;
//            else if (depthType == typeof(Vec6b)) return 0;
//            else if (depthType == typeof(Vec6w)) return 2;
//            else if (depthType == typeof(Vec6s)) return 3;
//            else if (depthType == typeof(Vec6i)) return 4;
//            else if (depthType == typeof(Vec6f)) return 5;
//            else if (depthType == typeof(Vec6d)) return 6;
//            else return -1;
//        }

//        public Type GetElementVecType(int depth, int channelCount) => channelCount switch
//        {
//            1 => depth switch
//            {
//                0 => typeof(byte),
//                1 => typeof(sbyte),
//                2 => typeof(short),
//                3 => typeof(ushort),
//                4 => typeof(int),
//                5 => typeof(float),
//                6 => typeof(double),
//                _ => typeof(object),
//            },
//            2 => depth switch
//            {
//                0 => typeof(Vec2b),
//                2 => typeof(Vec2w),
//                3 => typeof(Vec2s),
//                4 => typeof(Vec2i),
//                5 => typeof(Vec2f),
//                6 => typeof(Vec2d),
//                _ => typeof(object),
//            },
//            3 => depth switch
//            {
//                0 => typeof(Vec3b),
//                2 => typeof(Vec3w),
//                3 => typeof(Vec3s),
//                4 => typeof(Vec3i),
//                5 => typeof(Vec3f),
//                6 => typeof(Vec3d),
//                _ => typeof(object),
//            },
//            4 => depth switch
//            {
//                0 => typeof(Vec4b),
//                2 => typeof(Vec4w),
//                3 => typeof(Vec4s),
//                4 => typeof(Vec4i),
//                5 => typeof(Vec4f),
//                6 => typeof(Vec4d),
//                _ => typeof(object),
//            },
//            6 => depth switch
//            {
//                0 => typeof(Vec6b),
//                2 => typeof(Vec6w),
//                3 => typeof(Vec6s),
//                4 => typeof(Vec6i),
//                5 => typeof(Vec6f),
//                6 => typeof(Vec6d),
//                _ => typeof(object),
//            },
//            _ => typeof(object),
//        };

//        public Type GetElementVecType(Type depthType, int channelCount)
//        {
//            int depth;
//            if (depthType == typeof(byte)) depth = 0;
//            else if (depthType == typeof(sbyte)) depth = 1;
//            else if (depthType == typeof(ushort)) depth = 2;
//            else if (depthType == typeof(short)) depth = 3;
//            else if (depthType == typeof(int)) depth = 4;
//            else if (depthType == typeof(float)) depth = 5;
//            else if (depthType == typeof(double)) depth = 6;
//            else depth = -1;
//            if (channelCount == 1 && depth >= 0 && depth <= 6) return depthType;
//            return GetElementVecType(depth, channelCount);
//        }

//        public object GetStructVec(Array array)
//        {
//            int depth = GetDepthFromType(array.GetType().GetElementType());

//            return array.GetLength(0) switch
//            {
//                1 => depth switch
//                {
//                    0 => (byte)array.GetValue(0),
//                    1 => (sbyte)array.GetValue(0),
//                    2 => (short)array.GetValue(0),
//                    3 => (ushort)array.GetValue(0),
//                    4 => (int)array.GetValue(0),
//                    5 => (float)array.GetValue(0),
//                    6 => (double)array.GetValue(0),
//                    _ => (object)array.GetValue(0),
//                },
//                2 => depth switch
//                {
//                    0 => new Vec2b((byte)array.GetValue(0), (byte)array.GetValue(1)),
//                    2 => new Vec2w((ushort)array.GetValue(0), (ushort)array.GetValue(1)),
//                    3 => new Vec2s((short)array.GetValue(0), (short)array.GetValue(1)),
//                    4 => new Vec2i((int)array.GetValue(0), (int)array.GetValue(1)),
//                    5 => new Vec2f((float)array.GetValue(0), (float)array.GetValue(1)),
//                    6 => new Vec2d((double)array.GetValue(0), (double)array.GetValue(1)),
//                    _ => null,
//                },
//                3 => depth switch
//                {
//                    0 => new Vec3b((byte)array.GetValue(0), (byte)array.GetValue(1), (byte)array.GetValue(2)),
//                    2 => new Vec3w((ushort)array.GetValue(0), (ushort)array.GetValue(1), (ushort)array.GetValue(2)),
//                    3 => new Vec3s((short)array.GetValue(0), (short)array.GetValue(1), (short)array.GetValue(2)),
//                    4 => new Vec3i((int)array.GetValue(0), (int)array.GetValue(1), (int)array.GetValue(2)),
//                    5 => new Vec3f((float)array.GetValue(0), (float)array.GetValue(1), (float)array.GetValue(2)),
//                    6 => new Vec3d((double)array.GetValue(0), (double)array.GetValue(1), (double)array.GetValue(2)),
//                    _ => null,
//                },
//                4 => depth switch
//                {
//                    0 => new Vec4b((byte)array.GetValue(0), (byte)array.GetValue(1), (byte)array.GetValue(2), (byte)array.GetValue(3)),
//                    2 => new Vec4w((ushort)array.GetValue(0), (ushort)array.GetValue(1), (ushort)array.GetValue(2), (ushort)array.GetValue(3)),
//                    3 => new Vec4s((short)array.GetValue(0), (short)array.GetValue(1), (short)array.GetValue(2), (short)array.GetValue(3)),
//                    4 => new Vec4i((int)array.GetValue(0), (int)array.GetValue(1), (int)array.GetValue(2), (int)array.GetValue(3)),
//                    5 => new Vec4f((float)array.GetValue(0), (float)array.GetValue(1), (float)array.GetValue(2), (float)array.GetValue(3)),
//                    6 => new Vec4d((double)array.GetValue(0), (double)array.GetValue(1), (double)array.GetValue(2), (double)array.GetValue(3)),
//                    _ => null,
//                },
//                6 => depth switch
//                {
//                    0 => new Vec6b((byte)array.GetValue(0), (byte)array.GetValue(1), (byte)array.GetValue(2), (byte)array.GetValue(3), (byte)array.GetValue(4), (byte)array.GetValue(5)),
//                    2 => new Vec6w((ushort)array.GetValue(0), (ushort)array.GetValue(1), (ushort)array.GetValue(2), (ushort)array.GetValue(3), (ushort)array.GetValue(4), (ushort)array.GetValue(5)),
//                    3 => new Vec6s((short)array.GetValue(0), (short)array.GetValue(1), (short)array.GetValue(2), (short)array.GetValue(3), (short)array.GetValue(4), (short)array.GetValue(5)),
//                    4 => new Vec6i((int)array.GetValue(0), (int)array.GetValue(1), (int)array.GetValue(2), (int)array.GetValue(3), (int)array.GetValue(4), (int)array.GetValue(5)),
//                    5 => new Vec6f((float)array.GetValue(0), (float)array.GetValue(1), (float)array.GetValue(2), (float)array.GetValue(3), (float)array.GetValue(4), (float)array.GetValue(5)),
//                    6 => new Vec6d((double)array.GetValue(0), (double)array.GetValue(1), (double)array.GetValue(2), (double)array.GetValue(3), (double)array.GetValue(4), (double)array.GetValue(5)),
//                    _ => null,
//                },
//                _ => null,
//            };
//        }

//        public Array GetArrayFromStructVec(object obj)
//        {
//            if (obj is byte val10) return new byte[] { val10 };
//            else if (obj is sbyte val11) return new sbyte[] { val11 };
//            else if (obj is ushort val12) return new ushort[] { val12 };
//            else if (obj is short val13) return new short[] { val13 };
//            else if (obj is int val14) return new int[] { val14 };
//            else if (obj is float val15) return new float[] { val15 };
//            else if (obj is double val16) return new double[] { val16 };
//            else if (obj is Vec2b val20) return new byte[] { val20.Item0, val20.Item1 };
//            else if (obj is Vec2w val22) return new ushort[] { val22.Item0, val22.Item1 };
//            else if (obj is Vec2s val23) return new short[] { val23.Item0, val23.Item1 };
//            else if (obj is Vec2i val24) return new int[] { val24.Item0, val24.Item1 };
//            else if (obj is Vec2f val25) return new float[] { val25.Item0, val25.Item1 };
//            else if (obj is Vec2d val26) return new double[] { val26.Item0, val26.Item1 };
//            else if (obj is Vec3b val30) return new byte[] { val30.Item0, val30.Item1, val30.Item2 };
//            else if (obj is Vec3w val32) return new ushort[] { val32.Item0, val32.Item1, val32.Item2 };
//            else if (obj is Vec3s val33) return new short[] { val33.Item0, val33.Item1, val33.Item2 };
//            else if (obj is Vec3i val34) return new int[] { val34.Item0, val34.Item1, val34.Item2 };
//            else if (obj is Vec3f val35) return new float[] { val35.Item0, val35.Item1, val35.Item2 };
//            else if (obj is Vec3d val36) return new double[] { val36.Item0, val36.Item1, val36.Item2 };
//            else if (obj is Vec4b val40) return new byte[] { val40.Item0, val40.Item1, val40.Item2, val40.Item3 };
//            else if (obj is Vec4w val42) return new ushort[] { val42.Item0, val42.Item1, val42.Item2, val42.Item3 };
//            else if (obj is Vec4s val43) return new short[] { val43.Item0, val43.Item1, val43.Item2, val43.Item3 };
//            else if (obj is Vec4i val44) return new int[] { val44.Item0, val44.Item1, val44.Item2, val44.Item3 };
//            else if (obj is Vec4f val45) return new float[] { val45.Item0, val45.Item1, val45.Item2, val45.Item3 };
//            else if (obj is Vec4d val46) return new double[] { val46.Item0, val46.Item1, val46.Item2, val46.Item3 };
//            else if (obj is Vec6b val60) return new byte[] { val60.Item0, val60.Item1, val60.Item2, val60.Item3, val60.Item4, val60.Item5 };
//            else if (obj is Vec6w val62) return new ushort[] { val62.Item0, val62.Item1, val62.Item2, val62.Item3, val62.Item4, val62.Item5 };
//            else if (obj is Vec6s val63) return new short[] { val63.Item0, val63.Item1, val63.Item2, val63.Item3, val63.Item4, val63.Item5 };
//            else if (obj is Vec6i val64) return new int[] { val64.Item0, val64.Item1, val64.Item2, val64.Item3, val64.Item4, val64.Item5 };
//            else if (obj is Vec6f val65) return new float[] { val65.Item0, val65.Item1, val65.Item2, val65.Item3, val65.Item4, val65.Item5 };
//            else if (obj is Vec6d val66) return new double[] { val66.Item0, val66.Item1, val66.Item2, val66.Item3, val66.Item4, val66.Item5 };
//            else return null;
//        }

//        public override int GetLayerCount(Mat image) => image.Channels();
//    }
//}
