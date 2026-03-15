# DDTripEdit

A **Blazor WebAssembly** application for reading, displaying, and editing a CSV-based trip diary.

## Features

- 📋 Browse all trips in a sortable, filterable, paginated table
- 🔍 Search by trip name, city, country or type
- ✏️ Add, edit and delete trips via a validated form
- 💾 All data persisted to `Trips.csv` (semicolon-delimited, comma decimal scores)
- 📱 Responsive Bootstrap 5 UI

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Project Structure

```
DDTripEdit/
├── DDTripEdit.sln
├── Trips.csv                        # Data file
├── DDTripEdit.Client/               # Blazor WebAssembly UI
│   ├── Pages/
│   │   ├── Home.razor               # Home page
│   │   ├── Trips.razor              # Trip list with sort/filter/pagination
│   │   └── TripEdit.razor           # Add/Edit trip form
│   └── wwwroot/
├── DDTripEdit.Server/               # ASP.NET Core Web API host
│   ├── Controllers/TripsController.cs
│   └── Services/CsvService.cs
└── DDTripEdit.Shared/               # Shared models
    └── Models/Trip.cs
```

## Setup & Run

```bash
# Restore packages
dotnet restore

# Run the application (starts both server and client)
dotnet run --project DDTripEdit.Server

# The app will be available at https://localhost:5001 (or http://localhost:5000)
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/trips` | Get all trips |
| GET | `/api/trips/{id}` | Get trip by id |
| POST | `/api/trips` | Create new trip |
| PUT | `/api/trips/{id}` | Update trip |
| DELETE | `/api/trips/{id}` | Delete trip |

## CSV Format

The `Trips.csv` file uses:
- **Delimiter**: `;` (semicolon)
- **Decimal notation**: `,` (comma, e.g. `4,5` → 4.5)
- **Date format**: `yyyy-MM-dd`
- **Header row**: `Date;TripName;City;Country;Type;Score`

## Configuration

The CSV file path can be configured in `DDTripEdit.Server/appsettings.json`:

```json
{
  "CsvFilePath": "Trips.csv"
}
```
