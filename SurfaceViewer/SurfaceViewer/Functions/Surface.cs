using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Differentiation;
using Symbolic.Functions;
using Symbolic;

namespace SurfaceViewer.Functions
{
    class Surface
    {

        private Vector3 r, normal;

        public Surface(string xExpr, string yExpr, string zExpr, string u, string v, bool analytic)
        {
            var domain = ":" + u + ";" + v;
            var x = (CommonFunction)(xExpr + domain);
            var y = (CommonFunction)(yExpr + domain);
            var z = (CommonFunction)(zExpr + domain);
            r = new Vector3(x, y, z);
            if (analytic)
                normal = getNormalAnalitic(xExpr, yExpr, zExpr, u, v);
            else
                normal = getNormalNumeric(r);
        }

        private Vector3 getNormalAnalitic(string xExpr, string yExpr, string zExpr, string u, string v)
        {
            var d = ":" + u + ";" + v;

            var xonu = Derivative.GetDerivative(xExpr, u, false);
            var xonv = Derivative.GetDerivative(xExpr, v, false);
            var yonu = Derivative.GetDerivative(yExpr, u, false);
            var yonv = Derivative.GetDerivative(yExpr, v, false);
            var zonu = Derivative.GetDerivative(zExpr, u, false);
            var zonv = Derivative.GetDerivative(zExpr, v, false);

            var v1 = new Vector3(xonu + d, yonu + d, zonu + d);
            var v2 = new Vector3(xonv + d, yonv + d, zonv + d);

            var normalNotNormalized = (v1 ^ v2);
            var len = (CommonFunction)("sqrt(" + (normalNotNormalized * normalNotNormalized).Print() + ")" + d);
            return normalNotNormalized / len;
        }

        private Vector3 getNormalNumeric(Vector3 r)
        {
            List<DefinedCommonFunction> derivativesU = new List<DefinedCommonFunction>();
            List<DefinedCommonFunction> derivativesV = new List<DefinedCommonFunction>();
            NumericalDerivative derivative = new NumericalDerivative();
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(r.X.Invoke, 0, 1), "null"));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(r.X.Invoke, 1, 1), "null"));
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(r.Y.Invoke, 0, 1), "null"));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(r.Y.Invoke, 1, 1), "null"));
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(r.Z.Invoke, 0, 1), "null"));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(r.Z.Invoke, 1, 1), "null"));

            Vector3 v1 = new Vector3(derivativesU);
            Vector3 v2 = new Vector3(derivativesV);

            var normalNotNormalized = (v1 ^ v2);
            var len = new OneVarFunction(Math.Sqrt, normalNotNormalized * normalNotNormalized, null);
            return normalNotNormalized / len;
        }

        public float[] getVertex(double u, double v)
        {
            double[] result = r.InvokeVec(new double[] { u, v });
            return new float[] { (float)result[0], (float)result[1], (float)result[2] };
        }

        public float[] getNormal(double u, double v)
        {
            double[] result = normal.InvokeVec(new double[] { u, v });
            return new float[] { (float)result[0], (float)result[1], (float)result[2] };
        }
    }
}
