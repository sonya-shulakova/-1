using System;
using System.IO;

class Program
{
    static int[,] inputData; // Массив для хранения исходных данных
    static int m; // Количество блоков
    static int n; // Количество предприятий

    static void Main(string[] args)
    {
        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Формирование исходных данных");
            Console.WriteLine("2. Вывод исходных данных на экран");
            Console.WriteLine("3. Решение задачи методом перебора");
            Console.WriteLine("4. Решение задачи жадным алгоритмом");
            Console.WriteLine("5. Вывод результатов решения задачи");
            Console.WriteLine("6. Сохранение исходных данных в файл");
            Console.WriteLine("7. Восстановление исходных данных с файла");
            Console.WriteLine("0. Выход");

            Console.Write("Выберите пункт меню: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    GenerateInputData();
                    break;
                case "2":
                    DisplayInputData();
                    break;
                case "3":
                    SolveUsingBruteForce();
                    break;
                case "4":
                    SolveUsingGreedyAlgorithm();
                    break;
                case "5":
                    DisplaySolutionResults();
                    break;
                case "6":
                    SaveInputDataToFile();
                    break;
                case "7":
                    RestoreInputDataFromFile();
                    break;
                case "0":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                    break;
            }
        }
    }

    static void GenerateInputData()
    {
        Console.WriteLine("Выберите способ формирования исходных данных:");
        Console.WriteLine("1. Вручную");
        Console.WriteLine("2. Случайным образом");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                GenerateInputDataManual();
                break;
            case "2":
                GenerateInputDataRandom();
                break;
            default:
                Console.WriteLine("Некорректный ввод. Используются случайные данные.");
                GenerateInputDataRandom();
                break;
        }
    }

    static void GenerateInputDataManual()
    {
        Console.Write("Введите количество блоков (m): ");
        m = int.Parse(Console.ReadLine());
        Console.Write("Введите количество предприятий (n): ");
        n = int.Parse(Console.ReadLine());

        // Проверяем условие: количество блоков не может быть больше, чем количество предприятий
        if (m > n)
        {
            Console.WriteLine("Ошибка: количество блоков не может быть больше, чем количество предприятий.");
            return;
        }


        inputData = new int[m, n];

        Console.WriteLine("Введите исходные данные:");

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write($"Введите значение для блока {i + 1} и предприятия {j + 1}: ");
                inputData[i, j] = int.Parse(Console.ReadLine());
            }
        }
    }

    static void GenerateInputDataRandom()
    {
        Console.Write("Введите количество блоков (m): ");
        m = int.Parse(Console.ReadLine());
        Console.Write("Введите количество предприятий (n): ");
        n = int.Parse(Console.ReadLine());

        // Проверяем условие: количество блоков не может быть больше, чем количество предприятий
        if (m > n)
        {
            Console.WriteLine("Ошибка: количество блоков не может быть больше, чем количество предприятий.");
            return;
        }


        inputData = new int[m, n];

        Random random = new Random();
        Console.WriteLine("Случайные исходные данные:");

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                inputData[i, j] = random.Next(1, 20); // Пример случайного числа от 1 до 20
                Console.Write(inputData[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }

    static void DisplayInputData()
    {
        Console.WriteLine("Исходные данные:");
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(inputData[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }

    static void SolveUsingBruteForce()
    {
        int[] bestAssignment = null;
        int minCost = int.MaxValue;

        // Простой перебор всех возможных вариантов
        for (int i = 0; i < Math.Pow(n, m); i++)
        {
            int[] assignment = GetAssignment(i);

            //каждое предприятие должно изготавливать только один тип блоков
            bool isValid = true;
            for (int j = 0; j < m; j++)
            {
                int count = 0;
                for (int k = 0; k < n; k++)
                {
                    if (assignment[k] == j)
                        count++;
                }
                if (count != 1)
                {
                    isValid = false;
                    break;
                }
            }

            //каждый тип блока должен выполняться только одним предприятием
            if (isValid)
            {
                bool isValid2 = true;
                for (int k = 0; k < n; k++)
                {
                    int count = 0;
                    for (int j = 0; j < m; j++)
                    {
                        if (assignment[k] == j)
                            count++;
                    }
                    if (count != 1)
                    {
                        isValid2 = false;
                        break;
                    }
                }

                if (isValid2)
                {
                    int cost = CalculateCost(assignment);
                    if (cost < minCost)
                    {
                        minCost = cost;
                        bestAssignment = assignment;
                    }
                }
            }
        }


        Console.WriteLine("Лучшее распределение (метод перебора):");
        for (int i = 0; i < m; i++)
        {
            Console.WriteLine($"Блок {i + 1} -> Предприятие {bestAssignment[i] + 1}");
        }
        Console.WriteLine($"Надежность: {minCost}");
    }

    static void SolveUsingGreedyAlgorithm()
    {
        int[] bestAssignment = new int[m]; // Хранит присвоение блоков предприятиям
        int[] assignedBlocks = new int[n]; // Хранит количество уже назначенных блоков для каждого предприятия
        int[] minCostBlock = new int[m]; // Хранит минимальную надежность производства для каждого блока

        // Инициализация массива минимальных надежностей производства для каждого блока
        for (int i = 0; i < m; i++)
        {
            minCostBlock[i] = int.MaxValue;
        }

        // Проходим по каждому блоку
        for (int i = 0; i < m; i++)
        {
            // Выбираем наименьшую надежность производства для данного блока
            for (int j = 0; j < n; j++)
            {
                if (inputData[i, j] < minCostBlock[i] && assignedBlocks[j] == 0)
                {
                    minCostBlock[i] = inputData[i, j];
                    bestAssignment[i] = j; // Присваиваем блоку предприятие с минимальной надежностью
                }
            }

            if (assignedBlocks[bestAssignment[i]] == 0)
            {
                assignedBlocks[bestAssignment[i]]++; // Увеличиваем количество назначенных блоков для выбранного предприятия
            }
            else
            {
                // Предприятие уже производит блок, выбираем следующее с наименьшей стоимостью
                for (int j = 0; j < n; j++)
                {
                    if (assignedBlocks[j] == 0)
                    {
                        minCostBlock[i] = inputData[i, j];
                        bestAssignment[i] = j;
                        assignedBlocks[j]++;
                        break;
                    }
                }
            }
        }

        // Выводим результаты жадного алгоритма
        Console.WriteLine("Лучшее распределение (жадный алгоритм):");
        for (int i = 0; i < m; i++)
        {
            Console.WriteLine($"Блок {i + 1} -> Предприятие {bestAssignment[i] + 1}");
        }
        Console.WriteLine($"Надежность: {CalculateCost(bestAssignment)}");
    }

    

    static int[] GetAssignment(int number)
    {
        int[] assignment = new int[m];
        for (int i = 0; i < m; i++)
        {
            assignment[i] = number % n;
            number /= n;
        }
        return assignment;
    }

    static int CalculateCost(int[] assignment)
    {
        int cost = 0;
        for (int i = 0; i < m; i++)
        {
            cost += inputData[i, assignment[i]];
        }
        return cost;
    }

    static void DisplaySolutionResults()
    {
        Console.WriteLine("Результаты решения задачи:");
        if (inputData == null)
        {
            Console.WriteLine("Исходные данные не сгенерированы.");
            return;
        }

        SolveUsingBruteForce(); // Для сравнения выведем результаты метода перебора также

        SolveUsingGreedyAlgorithm(); // Выводим результаты жадного алгоритма
    }

    static void SaveInputDataToFile()
    {
        using (StreamWriter writer = new StreamWriter("input_data.txt"))
        {
            writer.WriteLine($"{m} {n}");
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    writer.Write(inputData[i, j] + " ");
                }
                writer.WriteLine();
            }
        }
        Console.WriteLine("Исходные данные успешно сохранены в файл input_data.txt");
    }

    static void RestoreInputDataFromFile()
    {
        using (StreamReader reader = new StreamReader("input_data.txt"))
        {
            string[] dimensions = reader.ReadLine().Split(' ');
            m = int.Parse(dimensions[0]);
            n = int.Parse(dimensions[1]);
            inputData = new int[m, n];
            for (int i = 0; i < m; i++)
            {
                string[] line = reader.ReadLine().Split(' ');
                for (int j = 0; j < n; j++)
                {
                    inputData[i, j] = int.Parse(line[j]);
                }
            }
        }
        Console.WriteLine("Исходные данные успешно восстановлены из файла input_data.txt");
    }
}