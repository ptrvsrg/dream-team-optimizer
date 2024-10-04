using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace DreamTeamOptimizer.Core.Helpers;

public class CsvLoader
{
    public static List<T> Load<T>(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        return csv.GetRecords<T>().ToList();
    }
}