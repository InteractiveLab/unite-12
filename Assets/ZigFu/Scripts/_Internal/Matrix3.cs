using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class Matrix3
{
    float[,] matrix;

    public Matrix3()
    {
        matrix = new float[3, 3];
    }

    public Matrix3(float[,] matrix)
    {
        this.matrix = matrix; //TODO: clone instead of point to
    }

    public static Matrix3 I()
    {
        return new Matrix3(new float[,] { 
        { 1.0f, 0.0f, 0.0f }, 
        { 0.0f, 1.0f, 0.0f }, 
        { 0.0f, 0.0f, 1.0f } });
    }

    public static Vector3 operator *(Matrix3 matrix3, Vector3 v)
    {
        float[,] m = matrix3.matrix;
        return new Vector3(
            m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z,
            m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z,
            m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z);
    }

    public static Matrix3 operator *(Matrix3 mat1, Matrix3 mat2)
    {
        float[,] m1 = mat1.matrix;
        float[,] m2 = mat2.matrix;
        float[,] m3 = new float[3, 3];
        m3[0, 0] = m1[0, 0] * m2[0, 0] + m1[0, 1] * m2[1, 0] + m1[0, 2] * m2[2, 0];
        m3[0, 1] = m1[0, 0] * m2[0, 1] + m1[0, 1] * m2[1, 1] + m1[0, 2] * m2[2, 1];
        m3[0, 2] = m1[0, 0] * m2[0, 2] + m1[0, 1] * m2[1, 2] + m1[0, 2] * m2[2, 2];
        m3[1, 0] = m1[1, 0] * m2[0, 0] + m1[1, 1] * m2[1, 0] + m1[1, 2] * m2[2, 0];
        m3[1, 1] = m1[1, 0] * m2[0, 1] + m1[1, 1] * m2[1, 1] + m1[1, 2] * m2[2, 1];
        m3[1, 2] = m1[1, 0] * m2[0, 2] + m1[1, 1] * m2[1, 2] + m1[1, 2] * m2[2, 2];
        m3[2, 0] = m1[2, 0] * m2[0, 0] + m1[2, 1] * m2[1, 0] + m1[2, 2] * m2[2, 0];
        m3[2, 1] = m1[2, 0] * m2[0, 1] + m1[2, 1] * m2[1, 1] + m1[2, 2] * m2[2, 1];
        m3[2, 2] = m1[2, 0] * m2[0, 2] + m1[2, 1] * m2[1, 2] + m1[2, 2] * m2[2, 2];
        return new Matrix3(m3);
    }

    public static Matrix3 operator *(Matrix3 m, float scalar)
    {
        float[,] m1 = m.matrix;
        float[,] m2 = new float[3, 3];
        m2[0, 0] = m1[0, 0] * scalar;
        m2[0, 1] = m1[0, 1] * scalar;
        m2[0, 2] = m1[0, 2] * scalar;
        m2[1, 0] = m1[1, 0] * scalar;
        m2[1, 1] = m1[1, 1] * scalar;
        m2[1, 2] = m1[1, 2] * scalar;
        m2[2, 0] = m1[2, 0] * scalar;
        m2[2, 1] = m1[2, 1] * scalar;
        m2[2, 2] = m1[2, 2] * scalar;
        return new Matrix3(m2);
    }
    public static Matrix3 operator +(Matrix3 first, Matrix3 second)
    {
        float[,] m1 = first.matrix;
        float[,] m2 = second.matrix;
        float[,] m3 = new float[3, 3];
        m3[0, 0] = m1[0, 0] + m2[0, 0];
        m3[0, 1] = m1[0, 1] + m2[0, 1];
        m3[0, 2] = m1[0, 2] + m2[0, 2];
        m3[1, 0] = m1[1, 0] + m2[1, 0];
        m3[1, 1] = m1[1, 1] + m2[1, 1];
        m3[1, 2] = m1[1, 2] + m2[1, 2];
        m3[2, 0] = m1[2, 0] + m2[2, 0];
        m3[2, 1] = m1[2, 1] + m2[2, 1];
        m3[2, 2] = m1[2, 2] + m2[2, 2];
        return new Matrix3(m3);
    }
    public static Matrix3 operator -(Matrix3 first, Matrix3 second)
    {
        float[,] m1 = first.matrix;
        float[,] m2 = second.matrix;
        float[,] m3 = new float[3, 3];
        m3[0, 0] = m1[0, 0] - m2[0, 0];
        m3[0, 1] = m1[0, 1] - m2[0, 1];
        m3[0, 2] = m1[0, 2] - m2[0, 2];
        m3[1, 0] = m1[1, 0] - m2[1, 0];
        m3[1, 1] = m1[1, 1] - m2[1, 1];
        m3[1, 2] = m1[1, 2] - m2[1, 2];
        m3[2, 0] = m1[2, 0] - m2[2, 0];
        m3[2, 1] = m1[2, 1] - m2[2, 1];
        m3[2, 2] = m1[2, 2] - m2[2, 2];
        return new Matrix3(m3);
    }
    public float trace()
    {
        return matrix[0, 0] + matrix[1, 1] + matrix[2, 2];
    }
    public float determinant()
    {
        // straight-forward calculation
        return (matrix[0, 0] * matrix[1, 1] * matrix[2, 2]) +
               (matrix[1, 0] * matrix[2, 1] * matrix[0, 2]) +
               (matrix[2, 0] * matrix[0, 1] * matrix[1, 2]) -
               (matrix[2, 0] * matrix[1, 1] * matrix[0, 2]) -
               (matrix[1, 0] * matrix[0, 1] * matrix[2, 2]) -
               (matrix[0, 0] * matrix[2, 1] * matrix[1, 2]);
    }
    public float sumCells()
    {
        return matrix[0, 0] + matrix[0, 1] + matrix[0, 2] +
               matrix[1, 0] + matrix[1, 1] + matrix[1, 2] +
               matrix[2, 0] + matrix[2, 1] + matrix[2, 2];
    }
}