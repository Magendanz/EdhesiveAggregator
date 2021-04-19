using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;

namespace EdhesiveAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: EdhesiveAggregator <file.csv>");
                return;
            }
            var filename = args[0];

            // Load CSV file that has been exported from Canvas gradebook
            var dt = DataTable.New.ReadCsv(filename);

            dt.DeleteColumns(new string[] { "ID", "SIS User ID", "SIS Login ID", "Section" });
            RemoveEmptyColumns(dt);
            AggregateColumns(dt, LessonFilter("Fast Start"));
            AggregateColumns(dt, LessonFilter("Review Questions"));
            AggregateColumns(dt, LessonFilter("Coding Activity"));
            AggregateColumns(dt, LabFilter("Consumer Review"));
            AggregateColumns(dt, LabFilter("Data"));
            AggregateColumns(dt, LabFilter("Steganography"));
            AggregateColumns(dt, LabFilter("Celebrity"));
            AggregateColumns(dt, LabFilter("Magpie"));
            AggregateColumns(dt, LabFilter("Elevens"));
            AggregateColumns(dt, LabFilter("Picture"));
            AggregateColumns(dt, MultipartFilter("FRQ"));

            // Save result and open with Excel
            dt.SaveCSV(@$"Output\{filename}");
            Process.Start("explorer.exe", @$"Output\{filename}");
        }

        static void RemoveEmptyColumns(MutableDataTable dt)
        {
            var names = new List<string>();
            foreach (var col in dt.Columns)
            {
                // Remove volumes with only the "Points Possible" row filled
                var count = col.Values.Count(c => !String.IsNullOrEmpty(c));
                if (count <= 1)
                    names.Add(col.Name);

                // Also, remove calculated columns
                if (col.Values[0] == "(read only)")
                    names.Add(col.Name);
            }
            dt.DeleteColumns(names.ToArray());
        }

        static void AggregateColumns(MutableDataTable dt, string pattern)
        {
            var input = string.Join('\n', dt.ColumnNames.ToArray());
            var matches = Regex.Matches(input, pattern);
            var names = new List<string>();
            foreach (Match match in matches)
            {
                var name = match.Groups[1].Value + match.Groups[2].Value;
                var col = dt.GetColumn(match.Value);
                var target = dt.GetColumn(name);
                if (target == null)
                {
                    col.Name = name;
                    continue;
                }
                var rows = col.Values.Count();
                for (int i = 0; i < rows; i++)
                {
                    double.TryParse(target.Values[i], out var total);
                    double.TryParse(col.Values[i], out var value);
                    target.Values[i] = $"{total + value}";
                }
                names.Add(col.Name);
            }
            dt.DeleteColumns(names.ToArray());
        }

        static string LessonFilter(string filter)
            => @$"(Unit \d+: )Lesson \d.+?- ({filter}).+";
        static string LabFilter(string filter)
            => @$"({filter}): Activity [1-9] \(\d+\)";
        static string MultipartFilter(string filter)
            => @$"({filter}: )([\w\s]+) Part [A-Z] \(\d+\)";
    }
}
