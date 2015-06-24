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

        private double left, right, top, bottom;

        public Surface(VectorFunction r, double left, double right, double bottom, double top, double seed)
        {
            this.r = r;
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;

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
    }
}
