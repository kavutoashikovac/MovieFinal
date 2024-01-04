using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business;

public interface IGenreService
{
    IQueryable<GenreModel> Query();
    Result Add(GenreModel model);
    Result Update(GenreModel model);
    Result DeleteGenre(int id);
    List<GenreModel> GetList();
  
    GenreModel GetItem(int id);
}

public class GenreService : IGenreService
{
    private readonly Db _db;

    public GenreService(Db db)
    {
        _db = db;
    }

    public IQueryable<GenreModel> Query()
    {
        return _db.Genre.Select(g => new GenreModel
        {
            Movies = g.Movies,
            Name = g.Name
        });
    }

    public Result Add(GenreModel model)
    {
        // Check for existing genre with the same name
        if (_db.Genre.Any(g => g.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new ErrorResult("Genre with the same name already exists!");

        // Create entity from the model
        var genreEntity = new Genre
        {
            Name = model.Name.Trim()
        };

        _db.Genre.Add(genreEntity);
        _db.SaveChanges();

        return new SuccessResult("Genre added successfully.");
    }

    public Result Update(GenreModel model)
    {
        // Check for existing genre with the same name
        var existingGenres = _db.Genre.Where(g => g.id != model.Id).ToList();
        if (existingGenres.Any(g => g.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new ErrorResult("Genre with the same name already exists!");

        // Get genre entity to be updated
        var genreEntity = _db.Genre.SingleOrDefault(g => g.id == model.Id);
        if (genreEntity is null)
            return new ErrorResult("Genre not found!");

        // Update entity properties
        genreEntity.Name = model.Name.Trim();

        _db.Genre.Update(genreEntity);
        _db.SaveChanges();

        return new SuccessResult("Genre updated successfully.");
    }

    public Result DeleteGenre(int id)
    {
        // Get genre record with associated movie genres
        var genreEntity = _db.Genre.Include(g => g.MovieGenres).SingleOrDefault(g => g.id == id);
        if (genreEntity is null)
            return new ErrorResult("Genre not found!");

        // Delete associated movie genres
        _db.MovieGenre.RemoveRange(genreEntity.MovieGenres);

        // Delete the genre record
        _db.Genre.Remove(genreEntity);

        _db.SaveChanges();

        return new SuccessResult("Genre deleted successfully.");
    }

    public List<GenreModel> GetList()
    {
        // since we wrote the Query method above, we should call it
        // and return the result as a list by calling ToList method
        return Query().ToList();
    }

    // Way 1:
    //public ResourceModel GetItem(int id)
    //{
    //    return Query().SingleOrDefault(r => r.Id == id);
    //}
    // Way 2:
    public GenreModel GetItem(int id) => Query().SingleOrDefault(r => r.Id == id);
}
