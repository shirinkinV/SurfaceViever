using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationUsingOpenGLAndWPF.Parsing
{

    public interface Function                                              //интерфейс для всех функций
    {
        Func<double[], double[]> getFunction();              //метод должен возвращать делегат на функцию, параметром которой является массив чисел, функция тоже возвращает массив чисел
    }

    public abstract class CommonFunction : Function                   //класс для обычной функции (не вектор-функции) нескольких переменных
    {

        public abstract Func<double[], double> getCommonFunction(); //метод, который должен возвращать делегат на обычную функцию

        public Func<double[], double[]> getFunction()               //заглушка метода, обращение к методу выше
        {
            return p => new double[] { getCommonFunction()(p) };
        }
    }

    public class Variable : CommonFunction                          //переменная тоже являестя функцией нескольких переменных, возвращает она значение одой из переменных
    {

        public int index;
        
        public override Func<double[], double> getCommonFunction()
        {
            return p => p[index];
        }
    }

    public class OneVarFunction : CommonFunction                    //класс для обычной функции одного переменного
    {

        public Function arg;                                        //есть смысл создать поле аргумента, так как этот класс является конечным в данной ветке

        public Func<double, double> function = p => Double.NaN;     //делегат на функцию одного аргумента, стандартная инициализация - возврат неопределенного числа

        public override Func<double[], double> getCommonFunction()  //заглушка метода, обращение к делегату с аргументом в виде запрошенного значения с поля arg
        {
            if (arg != null)
                return p => function(arg.getFunction()(p)[0]);
            else
                return p => function(0);
        }
    }

    public class Sum : CommonFunction                               //функция суммы нескольких элементов с разными знаками, элементы могут быть любыми функциями
    {

        public List<Function> operands = new List<Function>();      //контейнер, который содержит элементы
        public List<bool> signs = new List<bool>();                 //знак (-,+) для каждого элемента

        private double func(double[] p)                             //метод для подсчёта суммы
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

        public override Func<double[], double> getCommonFunction()  //заглушка метода, обращается к методу выше
        {
            return func;
        }
    }
    public class Mul : CommonFunction                               //произведение элементов с двумя разными степенями.
    {

        public List<Function> operands = new List<Function>();      //аналог
        public List<bool> powers = new List<bool>();                //массив степеней, означает либо -1 степень при произведении, либо 1 степень

        private double func(double[] p)                             //аналог
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

        public override Func<double[], double> getCommonFunction()  //аналог
        {
            return func;
        }
    }

    public class Pow : CommonFunction                               //степень
    {
        public List<Function> baseAndPower=new List<Function>();                         //основание и показатели последовательных степеней

        private double compute(double[] p)                          //осуществляет математическую операцию подсчёта степени несколько раз
        {
            if (baseAndPower.Count == 1) return baseAndPower[0].getFunction()(p)[0];
            else
            {
                double result = baseAndPower[baseAndPower.Count-1].getFunction()(p)[0];
                for (int i = baseAndPower.Count-2; i >=0; i--)
                {
                    result= Math.Pow(baseAndPower[i].getFunction()(p)[0],result);
                }
                return result;
            }
        }

        public override Func<double[], double> getCommonFunction()  //заглушка метода, обращается к методу выше
        {
            return p => compute(p);
        }
    }

}
