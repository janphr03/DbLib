using System;
using System.Diagnostics;

namespace DbLib.TestUnits
{
    public class MySqlAccessIntegrationTests
    {
        [Fact]
        public void Test_StartMySqlContainer()
        {
            // Starte den Dockercontainer – vorhandene Instanz wird dabei vorher entfernt
            //StartMySqlContainer();

            // Optional: Hier kannst du noch einen Test einbauen, der z. B. eine TCP-Verbindung versucht.
        }

        [Fact]
        public void Test_StopMySqlContainer()
        {
            // Starten zunächst, damit dann der Stopp-Test sinnvoll ist.
            StartMySqlContainer();

            // Stoppe den Dockercontainer
            //StopMySqlContainer();
        }

        /// <summary>
        /// Startet einen MySQL-Dockercontainer.
        /// Voraussetzung: Docker muss installiert sein und im PATH verfügbar.
        /// Falls bereits ein Container mit dem Namen existiert, wird er vorher entfernt.
        /// </summary>
        private static void StartMySqlContainer()
        {
            // Entferne existierenden Container (force), falls vorhanden
            RunCommand("docker rm -f mysql_test_db");

            // Starte den Container. Hier wird Hostport 3306 an Containerport 3306 gebunden.
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments =
                    "run -d -p 3306:3306 --name mysql_test_db -e MYSQL_ROOT_PASSWORD=rootpw -e MYSQL_DATABASE=testprotocol -e MYSQL_USER=testuser -e MYSQL_PASSWORD=testpw mysql:8.0",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            Console.WriteLine("Container Start Output: " + output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Container Start Error: " + error);
            }
        }


        /// <summary>
        /// Stoppt den MySQL-Dockercontainer und entfernt ihn.
        /// </summary>
        private static void StopMySqlContainer()
        {
            // Container stoppen
            var psiStop = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "rm mysql_test_db",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var processStop = Process.Start(psiStop);
            processStop.WaitForExit();
            string outputStop = processStop.StandardOutput.ReadToEnd();
            string errorStop = processStop.StandardError.ReadToEnd();
            Console.WriteLine("Container Stop Output: " + outputStop);
            if (!string.IsNullOrEmpty(errorStop))
            {
                Console.WriteLine("Container Stop Error: " + errorStop);
            }

            // Container entfernen
            var psiRm = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "rm mysql_test_db",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var processRm = Process.Start(psiRm);
            processRm.WaitForExit();
            string outputRm = processRm.StandardOutput.ReadToEnd();
            string errorRm = processRm.StandardError.ReadToEnd();
            Console.WriteLine("Container Remove Output: " + outputRm);
            if (!string.IsNullOrEmpty(errorRm))
            {
                Console.WriteLine("Container Remove Error: " + errorRm);
            }
        }

        /// <summary>
        /// Hilfsmethode zum Ausführen eines Befehls via Docker.
        /// </summary>
        private static void RunCommand(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = command.Substring("docker ".Length), // Entfernt "docker " von der Zeichenkette, wenn nötig
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            Console.WriteLine("RunCommand Output: " + output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("RunCommand Error: " + error);
            }
        }
    }
}
