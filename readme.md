# 🏋️‍♂️ KanGainNET – Kompleksowy System Zarządzania Siłownią

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Azure](https://img.shields.io/badge/Microsoft_Azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-626CD9?style=for-the-badge&logo=Stripe&logoColor=white)

Kompleksowa, chmurowa aplikacja webowa do zarządzania klubem fitness, zbudowana w architekturze MVC. System automatyzuje procesy sprzedażowe, operacyjne oraz wspomaga interakcję między trenerami a klientami. Projekt został zrealizowany z zachowaniem dobrych praktyk programowania obiektowego w ekosystemie .NET.

## 👥 Autorzy
* **Patryk Romanowicz**
* **Romek** * *(Wpisz imię i nazwisko trzeciej osoby z grupy)*

---

## 🎯 Zrealizowane Funkcjonalności

Zgodnie z wymogami projektowymi, aplikacja posiada wdrożone następujące elementy:

* **Modele i ORM (Entity Framework Core):** Aplikacja opiera się na 19 encjach niesłownikowych (m.in. `Uzytkownik`, `Grafik`, `PlanTreningowy`, `Subskrypcja`). Wdrożono i poprawnie skonfigurowano relacje `One-to-One`, `One-to-Many` oraz `Many-to-Many`.
* **Operacje CRUD i Formularze:** Rozbudowane widoki pozwalające na tworzenie, odczyt, aktualizację i usuwanie danych. M.in. zarządzanie pracownikami, awansowanie/degradowanie użytkowników, przegląd i zakup karnetów.
* **Autoryzacja i Role (ClaimsIdentity):** Zaimplementowano bezpieczny moduł logowania z wykorzystaniem ASP.NET Core Identity. Obsługa 3 niezależnych ról:
  * *Niezalogowany:* Dostęp do publicznej galerii i cennika.
  * *Użytkownik:* Dostęp do zakupów (Stripe), rezerwacji i podglądu swoich karnetów.
  * *Moderator/Admin i Trener:* Zarządzanie pracownikami, zmiana ról, wgląd w plany treningowe.
* **Integracja z Zewnętrznym API:** Automatyczna obsługa zakupu karnetów z wykorzystaniem bezpiecznej bramki płatniczej **Stripe API** (tworzenie sesji, webhooki, przekierowania 303).
* **Wdrożenie na Produkcję:** Aplikacja jest w pełni hostowana i skonfigurowana w chmurze **Microsoft Azure Web Apps**.
* **Interfejs Użytkownika:** Estetyczny, nowoczesny wygląd z użyciem ciemnego motywu, dostosowany do urządzeń mobilnych dzięki responsywnemu systemowi Bootstrap 5 oraz CSS Grid (dynamiczna galeria zdjęć).

## 🛡️ Architektura i Bezpieczeństwo Danych

* **Zabezpieczenie Integralności Danych (Brak Cascade Delete):** W celu ochrony przed niekontrolowaną, lawinową utratą danych, w całym systemie globalnie wyłączono mechanizm kaskadowego usuwania (*Cascade Delete*). Zamiast tego, proces usuwania powiązanych rekordów jest precyzyjnie kontrolowany w kodzie C#. Przykładowo, przed usunięciem konta trenera, system ręcznie i bezpiecznie czyści przypisane do niego zajęcia w grafiku i plany treningowe (przy użyciu `RemoveRange`).
* **Ochrona przed Path Traversal:** Obrazy wyświetlane w systemie są serwowane przez dedykowaną akcję kontrolera z ukrytego folderu (poza katalogiem `wwwroot`), co zapobiega nieautoryzowanemu dostępowi do plików na serwerze.
* **Zarządzanie Sekretami:** Klucze API (Stripe) oraz hasła do bazy danych (Connection Strings) są wykluczone z repozytorium (bezpieczna obsługa przez `User Secrets` lokalnie oraz `Environment Variables` na Azure).

## 💻 Technologie

* **Backend:** C#, .NET 8, ASP.NET Core MVC
* **Baza Danych:** Microsoft SQL Server
* **Frontend:** HTML5, CSS3, Bootstrap 5
* **Hosting:** Microsoft Azure