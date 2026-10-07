using FilmLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Data;

public static class DbInitializer
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        await db.Database.EnsureCreatedAsync();

        // Création des rôles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in new[] { AdminRole, UserRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Films de démonstration
        if (!await db.Films.AnyAsync())
        {
            db.Films.AddRange(
                new Film
                {
                    Title = "Inception",
                    Director = "Christopher Nolan",
                    Genre = "Science-fiction",
                    ReleaseYear = 2010,
                    DurationMinutes = 148,
                    Description = "Un voleur spécialisé dans l'extraction de secrets à l'intérieur des rêves reçoit une mission inverse : implanter une idée.",
                    Prix = 4.99
                },
                new Film
                {
                    Title = "Le Parrain",
                    Director = "Francis Ford Coppola",
                    Genre = "Drame",
                    ReleaseYear = 1972,
                    DurationMinutes = 175,
                    Description = "La saga d'une famille de la mafia new-yorkaise et de la passation du pouvoir entre un père et son fils.",
                    Prix = 3.99
                },
                new Film
                {
                    Title = "Parasite",
                    Director = "Bong Joon-ho",
                    Genre = "Thriller",
                    ReleaseYear = 2019,
                    DurationMinutes = 132,
                    Description = "Une famille pauvre s'infiltre peu à peu dans la vie d'une famille aisée.",
                    Prix = 4.99
                },
                new Film
                {
                    Title = "Le Voyage de Chihiro",
                    Director = "Hayao Miyazaki",
                    Genre = "Animation",
                    ReleaseYear = 2001,
                    DurationMinutes = 125,
                    Description = "Une fillette se retrouve piégée dans un monde d'esprits et doit travailler pour sauver ses parents.",
                    Prix = 3.99
                },
                new Film
                {
                    Title = "Mad Max: Fury Road",
                    Director = "George Miller",
                    Genre = "Action",
                    ReleaseYear = 2015,
                    DurationMinutes = 120,
                    Description = "Une course-poursuite dans le désert pour échapper à un tyran.",
                    Prix = 4.99
                },
                new Film
                {
                    Title = "Amélie Poulain",
                    Director = "Jean-Pierre Jeunet",
                    Genre = "Comédie",
                    ReleaseYear = 2001,
                    DurationMinutes = 122,
                    Description = "Une jeune serveuse parisienne décide d'améliorer la vie de ceux qui l'entourent.",
                    Prix = 3.99
                });

            await db.SaveChangesAsync();
        }

        // Ajoute une image aux films qui n'en ont pas
        var withoutPoster = await db.Films
            .Where(f => f.PosterPath == null)
            .ToListAsync();

        foreach (var film in withoutPoster)
        {
            film.PosterPath =
                $"https://picsum.photos/seed/{Uri.EscapeDataString(film.Title)}/400/600";
        }

        await db.SaveChangesAsync();
    }
}