using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace lab3
{
    [Serializable]
    class Station
    {
        public string Name { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
    }

    [Serializable]
    class Route
    {
        public List<Station> Stations { get; private set; }
        public int TotalSeats { get; set; }
        public string DaysOfWeek { get; set; }
        public int FlightNumber { get; set; }

        public Route()
        {
            Stations = new List<Station>();
        }

        // Додавання станції до маршруту
        public void AddStation(Station station)
        {
            Stations.Add(station);
        }

        // Сортування станцій за різними параметрами
        public void SortStationsBySeats()
        {
            Stations.Sort((s1, s2) => s1.AvailableSeats.CompareTo(s2.AvailableSeats));
        }

        public void SortStationsByDayOfWeek()
        {
            Stations.Sort((s1, s2) => s1.DepartureTime.CompareTo(s2.DepartureTime));
        }

        public void SortStationsByFlightNumber()
        {
            Stations.Sort((s1, s2) => s1.ArrivalTime.CompareTo(s2.ArrivalTime));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ініціалізація директорії для зберігання/відновлення даних
            string dataDirectory = InitializeDataDirectory();

            List<Route> routes = new List<Route>();

            while (true)
            {
                Console.WriteLine("Оберіть опцію:");
                Console.WriteLine("1. Додати новий маршрут");
                Console.WriteLine("2. Переглянути існуючі маршрути");
                Console.WriteLine("3. Сортування маршрутів");
                Console.WriteLine("4. Вийти");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Route newRoute = CreateRoute();
                        routes.Add(newRoute);
                        SerializeRoutes(routes, dataDirectory);
                        break;

                    case "2":
                        routes = DeserializeRoutes(dataDirectory);
                        DisplayRoutes(routes);
                        break;

                    case "3":
                        SortRoutes(routes);
                        SerializeRoutes(routes, dataDirectory);
                        break;

                    case "4":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
        }

        static string InitializeDataDirectory()
        {
            string dataDirectory = "Data"; // Папка для збереження даних

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            return dataDirectory;
        }

        static Route CreateRoute()
        {
            Route route = new Route();

            Console.WriteLine("Введіть загальну кількість місць:");
            route.TotalSeats = int.Parse(Console.ReadLine());

            Console.WriteLine("Введіть дні тижня:");
            route.DaysOfWeek = Console.ReadLine();

            Console.WriteLine("Введіть номер рейсу:");
            route.FlightNumber = int.Parse(Console.ReadLine());

            while (true)
            {
                Console.WriteLine("Додайте станцію до маршруту (або натисніть Enter, щоб завершити):");
                Station station = new Station();

                Console.Write("Назва станції: ");
                station.Name = Console.ReadLine();

                if (string.IsNullOrEmpty(station.Name))
                {
                    break;
                }

                Console.Write("Час прибуття: ");
                station.ArrivalTime = Console.ReadLine();

                Console.Write("Час відправлення: ");
                station.DepartureTime = Console.ReadLine();

                Console.Write("Кількість вільних місць: ");
                station.AvailableSeats = int.Parse(Console.ReadLine());

                route.AddStation(station);
            }

            return route;
        }

        static void SerializeRoutes(List<Route> routes, string dataDirectory)
        {
            string filePath = Path.Combine(dataDirectory, "routes.dat");

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, routes);
            }

            Console.WriteLine("Маршрути збережено.");
        }

        static List<Route> DeserializeRoutes(string dataDirectory)
        {
            string filePath = Path.Combine(dataDirectory, "routes.dat");

            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    IFormatter formatter = new BinaryFormatter();
                    return (List<Route>)formatter.Deserialize(fs);
                }
            }
            else
            {
                Console.WriteLine("Файл з маршрутами не знайдено.");
                return new List<Route>();
            }
        }

        static void DisplayRoutes(List<Route> routes)
        {
            if (routes.Count == 0)
            {
                Console.WriteLine("Маршрути не знайдено.");
                return;
            }

            foreach (var route in routes)
            {
                Console.WriteLine($"Маршрут: Рейс {route.FlightNumber}, Дні тижня: {route.DaysOfWeek}, Загальна кількість місць: {route.TotalSeats}");
                Console.WriteLine("Станції:");

                foreach (var station in route.Stations)
                {
                    Console.WriteLine($"- {station.Name}, Прибуття: {station.ArrivalTime}, Відправлення: {station.DepartureTime}, Вільні місця: {station.AvailableSeats}");
                }

                Console.WriteLine();
            }
        }

        static void SortRoutes(List<Route> routes)
        {
            Console.WriteLine("Оберіть тип сортування:");
            Console.WriteLine("1. За загальною кількістю місць");
            Console.WriteLine("2. За днем тижня");
            Console.WriteLine("3. За номером рейсу");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    routes.Sort((r1, r2) => r1.TotalSeats.CompareTo(r2.TotalSeats));
                    break;

                case "2":
                    routes.Sort((r1, r2) => r1.DaysOfWeek.CompareTo(r2.DaysOfWeek));
                    break;

                case "3":
                    routes.Sort((r1, r2) => r1.FlightNumber.CompareTo(r2.FlightNumber));
                    break;

                default:
                    Console.WriteLine("Невірний вибір сортування.");
                    break;
            }
        }
    }
}
