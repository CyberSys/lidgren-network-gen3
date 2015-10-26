
#if UNITY_5
namespace Lidgren.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    public static class UnityExtensions
    {
        /*
        /// <summary>
        /// Write a Point
        /// </summary>
        public static void Write(this NetBuffer message, Point value) {
            message.Write(value.X);
            message.Write(value.Y);
            
        }

        /// <summary>
        /// Read a Point
        /// </summary>
        public static Point ReadPoint(this NetBuffer message) {
            return new Point(message.ReadInt32(), message.ReadInt32());
        }
        */
        /// <summary>
        /// Write a Single with half precision (16 bits)
        /// </summary>
        public static void WriteHalfPrecision(this NetBuffer message, float value) {
            message.Write(new HalfSingle(value).PackedValue);
        }

        /// <summary>
        /// Reads a half precision Single written using WriteHalfPrecision(float)
        /// </summary>
        public static float ReadHalfPrecisionSingle(this NetBuffer message) {
            HalfSingle h = new HalfSingle();
            h.PackedValue = message.ReadUInt16();
            return h.ToSingle();
        }

        /// <summary>
        /// Writes a Vector2
        /// </summary>
        public static void Write(this NetBuffer message, Vector2 vector) {
            message.Write(vector.x);
            message.Write(vector.y);
        }

        /// <summary>
        /// Reads a Vector2
        /// </summary>
        public static Vector2 ReadVector2(this NetBuffer message) {
            Vector2 retval;
            retval.x = message.ReadSingle();
            retval.y = message.ReadSingle();
            return retval;
        }

        /// <summary>
        /// Writes a Vector3
        /// </summary>
        public static void Write(this NetBuffer message, Vector3 vector) {
            message.Write(vector.x);
            message.Write(vector.y);
            message.Write(vector.z);
        }

        /// <summary>
        /// Writes a Vector3 at half precision
        /// </summary>
        public static void WriteHalfPrecision(this NetBuffer message, Vector3 vector) {
            message.Write(new HalfSingle(vector.x).PackedValue);
            message.Write(new HalfSingle(vector.y).PackedValue);
            message.Write(new HalfSingle(vector.z).PackedValue);
        }

        /// <summary>
        /// Reads a Vector3
        /// </summary>
        public static Vector3 ReadVector3(this NetBuffer message) {
            Vector3 retval;
            retval.x = message.ReadSingle();
            retval.y = message.ReadSingle();
            retval.z = message.ReadSingle();
            return retval;
        }

        /// <summary>
        /// Writes a Vector3 at half precision
        /// </summary>
        public static Vector3 ReadHalfPrecisionVector3(this NetBuffer message) {
            HalfSingle hx = new HalfSingle();
            hx.PackedValue = message.ReadUInt16();

            HalfSingle hy = new HalfSingle();
            hy.PackedValue = message.ReadUInt16();

            HalfSingle hz = new HalfSingle();
            hz.PackedValue = message.ReadUInt16();

            Vector3 retval;
            retval.x = hx.ToSingle();
            retval.y = hy.ToSingle();
            retval.z = hz.ToSingle();
            return retval;
        }

        /// <summary>
        /// Writes a Vector4
        /// </summary>
        public static void Write(this NetBuffer message, Vector4 vector) {
            message.Write(vector.x);
            message.Write(vector.y);
            message.Write(vector.z);
            message.Write(vector.w);
        }
        public static void Write(this NetBuffer message, Color vector) {            
            message.WriteHalfPrecision(vector.r);
            message.WriteHalfPrecision(vector.g);
            message.WriteHalfPrecision(vector.b);
            message.WriteHalfPrecision(vector.a);
        }
        public static void Write(this NetBuffer message, Color32 vector) {
            message.Write(vector.r);
            message.Write(vector.g);
            message.Write(vector.b);
            message.Write(vector.a);
        }
        public static Color32 ReadColor32(this NetBuffer message) {
            Color32 retval;
            retval.r = message.ReadByte();
            retval.g = message.ReadByte();
            retval.b = message.ReadByte();
            retval.a = message.ReadByte();
            return retval;
        }
        public static Color ReadColor(this NetBuffer message) {
            Color retval;
            retval.r = message.ReadHalfPrecisionSingle();
            retval.g = message.ReadHalfPrecisionSingle();
            retval.b = message.ReadHalfPrecisionSingle();
            retval.a = message.ReadHalfPrecisionSingle();
            return retval;
        }
        /// <summary>
        /// Reads a Vector4
        /// </summary>
        public static Vector4 ReadVector4(this NetBuffer message) {
            Vector4 retval;
            retval.x = message.ReadSingle();
            retval.y = message.ReadSingle();
            retval.z = message.ReadSingle();
            retval.w = message.ReadSingle();
            return retval;
        }


        /// <summary>
        /// Writes a unit vector (ie. a vector of length 1.0, for example a surface normal) 
        /// using specified number of bits
        /// </summary>
        public static void WriteUnitVector3(this NetBuffer message, Vector3 unitVector, int numberOfBits) {
            float x = unitVector.x;
            float y = unitVector.y;
            float z = unitVector.z;
            double invPi = 1.0 / Math.PI;
            float phi = (float)(Math.Atan2(x, y) * invPi);
            float theta = (float)(Math.Atan2(z, Math.Sqrt(x * x + y * y)) * (invPi * 2));

            int halfBits = numberOfBits / 2;
            message.WriteSignedSingle(phi, halfBits);
            message.WriteSignedSingle(theta, numberOfBits - halfBits);
        }

        /// <summary>
        /// Reads a unit vector written using WriteUnitVector3(numberOfBits)
        /// </summary>
        public static Vector3 ReadUnitVector3(this NetBuffer message, int numberOfBits) {
            int halfBits = numberOfBits / 2;
            float phi = message.ReadSignedSingle(halfBits) * (float)Math.PI;
            float theta = message.ReadSignedSingle(numberOfBits - halfBits) * (float)(Math.PI * 0.5);

            Vector3 retval;
            retval.x = (float)(Math.Sin(phi) * Math.Cos(theta));
            retval.y = (float)(Math.Cos(phi) * Math.Cos(theta));
            retval.z = (float)Math.Sin(theta);

            return retval;
        }

        /// <summary>
        /// Writes a unit quaternion using the specified number of bits per element
        /// for a total of 4 x bitsPerElements bits. Suggested value is 8 to 24 bits.
        /// </summary>
        public static void WriteRotation(this NetBuffer message, Quaternion quaternion, int bitsPerElement) {
            if (quaternion.x > 1.0f)
                quaternion.x = 1.0f;
            if (quaternion.y > 1.0f)
                quaternion.y = 1.0f;
            if (quaternion.z > 1.0f)
                quaternion.z = 1.0f;
            if (quaternion.w > 1.0f)
                quaternion.w = 1.0f;
            if (quaternion.x < -1.0f)
                quaternion.x = -1.0f;
            if (quaternion.y < -1.0f)
                quaternion.y = -1.0f;
            if (quaternion.z < -1.0f)
                quaternion.z = -1.0f;
            if (quaternion.w < -1.0f)
                quaternion.w = -1.0f;

            message.WriteSignedSingle(quaternion.x, bitsPerElement);
            message.WriteSignedSingle(quaternion.y, bitsPerElement);
            message.WriteSignedSingle(quaternion.z, bitsPerElement);
            message.WriteSignedSingle(quaternion.w, bitsPerElement);
        }

        /// <summary>
        /// Reads a unit quaternion written using WriteRotation(... ,bitsPerElement)
        /// </summary>
        public static Quaternion ReadRotation(this NetBuffer message, int bitsPerElement) {
            Quaternion retval;
            retval.x = message.ReadSignedSingle(bitsPerElement);
            retval.y = message.ReadSignedSingle(bitsPerElement);
            retval.z = message.ReadSignedSingle(bitsPerElement);
            retval.w = message.ReadSignedSingle(bitsPerElement);
            return retval;
        }

        /// <summary>
        /// Writes an orthonormal matrix (rotation, translation but not scaling or projection)
        /// </summary>
        public static void WriteMatrix4x4(this NetBuffer message, ref Matrix4x4 matrix) {           
            //Quaternion rot =Quaternion.CreateFromRotationMatrix(matrix);
            Quaternion rot = QuaternionFromMatrix(matrix);
            WriteRotation(message, rot, 24);
            message.Write(matrix.m31);
            message.Write(matrix.m32);
            message.Write(matrix.m33);
        }

        /// <summary>
        /// Writes an orthonormal matrix (rotation, translation but no scaling or projection)
        /// </summary>
        public static void WriteMatrix4x4(this NetBuffer message, Matrix4x4 matrix) {
            //Quaternion rot = Quaternion.CreateFromRotationMatrix(matrix);
            Quaternion rot = QuaternionFromMatrix(matrix);
            WriteRotation(message, rot, 24);
            message.Write(matrix.m31);
            message.Write(matrix.m32);
            message.Write(matrix.m33);
        }
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m) { 
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); 
        }
        /// <summary>
        /// Reads a matrix written using WriteMatrix()
        /// </summary>
        public static Matrix4x4 ReadMatrix4x4(this NetBuffer message) {
            Quaternion rot = ReadRotation(message, 24);
            //Matrix4x4 m = Matrix4x4.TRS(translation, rotation, scale);
            Matrix4x4 retval = Matrix4x4.TRS(Vector3.zero,rot,Vector3.one);//CreateFromQuaternion(rot);
            retval.m31 = message.ReadSingle();
            retval.m32 = message.ReadSingle();
            retval.m33 = message.ReadSingle();
            return retval;
        }

        /// <summary>
        /// Reads a matrix written using WriteMatrix()
        /// </summary>
        public static void ReadMatrix4x4(this NetBuffer message, ref Matrix4x4 destination) {
            Quaternion rot = ReadRotation(message, 24);
            destination = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one); //Matrix.CreateFromQuaternion(rot);
            destination.m31 = message.ReadSingle();
            destination.m32 = message.ReadSingle();
            destination.m33 = message.ReadSingle();
        }
        /*
        /// <summary>
        /// Writes a bounding sphere
        /// </summary>
        public static void Write(this NetBuffer message, BoundingSphere bounds) {
            message.Write(bounds.Center.X);
            message.Write(bounds.Center.Y);
            message.Write(bounds.Center.Z);
            message.Write(bounds.Radius);
        }

        /// <summary>
        /// Reads a bounding sphere written using Write(message, BoundingSphere)
        /// </summary>
        public static BoundingSphere ReadBoundingSphere(this NetBuffer message) {
            BoundingSphere retval;
            retval.Center.X = message.ReadSingle();
            retval.Center.Y = message.ReadSingle();
            retval.Center.Z = message.ReadSingle();
            retval.Radius = message.ReadSingle();
            return retval;
        }
        */
    }
}
#endif