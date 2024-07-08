using System.Diagnostics;
using System.Reflection.Metadata;

namespace Übung_Gerät.Windows.Models;

internal class Leistungsregelung
{
    // Method to update the power level of the device
    public static int UpdatePowerLevel(int puls, int lastPuls)
    {
        if (puls < 90 && lastPuls < 90)
        {
            return 3;
        }
        else if (puls < 100 && lastPuls < 100)
        {
            return 2;
        }
        else if (puls < 110 && lastPuls < 110)
        {
            return 1;
        }
        else
        {
            return 1; // Default case if none of the conditions are met
        }
    }

}
