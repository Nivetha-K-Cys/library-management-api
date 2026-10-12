using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs;

public class UpdateBookDto : IValidatableObject
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    public int PublishedYear { get; set; }

    public bool IsAvailable { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (PublishedYear < 1450 || PublishedYear > DateTime.Now.Year)
        {
            yield return new ValidationResult(
                $"PublishedYear must be between 1450 and {DateTime.Now.Year}.",
                new[] { nameof(PublishedYear) });
        }
    }
}