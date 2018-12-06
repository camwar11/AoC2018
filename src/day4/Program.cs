using System;
using System.Linq;
using System.Text.RegularExpressions;
using common;

namespace day4
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] guardSleepTracker = new int[4000,61];
            int sleepiestGuard = -1;
            int mostSleepMinutes = -1;
            int currentGuard = -1;
            int currentSleepStart = -1;
            int minuteSleepiestGuard = -1;
            int minuteSleepiestMinuteAmt = -1;
            int minuteSleepiestMinute = -1;
            Regex guardBeginsShift = new Regex("Guard #(\\d+) begins", RegexOptions.Compiled);
            Regex guardFallsAsleep = new Regex(":(\\d{2})\\] falls asleep", RegexOptions.Compiled);
            Regex guardWakesUp = new Regex(":(\\d{2})\\] wakes up", RegexOptions.Compiled);

            using(var reader = new InputReader("input.txt"))
            {
                foreach (var line in reader.GetLines().OrderBy(x => x.Substring(0, 18)))
                {
                    var shiftBegins = guardBeginsShift.Match(line);

                    if(shiftBegins.Success)
                    {
                        currentGuard = int.Parse(shiftBegins.Groups[1].Value);
                        continue;                        
                    }

                    var fallsAsleep = guardFallsAsleep.Match(line);

                    if(fallsAsleep.Success)
                    {
                        currentSleepStart = int.Parse(fallsAsleep.Groups[1].Value);
                        continue;
                    }

                    var wakesUp = guardWakesUp.Match(line);

                    if(wakesUp.Success)
                    {
                        int wakeUpTime = int.Parse(wakesUp.Groups[1].Value);
                        
                        for (int i = currentSleepStart; i < wakeUpTime; i++)
                        {
                            int minuteAmount = ++guardSleepTracker[currentGuard, i];
                            if(minuteAmount > minuteSleepiestMinuteAmt)
                            {
                                minuteSleepiestMinuteAmt = minuteAmount;
                                minuteSleepiestGuard = currentGuard;
                                minuteSleepiestMinute = i;
                            }
                        }

                        int sleepAmount = wakeUpTime - currentSleepStart;
                        int guardSleepTotal = (guardSleepTracker[currentGuard, 60] += sleepAmount);

                        if(guardSleepTotal > mostSleepMinutes)
                        {
                            sleepiestGuard = currentGuard;
                            mostSleepMinutes = guardSleepTotal;
                        }
                    }
                }
                
                int guardsSleepiestMinute = -1;
                int guardsSleepiestMinuteAmount = -1;

                for (int i = 0; i < 60; i++)
                {
                    if(guardSleepTracker[sleepiestGuard, i] > guardsSleepiestMinuteAmount)
                    {
                        guardsSleepiestMinute = i;
                        guardsSleepiestMinuteAmount = guardSleepTracker[sleepiestGuard, i];
                    }
                }

                Console.WriteLine("Strategy1: {0} x {1} = {2}", sleepiestGuard, guardsSleepiestMinute, sleepiestGuard * guardsSleepiestMinute);
                Console.WriteLine("Strategy2: {0} x {1} = {2}", minuteSleepiestGuard, minuteSleepiestMinute, minuteSleepiestGuard * minuteSleepiestMinute);
            }
        }
    }
}
