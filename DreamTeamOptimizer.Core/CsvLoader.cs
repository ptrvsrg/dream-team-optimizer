using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace DreamTeamOptimizer.Core;

public class CsvLoader
{
    public static List<T> Load<T>(String filePath)
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            return new List<T>(csv.GetRecords<T>());
        }
    }
}