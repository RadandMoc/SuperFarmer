using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Assets.Scripts
{
    [Serializable]
    public class Save
    {
        List<Player> playerList;
        private byte actualPlayerId = 1;
        bool wasTrade;

        public Save(List<Player> playerList, byte actualPlayerId, bool wasTrade)
        {
            PlayerList = playerList;
            ActualPlayerId = actualPlayerId;
            WasTrade = wasTrade;
        }

        public List<Player> PlayerList { get => playerList; set => playerList = value; }
        public byte ActualPlayerId { get => actualPlayerId; set => actualPlayerId = value; }
        public bool WasTrade { get => wasTrade; set => wasTrade = value; }

        /// <summary>
        /// Create binary save
        /// </summary>
        /// <param name="nazwa">file name</param>
        public void SaveBinary(string nazwa)
        {
            FileStream fs = new FileStream($"{nazwa}.bin", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, this);
            }
            catch (SerializationException e)
            {
                //Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Load binary file to Save class
        /// </summary>
        /// <param name="nazwa">file name (without .bin)</param>
        /// <returns>return class Save which was in file</returns>
        public static Save ReadBinary(string nazwa)
        {
            Save zapis;
            try
            {
                FileStream fs = new FileStream($"{nazwa}.bin", FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    zapis = (Save)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    //Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    //throw;
                    return null;
                }
                finally
                {
                    fs.Close();
                }
                return zapis;
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
        }
    }
}
