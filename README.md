# CinéBiblio — ASP.NET Core MVC (.NET 10)

Cas pratique « Bibliothèque de films », jusqu'à l'affichage moderne.

## Lancer
```bash
dotnet run
```
Puis ouvrir http://localhost:5080 (la base SQLite `filmlibrary.db` est créée et remplie au démarrage).

## Comptes de test
| Rôle  | Email             | Mot de passe |
|-------|-------------------|--------------|
| Admin | admin@films.local | Admin123!    |
| User  | user@films.local  | User123!     |

- **Admin** : CRUD complet (ajout, modification, suppression, upload d'affiche)
- **User** (et tout compte créé via « Inscription ») : consultation du catalogue et des détails

## Architecture (SOLID / DIP)
```
Controllers/FilmsController  -> reçoit la requête, orchestre
Services/IFilmService        -> contrat métier (+ FilmService, EF Core, async)
Services/IImageService       -> contrat upload/suppression d'images (+ ImageService)
Data/ApplicationDbContext    -> Identity + DbSet<Film>
Data/DbInitializer           -> rôles, comptes, films de démo
ViewModels/FilmFormViewModel -> validation du formulaire (+ IFormFile)
```

## Passer aux migrations (optionnel)
Le projet utilise `EnsureCreated` pour démarrer sans manipulation. Pour les migrations EF :
```bash
dotnet tool install --global dotnet-ef
rm filmlibrary.db
dotnet ef migrations add Initial
```
puis remplacer `EnsureCreatedAsync()` par `MigrateAsync()` dans `DbInitializer.cs`.
