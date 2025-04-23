using FBMarketBot.IoC;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FBMarketBot.Common
{
    /// <summary>
    /// Base class for accessing and deserializing JSON configuration or data from a file.
    /// </summary>
    public class BaseAccessors<TClass, TInterface>
        where TClass : class, TInterface
    {
        private readonly string _path;

        /// <summary>
        /// Initializes the accessor with the file path where the JSON data is stored.
        /// </summary>
        /// <param name="path">The path to the JSON file.</param>
        public BaseAccessors(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Gets the deserialized object from the JSON file.
        /// </summary>
        /// <returns>The deserialized object of type TInterface.</returns>
        /// <exception cref="InvalidOperationException">Thrown if object deserialization fails.</exception>
        public TInterface Get()
        {
            try
            {
                var obj = Deserialize();
                return obj;
            }
            catch (Exception ex)
            {
                // Throws an exception if deserialization fails
                throw new InvalidOperationException("Failed to get object", ex);
            }
        }

        /// <summary>
        /// Deserializes the JSON content of the file into an object of type TClass.
        /// </summary>
        /// <returns>The deserialized object of type TClass.</returns>
        /// <exception cref="JsonReaderException">Thrown if JSON parsing fails.</exception>
        /// <exception cref="Exception">Thrown if general errors occur during deserialization.</exception>
        private TClass Deserialize()
        {
            // Reads the entire JSON file content
            var settings = File.ReadAllText(_path);

            TClass jsonToObj = null;
            try
            {
                // Attempts to deserialize the JSON into an object of type TClass
                jsonToObj = JsonConvert.DeserializeObject<TClass>(settings);
                if (jsonToObj == null)
                {
                    throw new Exception("The JSON file could not be deserialized into an object.");
                }
            }
            catch (JsonReaderException jsonEx)
            {
                // Logs and rethrows JSON-specific errors
                Console.WriteLine($"JSON Parsing error: {jsonEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Logs and rethrows general errors
                Console.WriteLine($"General error: {ex.Message}");
                throw;
            }

            return jsonToObj;
        }
    }
}
