using System.Diagnostics;

namespace Übung_Gerät.Windows.Models;

internal static class Zeitregelung
{
    //Manage Time changes
    public static int ChangeTime(object parameter, int zeit)
    {
        // Check if parameter is a number
        if (!int.TryParse(parameter.ToString(), out var timechange))
            return zeit;

        // Calculate new Time value
        var time = zeit + timechange;

        // Min and Max Time values
        const int minTime = 4;
        const int maxTime = 99;

        // Check if time is within the range of min and max Time
        if (time < minTime)
        {
            Debug.WriteLine($"Ändere Zeit um: {timechange} zu {minTime}");
            return minTime;
        }
        else if (time > maxTime)
        {
            Debug.WriteLine($"Ändere Zeit um: {timechange} zu {maxTime}");
            return maxTime;
        }
        else
        {
            Debug.WriteLine($"Ändere Zeit um: {timechange} zu {time}");
            return time;
        }
    }

    //Manage Progress bar changes Time to TimeRemaining
    public static double ChangeProgress(int time, int timeremaining)
    {
        // Calculate Progress bar value
        var progress = (double)timeremaining / time * 100;        
        return progress;
    }
}
