using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excersize18IndustrialMachine.Helpers;
public static class DataGenerator
{
    private static Random rand = new Random();
    public static byte[] GetData()
    {
        int fakeData = rand.Next(0, 1000);

        string dataTempHolder = fakeData.ToString();

        return Encoding.ASCII.GetBytes(dataTempHolder);
    }

    public static byte[] GetData(byte[] previous)
    {
        string previousHolder = Encoding.ASCII.GetString(previous);

        if(int.TryParse(previousHolder, out int previousInt))
        {
            previousInt += rand.Next(1, 101) - 50;

            return Encoding.ASCII.GetBytes(previousInt.ToString());
        }

        return GetData();
    }
}
