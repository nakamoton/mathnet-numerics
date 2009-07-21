﻿// <copyright file="NevillePolynomialInterpolation.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2009 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.Interpolation.Algorithms
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Lagrange Polynomial Interpolation using Neville's Algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This algorithm supports differentiation, but doesn't support integration.
    /// </para>
    /// <para>
    /// When working with equidistant or Chebyshev sample points it is
    /// recommended to use the barycentric algorithms specialized for
    /// these cases instead of this arbitrary Neville algorithm.
    /// </para>
    /// </remarks>
    public class NevillePolynomialInterpolation : IInterpolation
    {
        /// <summary>
        /// Sample Points t.
        /// </summary>
        private IList<double> points;

        /// <summary>
        /// Spline Values x(t).
        /// </summary>
        private IList<double> values;

        /// <summary>
        /// Initializes a new instance of the NevillePolynomialInterpolation class.
        /// </summary>
        public NevillePolynomialInterpolation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NevillePolynomialInterpolation class.
        /// </summary>
        /// <param name="samplePoints">Sample Points t</param>
        /// <param name="sampleValues">Sample Values x(t)</param>
        public NevillePolynomialInterpolation(
            IList<double> samplePoints,
            IList<double> sampleValues)
        {
            this.Initialize(samplePoints, sampleValues);
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm supports differentiation (interpolated derivative).
        /// </summary>
        /// <seealso cref="Differentiate(double)"/>
        /// <seealso cref="Differentiate(double, out double, out double)"/>
        bool IInterpolation.SupportsDifferentiation
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm supports integration (interpolated quadrature).
        /// </summary>
        /// <seealso cref="Integrate"/>
        bool IInterpolation.SupportsIntegration
        {
            get { return false; }
        }

        /// <summary>
        /// Initialize the interpolation method with the given spline coefficients.
        /// </summary>
        /// <param name="samplePoints">Sample Points t</param>
        /// <param name="sampleValues">Sample Values x(t)</param>
        public void Initialize(
            IList<double> samplePoints,
            IList<double> sampleValues)
        {
            if (null == samplePoints)
            {
                throw new ArgumentNullException("samplePoints");
            }

            if (null == sampleValues)
            {
                throw new ArgumentNullException("sampleValues");
            }

            if (samplePoints.Count != sampleValues.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths);
            }

            this.points = samplePoints;
            this.values = sampleValues;
        }

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated value x(t).</returns>
        public double Interpolate(double t)
        {
            double[] x = new double[this.values.Count];
            this.values.CopyTo(x, 0);

            for (int level = 1; level < x.Length; level++)
            {
                for (int i = 0; i < x.Length - level; i++)
                {
                    double hp = t - this.points[i + level];
                    double ho = this.points[i] - t;
                    double den = this.points[i] - this.points[i + level];
                    x[i] = ((hp * x[i]) + (ho * x[i + 1])) / den;
                }
            }

            return x[0];
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated first derivative at point t.</returns>
        /// <seealso cref="IInterpolation.SupportsDifferentiation"/>
        /// <seealso cref="Differentiate(double, out double, out double)"/>
        public double Differentiate(double t)
        {
            double[] x = new double[this.values.Count];
            double[] dx = new double[this.values.Count];
            this.values.CopyTo(x, 0);

            for (int level = 1; level < x.Length; level++)
            {
                for (int i = 0; i < x.Length - level; i++)
                {
                    double hp = t - this.points[i + level];
                    double ho = this.points[i] - t;
                    double den = this.points[i] - this.points[i + level];
                    dx[i] = ((hp * dx[i]) + x[i] + (ho * dx[i + 1]) - x[i + 1]) / den;
                    x[i] = ((hp * x[i]) + (ho * x[i + 1])) / den;
                }
            }

            return dx[0];
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <param name="interpolatedValue">Interpolated value x(t)</param>
        /// <param name="secondDerivative">Interpolated second derivative at point t.</param>
        /// <returns>Interpolated first derivative at point t.</returns>
        /// <seealso cref="IInterpolation.SupportsDifferentiation"/>
        /// <seealso cref="Differentiate(double)"/>
        public double Differentiate(
            double t,
            out double interpolatedValue,
            out double secondDerivative)
        {
            double[] x = new double[this.values.Count];
            double[] dx = new double[this.values.Count];
            double[] d2x = new double[this.values.Count];
            this.values.CopyTo(x, 0);

            for (int level = 1; level < x.Length; level++)
            {
                for (int i = 0; i < x.Length - level; i++)
                {
                    double hp = t - this.points[i + level];
                    double ho = this.points[i] - t;
                    double den = this.points[i] - this.points[i + level];
                    d2x[i] = ((hp * d2x[i]) + (ho * d2x[i + 1]) + (2 * dx[i]) - (2 * dx[i + 1])) / den;
                    dx[i] = ((hp * dx[i]) + x[i] + (ho * dx[i + 1]) - x[i + 1]) / den;
                    x[i] = ((hp * x[i]) + (ho * x[i + 1])) / den;
                }
            }

            interpolatedValue = x[0];
            secondDerivative = d2x[0];
            return dx[0];
        }

        /// <summary>
        /// Integrate up to point t.
        /// </summary>
        /// <param name="t">Right bound of the integration interval [a,t].</param>
        /// <returns>Interpolated definite integral over the interval [a,t].</returns>
        /// <seealso cref="IInterpolation.SupportsIntegration"/>
        double IInterpolation.Integrate(double t)
        {
            throw new NotSupportedException();
        }
    }
}