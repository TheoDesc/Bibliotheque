using FilmLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FilmLibrary.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        var adminEmail = "admin@film-library.com";
        var adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                admin,
                adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
        else if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        var genres = new[]
        {
            "Action",
            "Comédie",
            "Drame",
            "Science-fiction",
            "Horreur",
            "Animation"
        };

        foreach (var genreName in genres)
        {
            if (!await context.Genres.AnyAsync(g => g.Name == genreName))
            {
                context.Genres.Add(new Genre
                {
                    Name = genreName
                });
            }
        }

        await context.SaveChangesAsync();

        if (!await context.Films.AnyAsync())
        {
            var action = await context.Genres
                .FirstAsync(g => g.Name == "Action");

            var comedy = await context.Genres
                .FirstAsync(g => g.Name == "Comédie");

            var drama = await context.Genres
                .FirstAsync(g => g.Name == "Drame");

            var sciFi = await context.Genres
                .FirstAsync(g => g.Name == "Science-fiction");

            var horror = await context.Genres
                .FirstAsync(g => g.Name == "Horreur");

            var animation = await context.Genres
                .FirstAsync(g => g.Name == "Animation");

            context.Films.AddRange(
                new Film
                {
                    Title = "Inception",
                    Director = "Christopher Nolan",
                    ReleaseYear = 2010,
                    DurationMinutes = 148,
                    GenreId = sciFi.Id,
                    Description = "Un voleur spécialisé dans l'extraction de secrets par les rêves doit accomplir une mission impossible.",
                    Prix = 12.99,
                    Stock = 5
                },
                new Film
                {
                    Title = "The Dark Knight",
                    Director = "Christopher Nolan",
                    ReleaseYear = 2008,
                    DurationMinutes = 152,
                    GenreId = action.Id,
                    Description = "Batman affronte le Joker, un criminel qui plonge Gotham dans le chaos.",
                    Prix = 14.99,
                    Stock = 4
                },
                new Film
                {
                    Title = "Intouchables",
                    Director = "Olivier Nakache",
                    ReleaseYear = 2011,
                    DurationMinutes = 112,
                    GenreId = comedy.Id,
                    Description = "Une amitié improbable entre un homme riche devenu tétraplégique et son aide à domicile.",
                    Prix = 9.99,
                    Stock = 6
                },
                new Film
                {
                    Title = "The Shawshank Redemption",
                    Director = "Frank Darabont",
                    ReleaseYear = 1994,
                    DurationMinutes = 142,
                    GenreId = drama.Id,
                    Description = "Un homme injustement condamné trouve espoir et amitié derrière les murs d'une prison.",
                    Prix = 11.99,
                    Stock = 3
                },
                new Film
                {
                    Title = "Alien",
                    Director = "Ridley Scott",
                    ReleaseYear = 1979,
                    DurationMinutes = 117,
                    GenreId = horror.Id,
                    Description = "L'équipage d'un vaisseau spatial rencontre une créature extraterrestre terrifiante.",
                    Prix = 10.99,
                    Stock = 4
                },
                new Film
                {
                    Title = "Le Roi Lion",
                    Director = "Roger Allers",
                    ReleaseYear = 1994,
                    DurationMinutes = 88,
                    GenreId = animation.Id,
                    Description = "Simba doit retrouver sa place de roi après la mort de son père.",
                    Prix = 8.99,
                    Stock = 7
                }
            );

            await context.SaveChangesAsync();
        }
    }
}