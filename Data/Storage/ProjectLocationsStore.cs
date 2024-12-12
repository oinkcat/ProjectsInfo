using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;

namespace ProjectsInfo.Data.Storage
{
    /// <summary>
    /// Хранит список расплолжений проектов
    /// </summary>
    internal class ProjectLocationsStore
    {
        private const string LocationsFileName = "locations.txt";

        /// <summary>
        /// Загрузить список расположений проектов
        /// </summary>
        /// <returns>Список, содержащий пути расположений проектов</returns>
        public async Task<IList<string>> LoadLocations()
        {
            string locationsFilePath = GetLocationsFilePath();

            if (File.Exists(locationsFilePath))
            {
                var locationPaths = new List<string>();

                using (var reader = new StreamReader(locationsFilePath))
                {
                    while(!reader.EndOfStream)
                    {
                        string pathFromFile = (await reader.ReadLineAsync()).Trim();
                        if (!Directory.Exists(pathFromFile)) { continue; }
                        
                        locationPaths.Add(pathFromFile);
                    }
                }

                return locationPaths;
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Сохранить пути расположения проектов
        /// </summary>
        /// <param name="locationPaths">Список, содержащий пути расположений проектов</param>
        public async Task SaveLocations(IList<string> locationPaths)
        {
            string locationsFilePath = GetLocationsFilePath();

            using(var writer = new StreamWriter(locationsFilePath))
            {
                foreach(string path in locationPaths)
                {
                    await writer.WriteLineAsync(path);
                }
            }
        }

        // Получить путь к файлу данных
        private static string GetLocationsFilePath()
        {
            var exeName = Assembly.GetEntryAssembly().GetName();
            string appName = exeName.Name.Split('.')[0];

            var baseFolderKind = Environment.SpecialFolder.LocalApplicationData;
            string baseDirectory = Environment.GetFolderPath(baseFolderKind);
            string appDataDirectory = Path.Combine(baseDirectory, appName);

            if(!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }

            return Path.Combine(appDataDirectory, LocationsFileName);
        }
    }
}
