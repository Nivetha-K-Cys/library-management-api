using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models;

public class Book
{
    public int Id { get; set; }

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

    public bool IsAvailable { get; set; } = true;
}