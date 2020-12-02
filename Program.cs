using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;

namespace EdhesiveAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = args[0];
            if (String.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Usage: EdhesiveAggregator <file>");
                return;
            }
            var dt = DataTable.New.ReadCsv(filename);
            dt.DeleteColumns(new string[] { "ID", "SIS User ID", "SIS Login ID", "Section" });
            RemoveEmptyColumns(dt);
            AggregateColumns(dt, "Fast Start");
            AggregateColumns(dt, "Review Questions");
            AggregateColumns(dt, "Coding Activity");
            dt.SaveCSV(@$"Output\{filename}");
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

        static void AggregateColumns(MutableDataTable dt, string str)
        {
            var pattern = @$"(Unit \d+: )Lesson \d.+?- {str}.+";
            var input = string.Join('\n', dt.ColumnNames.ToArray());
            var matches = Regex.Matches(input, pattern);
            var names = new List<string>();
            foreach (Match match in matches)
            {
                var name = match.Groups[1].Value + str;
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
    }
}
