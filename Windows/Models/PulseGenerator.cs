using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Übung_Gerät.Windows.Models;

public class PulseGenerator
{
    public int Puls
    {
        get; private set;
    }

    private Random random = new Random();

    public async Task GenerateRandomPulseAsync(int totalTime, CancellationToken cancellationToken)
    {
        var delay = 5000; // 5 seconds
        var iterations = totalTime / delay;

        // Initial pulse within the first range (65-75)
        Puls = random.Next(65, 75);
        Debug.WriteLine($"Initial Pulse (60-75): {Puls}");

        for (var i = 1; i <= iterations; i++)
        {
            // Check if cancellation is requested
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.WriteLine("Pulse generation was cancelled.");
                break;
            }

            // Wait for the specified delay
            await Task.Delay(delay, cancellationToken);

            if (i <= iterations / 3)
            {
                // First third of the time: small variations within 65-89
                Puls = GetNextPulse(Puls, 65, 89);
                Debug.WriteLine($"Pulse (65-89): {Puls}");
            }
            else if (i <= 2 * iterations / 3)
            {
                // Second third of the time: small variations within 90-109
                Puls = GetNextPulse(Puls, 90, 109);
                Debug.WriteLine($"Pulse (90-109): {Puls}");
            }
            else
            {
                // Final third of the time: small variations within 110-135
                Puls = GetNextPulse(Puls, 110, 135);
                Debug.WriteLine($"Pulse (110-135): {Puls}");
            }
        }
    }

    private int GetNextPulse(int currentPulse, int minRange, int maxRange)
    {
        // Small variation, between -3 and 6
        var variation = random.Next(-3, 6);
        var newPulse = currentPulse + variation;

        // Ensure the new pulse stays within the specified range
        if (newPulse < minRange) newPulse = minRange;
        if (newPulse > maxRange) newPulse = maxRange;

        return newPulse;
    }
}
