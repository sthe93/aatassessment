using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    private static List<int> globalList = new List<int>();
    private static int totalCount = 0;
    private static object lockObject = new object();

    static void Main(string[] args)
    {
        // Create and start the first thread for adding random odd numbers
        var oddThread = new Thread(AddRandomOddNumbers);
        oddThread.Start();

        // Create and start the second thread for calculating prime numbers
        var primeThread = new Thread(CalculateAndAddNegativePrimes);
        primeThread.Start();

        // Create and start the third thread when the count reaches 250,000
        var thirdThread = new Thread(AddRandomEvenNumbers);
        thirdThread.Start();

        while (totalCount < 1000000)
        {
            // Check if the count has reached 250,000 and start the third thread
            if (totalCount >= 250000 && thirdThread.ThreadState == ThreadState.Unstarted)
                thirdThread.Start();
        }

        // Wait for all threads to finish
        oddThread.Join();
        primeThread.Join();
        thirdThread.Join();

        // Sort the global list in ascending order
        globalList.Sort();

        // Count and display the number of odd and even numbers in the list
        int oddCount = globalList.Count(n => n % 2 != 0);
        int evenCount = globalList.Count(n => n % 2 == 0);
        Console.WriteLine($"Number of odd numbers: {oddCount}");
        Console.WriteLine($"Number of even numbers: {evenCount}");

        // Serialize the list to binary and XML files
        SerializeToBinary("globalList.bin");
        SerializeToXML("globalList.xml");
    }

    private static void AddRandomOddNumbers()
    {
        Random random = new Random();
        while (totalCount < 1000000)
        {
            int number = random.Next(1, 1000000);
            if (number % 2 != 0)
            {
                lock (lockObject)
                {
                    globalList.Add(number);
                    totalCount++;
                }
            }
        }
    }

    private static void CalculateAndAddNegativePrimes()
    {
        int number = 2;
        while (totalCount < 1000000)
        {
            if (IsPrime(number))
            {
                lock (lockObject)
                {
                    globalList.Add(-number);
                    totalCount++;
                }
            }
            number++;
        }
    }

    private static void AddRandomEvenNumbers()
    {
        Random random = new Random();
        while (totalCount < 1000000)
        {
            int number = random.Next(1, 1000000);
            if (number % 2 == 0)
            {
                lock (lockObject)
                {
                    globalList.Add(number);
                    totalCount++;
                }
            }
        }
    }

    private static bool IsPrime(int number)
    {
        if (number <= 1)
            return false;

        if (number <= 3)
            return true;

        if (number % 2 == 0 || number % 3 == 0)
            return false;

        for (int i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }

        return true;
    }

    private static void SerializeToBinary(string fileName)
    {
        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                foreach (int number in globalList)
                {
                    writer.Write(number);
                }
            }
        }
    }

    private static void SerializeToXML(string fileName)
    {
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<int>));
            xmlSerializer.Serialize(sw, globalList);
        }
    }
}
