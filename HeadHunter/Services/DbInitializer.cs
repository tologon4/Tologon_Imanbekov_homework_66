using HeadHunter.Models;
using Microsoft.EntityFrameworkCore;

namespace lesson65.Services;

public class DbInitializer
{
    private readonly ModelBuilder _modelBuilder;

    public DbInitializer(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }
    public void Seed()
    {
        _modelBuilder.Entity<Category>().HasData(
            new Category(){Id = 1, Name = "Программист"} ,
            new Category(){Id = 2, Name = "Бухгалтер"},
            new Category(){Id = 3, Name = "Экономист"} ,
            new Category(){Id = 4, Name = "Слесарь"} ,
            new Category(){Id = 5, Name = "Учитель"} ,
            new Category(){Id = 6, Name = "Доктор"} ,
            new Category(){Id = 7, Name = "Инженер"} , 
            new Category(){Id = 8, Name = "Водитель"} ,
            new Category(){Id = 9, Name = "Охраник"} ,
            new Category(){Id = 10, Name = "Садовник"} 
        );
    }
}