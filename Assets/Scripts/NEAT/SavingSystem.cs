using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NEAT
{
    public class SavingSystem
    {
        /// <summary>
        /// Store genome in files
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="genome">The genome struct</param>
        /// <param name="useJson">Use json format to store data</param>
        public static void StoreGenome(string fileName, Genometype genome, bool useJson=false)
        {
            if (useJson)
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), JsonUtility.ToJson(genome, true));
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();

                FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, fileName), FileMode.Create);
                formatter.Serialize(stream, genome);
                stream.Close();
            }
        }

        /// <summary>
        /// Get the genome that store in the file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="useJson">Use json format to read the data</param>
        /// <returns>The genome structure</returns>
        public static Genometype GetGenome(string fileName, bool useJson=false)
        {
            if (useJson)
            {
                StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, fileName));
                string text = reader.ReadToEnd();
                reader.Close();

                return JsonUtility.FromJson<Genometype>(text);
            }
            else
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, fileName), FileMode.Open);
                Genometype data = (Genometype)binaryFormatter.Deserialize(stream);
                stream.Close();

                return data;
            }
        }
    }
}