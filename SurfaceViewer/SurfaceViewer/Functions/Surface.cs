using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Differentiation;

namespace SurfaceViewer.Functions
{
    class Surface
    {

        private VectorFunction r, normal;

        public Surface(VectorFunction r)
        {
            this.r = r;

            List<CommonFunction> components = r.components;
            List<DefinedCommonFunction> derivativesU = new List<DefinedCommonFunction>();
            List<DefinedCommonFunction> derivativesV = new List<DefinedCommonFunction>();
            NumericalDerivative derivative = new NumericalDerivative();
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[0].getCommonFunction(), 0, 1)));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[0].getCommonFunction(), 1, 1)));
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[1].getCommonFunction(), 0, 1)));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[1].getCommonFunction(), 1, 1)));
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[2].getCommonFunction(), 0, 1)));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[2].getCommonFunction(), 1, 1)));

            VectorFunction v1 = new VectorFunction(derivativesU);
            VectorFunction v2 = new VectorFunction(derivativesV);

            normal = new Vector3Mul(v1, v2);
        }

        public float[] getVertex(double u, double v)
        {
            double[] result=r.getFunction()(new double[] { u, v });
            return new float[] { (float)result[0], (float)result[1], (float)result[2] };
        }

        public float[] getNormal(double u, double v)
        {
            double[] result = normal.getFunction()(new double[] { u, v });
            return new float[] { (float)result[0], (float)result[1], (float)result[2] };
        }

        public int[] getColor(double u, double v)
        {
            float[] colors = getNormal(u, v);
            return new int[] { (int)(255f * colors[0]), (int)(255f * colors[1]), (int)(255f * colors[2]) };
        }
    }
}
