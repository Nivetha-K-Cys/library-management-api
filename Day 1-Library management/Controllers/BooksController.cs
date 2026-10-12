using LibraryManagementAPI.Data;
using LibraryManagementAPI.DTOs;
using LibraryManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/books
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(CreateBookDto dto)
    {
        string isbn = dto.ISBN.Trim();

        bool isbnExists = await _context.Books
            .AnyAsync(book => book.ISBN == isbn);

        if (isbnExists)
        {
            return Conflict(new { message = "This ISBN already exists." });
        }

        var book = new Book
        {
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim(),
            ISBN = isbn,
            PublishedYear = dto.PublishedYear,
            IsAvailable = dto.IsAvailable
        };

        _context.Books.Add(book);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            return Conflict(new { message = "This ISBN already exists." });
        }

        return CreatedAtAction(
            nameof(GetBookById),
            new { id = book.Id },
            book);
    }

    // GET /api/books
    // GET /api/books?search=martin
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks(
        [FromQuery] string? search = null)
    {
        IQueryable<Book> query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string searchText = search.Trim();

            query = query.Where(book =>
                book.Title.Contains(searchText) ||
                book.Author.Contains(searchText));
        }

        var books = await query
            .OrderBy(book => book.Id)
            .ToListAsync();

        return Ok(books);
    }

    // GET /api/books/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound(new { message = "Book not found." });
        }

        return Ok(book);
    }

    // PUT /api/books/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Book>> UpdateBook(
        int id,
        UpdateBookDto dto)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound(new { message = "Book not found." });
        }

        string isbn = dto.ISBN.Trim();

        bool isbnExists = await _context.Books.AnyAsync(existingBook =>
            existingBook.ISBN == isbn &&
            existingBook.Id != id);

        if (isbnExists)
        {
            return Conflict(new { message = "This ISBN already exists." });
        }

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();
        book.ISBN = isbn;
        book.PublishedYear = dto.PublishedYear;
        book.IsAvailable = dto.IsAvailable;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            return Conflict(new { message = "This ISBN already exists." });
        }

        return Ok(book);
    }

    // DELETE /api/books/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound(new { message = "Book not found." });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}