using System.ComponentModel.DataAnnotations;

namespace DDTripEdit.Shared.Models;

/// <summary>
/// Represents a trip entry.
/// </summary>
public class Trip
{
    /// <summary>Gets or sets the unique identifier (row index).</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the date of the trip.</summary>
    [Required(ErrorMessage = "Date is required.")]
    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>Gets or sets the name of the trip.</summary>
    [Required(ErrorMessage = "Trip name is required.")]
    [StringLength(200)]
    public string TripName { get; set; } = string.Empty;

    /// <summary>Gets or sets the city visited.</summary>
    [Required(ErrorMessage = "City is required.")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the country visited.</summary>
    [Required(ErrorMessage = "Country is required.")]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    /// <summary>Gets or sets the type of trip.</summary>
    [Required(ErrorMessage = "Type is required.")]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the score (0-5), optional.</summary>
    [Range(0, 5, ErrorMessage = "Score must be between 0 and 5.")]
    public decimal? Score { get; set; }
}
