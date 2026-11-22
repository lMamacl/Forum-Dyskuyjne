# Kompendium Projektu: Architektura, Przepływ i Działanie Szkieletu ASP.NET Core MVC

Dokument ten stanowi techniczną instrukcję oraz ściągawkę z działania aplikacji, wyjaśniającą przepływ od momentu uruchomienia serwera do wyrenderowania gotowej strony w przeglądarce.

---

## 1. Cykl życia aplikacji — punkt wejścia: `Program.cs`

Każda aplikacja ASP.NET Core rozpoczyna swoje działanie w pliku `Program.cs`. To tutaj konfigurujemy usługi oraz potok przetwarzania żądań HTTP (HTTP Request Pipeline / Middleware).

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja usług w kontenerze zależności (IoC / Dependency Injection)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 2. Konfiguracja potoku middleware (kolejność jest kluczowa!)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### Co robi każda z tych sekcji?

1. **`WebApplication.CreateBuilder(args)`**:
   - Inicjalizuje środowisko hosta.
   - Wczytuje konfigurację z `appsettings.json`, zmiennych środowiskowych i argumentów wiersza poleceń.
   - Tworzy kontener wstrzykiwania zależności (Dependency Injection / IoC Container).
2. **`builder.Services.AddControllersWithViews()`**:
   - Rejestruje w kontenerze zależności wszystkie mechanizmy potrzebne do działania wzorca MVC: obsługę kontrolerów, silnik widoków Razor, mechanizmy bindowania modeli (`ModelBinding`) oraz walidacji (`Data Annotations`).
3. **`var app = builder.Build()`**:
   - Zamyka etap rejestracji usług i kompiluje potok aplikacji.
4. **Potok Middleware (Warstwy pośredniczące)**:
   - **`app.UseStaticFiles()`**: Przechwytuje zapytania o pliki fizyczne znajdujące się w folderze `wwwroot/` (np. arkusze CSS, skrypty JS, ikony). Nadaje im odpowiedni nagłówek `Content-Type` (np. `text/css`) i zwraca ich zawartość do przeglądarki bez angażowania kontrolerów.
     > **Ważna uwaga techniczna (UseStaticFiles vs MapStaticAssets):**  
     > W .NET 9/10 pojawiła się metoda `MapStaticAssets()`, która opiera się na kompilowanym manifeście endpointów. W klasycznych aplikacjach MVC bezpieczniejszym i bardziej przewidywalnym rozwiązaniem dla bibliotek lokalnych (Bootstrap, jQuery) jest tradycyjne `app.UseStaticFiles()`.
   - **`app.UseRouting()`**: Analizuje adres URL zapytania (np. `/Home/Index`) i dopasowuje go do zdefiniowanych tras.
   - **`app.UseAuthorization()`**: Weryfikuje uprawnienia użytkownika (np. czy ma dostęp do danej akcji oznaczonej atrybutem `[Authorize]`).
   - **`app.MapControllerRoute(...)`**: Definiuje domyślną regułę adresowania:
     - Wzorzec: `{controller=Home}/{action=Index}/{id?}`
     - Jeśli użytkownik wejdzie pod adres bazowy (np. `localhost:5000/`), serwer przyjmuje wartości domyślne: kontroler = `Home`, akcja = `Index`, a parametr `id` jest opcjonalny.
5. **`app.Run()`**:
   - Uruchamia serwer Kestrel i zaczyna nasłuchiwać na skonfigurowanych portach (np. 5000 / 7001).

---

## 2. Pliki konfiguracyjne widoków Razor: `_ViewImports` i `_ViewStart`

Silnik Razor automatycznie wyszukuje te dwa pliki w katalogu `Views/` przed renderowaniem jakiejkolwiek podstrony.

### A. `Views/_ViewImports.cshtml` (Importy globalne)

```razor
@using Forum_Dyskusyjne
@using Forum_Dyskusyjne.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

- **Rola:** Działa kaskadowo dla wszystkich plików `.cshtml` w folderze `Views/` i jego podfolderach.
- **`@using Forum_Dyskusyjne.Models`**: Pozwala na używanie krótkich nazw typów (np. `@model PostViewModel` zamiast pełnej ścieżki z przestrzenią nazw).
- **`@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`**: Aktywuje Tag Helpery ASP.NET Core.
  - Zamiast pisać surowy link HTML `<a href="/Forum/Index">` lub przestarzały kod `@Html.ActionLink("Fora", "Index", "Forum")`, piszemy naturalny kod:
    `<a asp-controller="Forum" asp-action="Index">Fora</a>`.
  - Serwer sam wygeneruje poprawny atrybut `href` w zależności od konfiguracji routingu.

### B. `Views/_ViewStart.cshtml` (Szablon startowy)

```razor
@{
    Layout = "_Layout";
}
```

- **Rola:** Kod ten wykonuje się automatycznie przed wyrenderowaniem każdego widoku.
- Mówi silnikowi Razor: _„każdy widok owiń domyślnie w szablon bazowy `Views/Shared/_Layout.cshtml`”_.
- Dzięki temu w widokach takich jak `Index.cshtml` nie musimy powtarzać deklaracji `<html>`, `<head>`, menu ani stopki. Jeśli jakaś podstrona nie ma mieć menu (np. okno modalne), w danym pliku można to nadpisać: `@{ Layout = null; }`.

---

## 3. Główny szkielet strony: `Views/Shared/_Layout.cshtml`

Plik `_Layout.cshtml` stanowi szkielet HTML dla całej aplikacji.

### Kluczowe składowe pliku:

1. **Sekcja `<head>`**:
   ```html
   <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
   ```

   - Znak tyldy `~` oznacza katalog główny aplikacji (`wwwroot/`).
2. **Pasek nawigacyjny Bootstrap**:
   - Klasy: `navbar navbar-expand-lg navbar-dark bg-dark` oznaczają:
     - `navbar`: komponent paska nawigacji.
     - `navbar-expand-lg`: na dużych ekranach (desktop) menu jest w pełni rozwinięte; na mniejszych (smartfony) zwija się do ikony „hamburgera”.
     - `navbar-dark bg-dark`: ciemne tło z jasnymi tekstami i ikonami.
   - Tag Helpery nawigacji:
     ```html
     <a class="navbar-brand" asp-controller="Home" asp-action="Index"
       >Forum Dyskusyjne</a
     >
     <a class="nav-link" asp-controller="Forum" asp-action="Index">Fora</a>
     ```
3. **Punkt wstrzykiwania podstron — `@RenderBody()`**:
   ```html
   <main role="main" class="container my-4 flex-grow-1">@RenderBody()</main>
   ```

   - **`@RenderBody()`** to najważniejsza dyrektywa w szablonie. W tym dokładnie miejscu silnik Razor wkleja zawartość generowaną przez konkretny widok (np. `Index.cshtml`).
4. **Sekcja skryptów — `@await RenderSectionAsync(...)`**:
   ```html
   <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
   @await RenderSectionAsync("Scripts", required: false)
   ```

   - Pozwala poszczególnym podstronom na dopisanie własnych skryptów na końcu dokumentu (np. skryptów walidacji formularzy w formularzu rejestracji). Parametr `required: false` oznacza, że podstrona nie musi definiować tej sekcji.

---

## 4. Przykładowe żądanie HTTP krok po kroku (Diagram przepływu)

Gdy użytkownik wpisuje w przeglądarce `http://localhost:5000/`:

```
1. Przeglądarka wysyła GET /
      │
      ▼
2. Kestrel (Program.cs)
      │ ↳ UseStaticFiles: Czy żądanie dotyczy pliku fizycznego? (Nie)
      │ ↳ UseRouting: Dopasowanie do trasy domyślnej -> Controller: Home, Action: Index
      │ ↳ UseAuthorization: Czy akcja wymaga logowania? (Nie)
      ▼
3. HomeController.cs
      │ ↳ public IActionResult Index() => View();
      ▼
4. Silnik Razor
      │ ↳ 1. Uruchamia _ViewStart.cshtml -> Ustawia Layout = "_Layout"
      │ ↳ 2. Wczytuje _ViewImports.cshtml -> Importuje Tag Helpery i typy
      │ ↳ 3. Generuje HTML z Views/Home/Index.cshtml
      │ ↳ 4. Otwiera Views/Shared/_Layout.cshtml i wkleja wygenerowany HTML w miejsce @RenderBody()
      ▼
5. Złożona odpowiedź HTML5 wraca do przeglądarki
      │
      ▼
6. Przeglądarka parsuje HTML i dociąga:
      ↳ GET /lib/bootstrap/dist/css/bootstrap.min.css (obsługiwane przez UseStaticFiles)
      ↳ GET /lib/bootstrap/dist/js/bootstrap.bundle.min.js (obsługiwane przez UseStaticFiles)
```
