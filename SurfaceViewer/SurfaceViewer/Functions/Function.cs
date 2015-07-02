using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceViewer.Functions
{

    public interface Function                                               //интерфейс для всех функций
    {
        Func<double[], double[]> getFunction();                             //метод должен возвращать делегат на функцию, параметром которой является массив чисел, функция тоже возвращает массив чисел
    }

    public abstract class CommonFunction : Function                         //класс для обычной функции (не вектор-функции) нескольких переменных
    {

        public static CommonFunction operator +(CommonFunction f1, CommonFunction f2)
        {
            Sum result = new Sum();

            if (f1 is Sum)
            {
                result.operands = ((Sum)f1).operands;
                result.signs = ((Sum)f1).signs;
            }
            else
            {
                result.operands.Add(f1);
                result.signs.Add(true);
            }
            if (f2 is Sum)
            {
                result.operands.AddRange(((Sum)f2).operands);
                result.signs.AddRange(((Sum)f2).signs);
            }
            else
            {
                result.operands.Add(f2);
                result.signs.Add(true);
            }
            return result;
        }

        public static CommonFunction operator -(CommonFunction f1, CommonFunction f2)
        {
            return f1 + (-f2);
        }

        public static CommonFunction operator -(CommonFunction f1)
        {
            Sum result = new Sum();
            if (f1 is Sum)
            {
                result = (Sum)f1;
                for (int i = 0; i < result.signs.Count; i++)
                {
                    result.signs[i] = !result.signs[i];
                }
            }
            else
            {
                result.operands.Add(f1);
                result.signs.Add(false);
            }
            return result;
        }

        public static CommonFunction operator *(CommonFunction f1, CommonFunction f2)
        {
            Mul result = new Mul();
            if (f1 is Mul)
            {
                result.operands = ((Mul)f1).operands;
                result.powers = ((Mul)f1).powers;
            }
            else
            {
                result.operands.Add(f1);
                result.powers.Add(true);
            }
            if (f2 is Mul)
            {
                result.operands.AddRange(((Mul)f2).operands);
                result.powers.AddRange(((Mul)f2).powers);
            }
            else
            {
                result.operands.Add(f2);
                result.powers.Add(true);
            }
            return result;
        }

        public static CommonFunction operator /(CommonFunction f1, CommonFunction f2)
        {
            Mul result = new Mul();
            if (f1 is Mul)
            {
                result.operands = ((Mul)f1).operands;
                result.powers = ((Mul)f1).powers;
            }
            else
            {
                result.operands.Add(f1);
                result.powers.Add(true);
            }
            if (f2 is Mul)
            {
                result.operands.AddRange(((Mul)f2).operands);
                foreach (bool flag in ((Mul)f2).powers)
                {
                    result.powers.Add(!flag);
                }
            }
            else
            {
                result.operands.Add(f2);
                result.powers.Add(false);
            }
            return result;
        }

        public Pow pow(CommonFunction f1)
        {
            Pow result = new Pow();
            result.baseAndPower.Add(this);
            result.baseAndPower.Add(f1);
            return result;
        }

        public abstract Func<double[], double> getCommonFunction();         //метод, который должен возвращать делегат на обычную функцию

        public Func<double[], double[]> getFunction()                       //заглушка метода, обращение к методу выше
        {
            return p => new double[] { getCommonFunction()(p) };
        }
    }

    public class DefinedCommonFunction : CommonFunction
    {

        protected Func<double[], double> function;

        public DefinedCommonFunction(Func<double[], double> function)
        {
            this.function = function;
        }

        public override Func<double[], double> getCommonFunction()
        {
            return function;
        }
    }

    public class Length : CommonFunction
    {

        private VectorFunction arg;

        public Length(VectorFunction arg)
        {
            this.arg = arg;
        }

        static double length(double[] p)
        {
            double sum = 0;
            for (int i = 0; i < p.Length; i++)
            {
                sum += p[i] * p[i];
            }
            return Math.Sqrt(sum);
        }

        public override Func<double[], double> getCommonFunction()
        {
            return p => length(arg.getFunction()(p));
        }
    }

    public class Variable : CommonFunction                                  //переменная тоже являестя функцией нескольких переменных, возвращает она значение одой из переменных
    {

        public int index;

        public Variable(int index)
        {
            this.index = index;
        }

        public override Func<double[], double> getCommonFunction()
        {
            return p => p[index];
        }
    }

    public class OneVarFunction : CommonFunction                            //класс для обычной функции одного переменного
    {

        public CommonFunction arg;                                          //есть смысл создать поле аргумента, так как этот класс является конечным в данной ветке

        public Func<double, double> function = p => Double.NaN;             //делегат на функцию одного аргумента, стандартная инициализация - возврат неопределенного числа

        public OneVarFunction(Func<double, double> function, CommonFunction arg)
        {
            this.function = function;
            this.arg = arg;
        }

        public override Func<double[], double> getCommonFunction()          //заглушка метода, обращение к делегату с аргументом в виде запрошенного значения с поля arg
        {
            if (arg != null)
                return p => function(arg.getCommonFunction()(p));
            else
                return p => function(0);
        }
    }

    public class Sum : CommonFunction                                       //функция суммы нескольких элементов с разными знаками, элементы могут быть любыми функциями
    {

        public List<CommonFunction> operands;                                     //контейнер, который содержит элементы
        public List<bool> signs;                                            //знак (-,+) для каждого элемента

        public Sum()
        {
            operands = new List<CommonFunction>();
            signs = new List<bool>();
        }

        public Sum(List<CommonFunction> operands, List<bool> signs)
        {
            this.operands = operands;
            this.signs = signs;
        }

        private double func(double[] p)                                     //метод для подсчёта суммы
        {
            double result = 0;
            for (int i = 0; i < operands.Count; i++)
            {
                if (signs[i])
                {
                    result += operands[i].getFunction()(p)[0];
                }
                else
                {
                    result -= operands[i].getFunction()(p)[0];
                }
            }
            return result;
        }

        public override Func<double[], double> getCommonFunction()          //заглушка метода, обращается к методу выше
        {
            return func;
        }
    }
    public class Mul : CommonFunction                                       //произведение элементов с двумя разными степенями.
    {

        public List<CommonFunction> operands;                                     //аналог
        public List<bool> powers;                                           //массив степеней, означает либо -1 степень при произведении, либо 1 степень

        public Mul()
        {
            operands = new List<CommonFunction>();
            powers = new List<bool>();
        }

        public Mul(List<CommonFunction> operands, List<bool> powers)
        {
            this.operands = operands;
            this.powers = powers;
        }

        private double func(double[] p)                                     //аналог
        {
            double result = operands[0].getFunction()(p)[0];
            for (int i = 1; i < operands.Count; i++)
            {
                if (powers[i])
                {
                    result *= operands[i].getFunction()(p)[0];
                }
                else
                {
                    result /= operands[i].getFunction()(p)[0];
                }
            }
            return result;
        }

        public override Func<double[], double> getCommonFunction()          //аналог
        {
            return func;
        }
    }

    public class Pow : CommonFunction                                       //степень
    {
        public List<CommonFunction> baseAndPower;                                 //основание и показатели последовательных степеней

        public Pow() { baseAndPower = new List<CommonFunction>(); }

        public Pow(List<CommonFunction> baseAndPower)
        {
            this.baseAndPower = baseAndPower;
        }

        private double compute(double[] p)                                  //осуществляет математическую операцию подсчёта степени несколько раз
        {
            if (baseAndPower.Count == 1) return baseAndPower[0].getFunction()(p)[0];
            else
            {
                double result = baseAndPower[baseAndPower.Count - 1].getFunction()(p)[0];
                for (int i = baseAndPower.Count - 2; i >= 0; i--)
                {
                    result = Math.Pow(baseAndPower[i].getFunction()(p)[0], result);
                }
                return result;
            }
        }

        public override Func<double[], double> getCommonFunction()          //заглушка метода, обращается к методу выше
        {
            return p => compute(p);
        }
    }

    public class VectorFunction : Function
    {
        public readonly List<CommonFunction> components = new List<CommonFunction>();

        public VectorFunction(List<CommonFunction> components)
        {
            this.components = components;
        }

        public VectorFunction(List<DefinedCommonFunction> components)
        {
            this.components = new List<CommonFunction>();
            for (int i = 0; i < components.Count; i++)
            {
                this.components.Add(components[i]);
            }
        }

        protected double[] func(double[] p)
        {
            if (components.Count == 0)
            {
                return null;
            }
            double[] result = new double[components.Count];
            for (int i = 0; i < components.Count; i++)
            {
                result[i] = components[i].getCommonFunction()(p);
            }
            return result;
        }

        public Func<double[], double[]> getFunction()
        {
            return func;
        }
    }

    public class Curve : VectorFunction
    {

        public Curve(List<CommonFunction> trajectory) : base(trajectory) { }

        public Func<double, double[]> trajectory()
        {
            return t => func(new double[] { t });
        }
    }


    public class Vector3Mul : VectorFunction
    {
        public Vector3Mul(VectorFunction v1, VectorFunction v2)
            : base(new List<CommonFunction>())
        {
            List<CommonFunction> x = new List<CommonFunction>();
            List<bool> x_ = new List<bool>();
            List<CommonFunction> x1 = new List<CommonFunction>();
            List<bool> x1_ = new List<bool>();

            x1.Add(v1.components[1]);
            x1_.Add(true);
            x1.Add(v2.components[2]);
            x1_.Add(true);
            Mul x1O = new Mul(x1, x1_);
            x.Add(x1O);
            x_.Add(true);

            List<CommonFunction> x2 = new List<CommonFunction>();
            List<bool> x2_ = new List<bool>();
            x2.Add(v1.components[2]);
            x2_.Add(true);
            x2.Add(v2.components[1]);
            x2_.Add(true);
            Mul x2O = new Mul(x2, x2_);
            x.Add(x2O);
            x_.Add(false);

            components.Add(new Sum(x, x_));

            List<CommonFunction> y = new List<CommonFunction>();
            List<bool> y_ = new List<bool>();
            List<CommonFunction> y1 = new List<CommonFunction>();
            List<bool> y1_ = new List<bool>();

            y1.Add(v1.components[2]);
            y1_.Add(true);
            y1.Add(v2.components[0]);
            y1_.Add(true);
            Mul y1O = new Mul(y1, y1_);
            y.Add(y1O);
            y_.Add(true);

            List<CommonFunction> y2 = new List<CommonFunction>();
            List<bool> y2_ = new List<bool>();
            y2.Add(v1.components[0]);
            y2_.Add(true);
            y2.Add(v2.components[2]);
            y2_.Add(true);
            Mul y2O = new Mul(y2, y2_);
            y.Add(y2O);
            y_.Add(false);

            components.Add(new Sum(y, y_));

            List<CommonFunction> z = new List<CommonFunction>();
            List<bool> z_ = new List<bool>();
            List<CommonFunction> z1 = new List<CommonFunction>();
            List<bool> z1_ = new List<bool>();

            z1.Add(v1.components[0]);
            z1_.Add(true);
            z1.Add(v2.components[1]);
            z1_.Add(true);
            Mul z1O = new Mul(z1, z1_);
            z.Add(z1O);
            z_.Add(true);

            List<CommonFunction> z2 = new List<CommonFunction>();
            List<bool> z2_ = new List<bool>();
            z2.Add(v1.components[1]);
            z2_.Add(true);
            z2.Add(v2.components[0]);
            z2_.Add(true);
            Mul z2O = new Mul(z2, z2_);
            z.Add(z2O);
            z_.Add(false);

            components.Add(new Sum(z, z_));
        }
    }
}
