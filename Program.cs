using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ShellGameClient
{
    static void Main()
    {
        using (TcpClient client = new TcpClient("127.0.0.1", 6000))
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string startMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            int ballPosition = int.Parse(startMsg.Split(' ')[1]);

            Console.WriteLine($"Шарик изначально под наперстком {ballPosition}.");
            Console.WriteLine("Перемешивание начинается...");

            // Чтение команд перемешивания
            for (int i = 0; i < 10; i++)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string swapMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] parts = swapMsg.Split(' ');

                if (parts[0] == "SWAP")
                {
                    int a = int.Parse(parts[1]);
                    int b = int.Parse(parts[2]);
                    Console.WriteLine($"Меняем местами {a} и {b}.");
                }

                Thread.Sleep(500);
            }

            // Запрос выбора наперстка
            Console.Write("Введите номер наперстка (0, 1 или 2): ");
            int choice = int.Parse(Console.ReadLine());

            byte[] guessMsg = Encoding.UTF8.GetBytes($"GUESS {choice}");
            stream.Write(guessMsg, 0, guessMsg.Length);

            // Чтение результата
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string resultMsg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (resultMsg.StartsWith("WIN"))
            {
                Console.WriteLine("Поздравляем! Вы угадали.");
            }
            else
            {
                int correct = int.Parse(resultMsg.Split(' ')[1]);
                Console.WriteLine($"Вы не угадали. Шарик был под наперстком {correct}.");
            }
        }
    }
}
