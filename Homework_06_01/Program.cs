using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Homework_06_01
{
    class Program
    {
        /// <summary>
        /// Метод, проверяющий верность введеного пользователем значения при выборе ответа на вопрос
        /// </summary>
        /// <param name="value">Ответ</param>
        /// <param name="flag">Флаг проверки</param>
        /// <returns>Вариант выбора</returns>
        static int CheckValue(string value, out bool flag)
        {
            flag = int.TryParse(value, out int number);                     //Попытка преобразования строчного значения в целое число
            return number;
        }
        /// <summary>
        /// Метод, считывающий выбор пользователя до тех пор, пока выбор не будет корректным и возвращающий значение
        /// </summary>
        /// <returns>Значение выбора</returns>
        static int CheckChoice()
        {
            int choice = new int();
            bool flag = false;                                              //Введение переменной, управляющей циклом ввода данных

            while (!flag)                                                   //Цикл ввода данных
            {
                flag = true;                                                //Флаг окончания цикла
                choice = CheckValue(Console.ReadLine(), out flag);

                if ((choice != 1 && choice != 2))                           //Проверка верно введеных данных
                {
                    Console.Clear();
                    Console.Write("Сделайте корректный выбор!\n");
                    flag = false;                                           //Флаг продолжения цикла
                }
            }
            return choice;
        }
        /// <summary>
        /// Метод, проверяющий корректность введеного в документ значения
        /// </summary>
        /// <returns>Число</returns>
        static int CheckNumber()
        {
            int n = new int();
            string m;
            bool flag = false;

            while (!flag)                                                   //Цикл проверки введеного значения
            {
                flag = true;
                FileInfo fi = new FileInfo("N.txt");

                if (!fi.Exists) fi.Create().Close();

                using (StreamReader sr = new StreamReader("N.txt"))         //Считывание значения
                    m = sr.ReadLine();

                n = CheckValue(m, out flag);

                if (n < 1 || n > 1000000)                                   //Условие попадания в диапазон
                {
                    Console.Clear();
                    Console.WriteLine("Введите в документ N.txt корректное значение!");

                    using (StreamWriter sw = new StreamWriter("N.txt"))
                        sw.WriteLine(Console.ReadLine());

                    flag = false;
                }
            }
            return n;
        }
        /// <summary>
        /// Метод, вычисляющий количество групп, в которых будут записаны числа из диапазона
        /// </summary>
        /// <param name="n">Диапазон</param>
        /// <returns>Количество групп</returns>
        static int NumberOfGroups(int n)
        {
            int m = 0;

            while (Math.Pow(2, m) <= n)                                     //Цикл подборки количества групп при условии, что первое
                m += 1;                                                     //число каждого ряда 2 в степени n

            return m;
        }
        /// <summary>
        /// Метод, увеличивающий размер массива, при превышении в процессе заполнения
        /// </summary>
        /// <param name="numbers">Массив</param>
        /// <returns>Массив</returns>
        static int[] Resize(int[] numbers)
        {
            Array.Resize(ref numbers, numbers.Length * 2);
            return numbers;
        }
        /// <summary>
        /// Метод, заполняющий массив строк диапазоном чисел по группам в зависимости от нахождения делителей чисел
        /// </summary>
        /// <param name="str">Массив строк</param>
        /// <param name="n">Значение диапазона чисел</param>
        /// <returns>Заполненный массив строк</returns>
        static int[][] StringConstraction (int[][] numbers, int n)
        {
            int i = 0;
            int j = 0;
            int a = 1;

            while (a <= n)
            {
                if (a == Math.Pow(2, i + 1))
                {
                    i += 1;
                    j = 0;
                }

                if (j >= numbers[i].Length) numbers[i] = Resize(numbers[i]);

                numbers[i][j] = a;
                a += 1;
                j += 1;
            }
            return numbers;
        }
        /// <summary>
        /// Метод считывания решения пользователя о архивации данных
        /// </summary>
        /// <returns>Флаг решения</returns>
        static bool ZipSolution()
        {
            bool flag = false;
            string s = Console.ReadLine();

            if (s == "Y" || s == "y" || s == "Д" || s == "д")               //Условия принятия решения об архивации данных
                flag = true;

            return flag;
        }
        static void Main(string[] args)
        {
            int N;
            N = CheckNumber();

            Console.Write("Выберите режим работы программы:\n" +
                "1 Вычислить только количество групп.\n" +
                "2 Вычислить результат группирования.\n");

            int Choice = CheckChoice();
            DateTime date = DateTime.Now;
            int M = NumberOfGroups(N);
            int[][] Numbers = new int[M][];
            int i = 0;

            while (i < M)
            {
                Numbers[i] = new int[1];
                i += 1;
            }

            if (Choice == 1) Console.WriteLine("Количество групп равно: " + M);
            else Numbers = StringConstraction(Numbers, N);

            TimeSpan timeSpan = DateTime.Now.Subtract(date);
            Console.WriteLine($"Время, затраченное на операцию = {timeSpan.TotalMilliseconds}мс");
            Console.WriteLine("Желаете ли Вы заархивировать полученные данные? (Y/N)");
            bool Flag = ZipSolution();

            if (Flag)
            {
                string source = "result.txt";
                string compressed = "result.zip";
                using (StreamWriter sw = new StreamWriter(source))
                {
                    i = 0;
                    int j = 0;

                    while (i < Numbers.Length)
                    {
                        while (j < Numbers[i].Length)
                        {
                            sw.Write(Numbers[i][j]+" ");
                            j += 1;
                        }
                        j = 0;
                        i += 1;
                        sw.WriteLine();
                    }
                }
                using (FileStream ss = new FileStream(source, FileMode.OpenOrCreate))
                {
                    using (FileStream cs = File.Create(compressed))
                    {
                        using (GZipStream gs = new GZipStream(cs, CompressionMode.Compress))
                        {
                            ss.CopyTo(gs);
                        }
                    }
                }
            }
        }
    }
}