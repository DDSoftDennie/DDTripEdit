using CsvHelper;
using CsvHelper.Configuration;
using DDTripEdit.Shared.Models;
using System.Globalization;

namespace DDTripEdit.Server.Services;

/// <summary>
/// Service for reading and writing trips data from/to a CSV file.
/// </summary>
public class CsvService
{
    private readonly string _filePath;
    private readonly object _lock = new();

    /// <summary>Initializes a new instance of <see cref="CsvService"/>.</summary>
    public CsvService(IConfiguration configuration, IWebHostEnvironment env)
    {
        var configuredPath = configuration["CsvFilePath"] ?? "Trips.csv";

        // Try the configured path relative to content root first
        var contentRootPath = Path.Combine(env.ContentRootPath, configuredPath);
        if (File.Exists(contentRootPath))
        {
            _filePath = contentRootPath;
            return;
        }

        // Try relative to app base directory (bin output folder)
        var appBasePath = Path.Combine(AppContext.BaseDirectory, configuredPath);
        if (File.Exists(appBasePath))
        {
            _filePath = appBasePath;
            return;
        }

        // Fallback: look in the current directory
        var currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
        if (File.Exists(currentDirPath))
        {
            _filePath = currentDirPath;
            return;
        }

        // Default to content root (will be created if needed)
        _filePath = contentRootPath;
    }

    /// <summary>Reads all trips from the CSV file.</summary>
    public List<Trip> ReadAll()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath))
                return [];

            using var reader = new StreamReader(_filePath, System.Text.Encoding.UTF8);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TripCsvMap>();

            var trips = csv.GetRecords<Trip>().ToList();

            // Assign sequential IDs based on position
            for (int i = 0; i < trips.Count; i++)
                trips[i].Id = i;

            return trips;
        }
    }

    /// <summary>Writes all trips back to the CSV file.</summary>
    public void WriteAll(List<Trip> trips)
    {
        lock (_lock)
        {
            // Ensure directory exists
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            using var writer = new StreamWriter(_filePath, false, System.Text.Encoding.UTF8);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            };

            using var csv = new CsvWriter(writer, config);
            csv.Context.RegisterClassMap<TripCsvMap>();
            csv.WriteRecords(trips);
        }
    }

    /// <summary>Gets a trip by its ID (row index).</summary>
    public Trip? GetById(int id)
    {
        var trips = ReadAll();
        return trips.FirstOrDefault(t => t.Id == id);
    }

    /// <summary>Adds a new trip and persists to CSV.</summary>
    public Trip Add(Trip trip)
    {
        var trips = ReadAll();
        // IDs are sequential position-based; next ID = count of existing trips
        trip.Id = trips.Count;
        trips.Add(trip);
        WriteAll(trips);
        return trip;
    }

    /// <summary>Updates an existing trip and persists to CSV.</summary>
    public bool Update(int id, Trip updated)
    {
        var trips = ReadAll();
        var index = trips.FindIndex(t => t.Id == id);
        if (index < 0) return false;

        updated.Id = id;
        trips[index] = updated;
        WriteAll(trips);
        return true;
    }

    /// <summary>Deletes a trip by ID and persists to CSV.</summary>
    public bool Delete(int id)
    {
        var trips = ReadAll();
        var index = trips.FindIndex(t => t.Id == id);
        if (index < 0) return false;

        trips.RemoveAt(index);
        // Re-assign IDs
        for (int i = 0; i < trips.Count; i++)
            trips[i].Id = i;

        WriteAll(trips);
        return true;
    }
}

/// <summary>CSV mapping class for Trip with semicolon delimiter and comma decimals.</summary>
public sealed class TripCsvMap : ClassMap<Trip>
{
    public TripCsvMap()
    {
        Map(m => m.Date).Name("Date").TypeConverterOption.Format("yyyy-MM-dd");
        Map(m => m.TripName).Name("TripName");
        Map(m => m.City).Name("City");
        Map(m => m.Country).Name("Country");
        Map(m => m.Type).Name("Type");
        Map(m => m.Score).Name("Score").TypeConverter<CommaDecimalConverter>();
        Map(m => m.Id).Ignore();
    }
}

/// <summary>Type converter that handles decimal values with comma notation (e.g. "4,5" → 4.5).</summary>
public class CommaDecimalConverter : CsvHelper.TypeConversion.DecimalConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, CsvHelper.Configuration.MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // Replace comma with dot and parse
        var normalized = text.Replace(',', '.');
        if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    public override string? ConvertToString(object? value, IWriterRow row, CsvHelper.Configuration.MemberMapData memberMapData)
    {
        if (value is null)
            return string.Empty;

        if (value is decimal d)
        {
            // Format with comma notation (strip trailing zeros)
            var str = d.ToString("G", CultureInfo.InvariantCulture);
            return str.Replace('.', ',');
        }

        return base.ConvertToString(value, row, memberMapData);
    }
}
