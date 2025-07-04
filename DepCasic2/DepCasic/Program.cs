using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DepCasic
{
    internal class Program
    {
        public static int n;
        public static int[] chips;
        public static int total;
        public static long moves;
        public static long totalMoves;
        static void Main()
        {
            Console.Write("Введите количество игроков за круглым столом: ");
            n = int.Parse(Console.ReadLine());

            Console.WriteLine($"Введите {n} чисел — количество фишек у каждого игрока:");
            chips = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            total = 0;
            foreach (int chip in chips)
            {
                total += chip;
            }

            if (total % n != 0)
            {
                Console.WriteLine("Невозможно равномерно распределить фишки.");
                return;
            }
            
            Small();
            Big();

            if (moves > totalMoves)
            {
                Console.WriteLine($"\nМинимальное количество ходов: {totalMoves}");
            }
            else
            {
                Console.WriteLine($"Минимальное количество ходов: {moves}");
            }
            
           
        }

        static void Small()
        {
            int avg = total / n;
            moves = 0;
            long diff = 0;

            for (int i = 0; i < n; i++)
            {
                diff += chips[i] - avg;
                moves += Math.Abs(diff);
                
            }
        }
        
        static void Big()
        {
            int avg = total / n;

            // Создаем списки избытка и недостатка
            List<(int index, int amount)> excess = new List<(int, int)>();
            List<(int index, int amount)> deficit = new List<(int, int)>();

            for (int i = 0; i < n; i++)
            {
                int diff = chips[i] - avg;
                if (diff > 0)
                    excess.Add((i, diff));
                else if (diff < 0)
                    deficit.Add((i, -diff)); // храним положительное значение
            }

            totalMoves = 0;
            List<string> log = new List<string>(); // для записи действий

            while (excess.Count > 0 && deficit.Count > 0)
            {
                // Ищем пару с минимальным расстоянием
                int minDist = int.MaxValue;
                int ei = -1, di = -1;
                bool clockwise = false;

                for (int e = 0; e < excess.Count; e++)
                {
                    var (eIndex, eAmount) = excess[e];
                    if (eAmount <= 0) continue;

                    for (int d = 0; d < deficit.Count; d++)
                    {
                        var (dIndex, dAmount) = deficit[d];
                        if (dAmount <= 0) continue;

                        int distCW = (dIndex - eIndex + n) % n;
                        int distCCW = (eIndex - dIndex + n) % n;
                        int currMin = Math.Min(distCW, distCCW);
                        if (currMin < minDist)
                        {
                            minDist = currMin;
                            ei = e;
                            di = d;
                            clockwise = distCW <= distCCW;
                        }
                    }
                }

                if (ei == -1 || di == -1) break;

                var (eIdx, eAmt) = excess[ei];
                var (dIdx, dAmt) = deficit[di];

                int transfer = Math.Min(eAmt, dAmt);

                totalMoves += (long)transfer * minDist;

                string direction = clockwise ? "по часовой стрелке" : "против часовой";
                log.Add($"Игрок {eIdx} → Игрок {dIdx}: передано {transfer} фишек за {minDist} шагов ({direction})");

                // Обновляем списки
                if (eAmt == transfer)
                    excess.RemoveAt(ei);
                else
                    excess[ei] = (eIdx, eAmt - transfer);

                if (dAmt == transfer)
                    deficit.RemoveAt(di);
                else
                    deficit[di] = (dIdx, dAmt - transfer);
            }

        }
    }
}
