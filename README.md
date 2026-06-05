# 🧊 HomeCool

Self-hosted Webanwendung zur Verwaltung eines Gemeinschafts-Kühlschranks.  
Mitglieder buchen Getränke, Admins erfassen Einkäufe und monatliche Abrechnungen werden automatisch berechnet.

---

## Schnellstart (Docker)

```bash
# 1. Repository klonen / Ordner öffnen
cd homecool

# 2. JWT-Secret anpassen (unbedingt vor dem ersten Start!)
#    Öffne docker-compose.yml und ändere JWT_SECRET

# 3. Starten
docker compose up -d

# 4. Im Browser öffnen
open http://localhost:8080
```

### Demo-Zugangsdaten

| Name  | Passwort  | Rolle |
|-------|-----------|-------|
| Admin | admin123  | Admin |
| Alice | 1234      | User  |
| Bob   | 1234      | User  |

---

## Lokale Entwicklung

Voraussetzung: .NET 10 SDK

```bash
cd src/HomeCool.Api

# Entwicklungsserver starten
dotnet run

# Swagger UI
open http://localhost:5000/swagger
```

---

## Konfiguration

Alle Einstellungen können per Umgebungsvariable überschrieben werden:

| Variable             | Beschreibung                         | Standard                          |
|----------------------|--------------------------------------|-----------------------------------|
| `JWT_SECRET`         | JWT-Signing-Secret (min. 32 Zeichen) | *(Pflicht in Production)*         |
| `DB_PATH`            | Pfad zur SQLite-Datei                | `/data/homecool.db`               |
| `ASPNETCORE_ENVIRONMENT` | `Development` oder `Production`  | `Production`                      |

---

## Projektstruktur

```
homecool/
├── src/HomeCool.Api/
│   ├── Data/               # DbContext, Migrations, SeedData
│   ├── Entities/           # EF Core Entitäten
│   ├── DTOs/               # Request/Response-Objekte
│   ├── Services/           # Business-Logik
│   ├── Endpoints/          # Minimal API Endpunktgruppen
│   ├── Middleware/         # Exception-Handler
│   ├── wwwroot/            # Frontend (HTML/CSS/JS)
│   └── Program.cs          # App-Startup
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## API-Übersicht

| Methode | Route | Beschreibung |
|---------|-------|--------------|
| POST | `/api/auth/login` | Anmelden, JWT erhalten |
| GET | `/api/auth/me` | Aktuellen Benutzer abrufen |
| GET/POST/PATCH | `/api/users` | Benutzerverwaltung (Admin) |
| GET/POST/PATCH | `/api/products` | Produktverwaltung (Admin) |
| GET/POST | `/api/purchases` | Einkäufe (Admin) |
| GET/POST/DELETE | `/api/consumptions` | Konsumbuchungen |
| GET/POST | `/api/payments` | Zahlungen (Admin) |
| GET | `/api/reports/stock` | Aktueller Bestand |
| GET | `/api/reports/user-balance` | Salden aller Benutzer |
| GET | `/api/reports/monthly-summary` | Monatsabrechnung |
| GET | `/api/reports/export/monthly-summary` | CSV-Export |
| GET | `/health` | Health-Check |

---

## Geschäftsregeln

- **Einkauf** erhöht den Bestand der enthaltenen Produkte
- **Konsum** reduziert den Bestand – negativer Bestand nur mit Admin-Override (`allowNegativeStock: true`)
- **Historischer Preis** wird beim Buchen gespeichert (kein Preisänderungs-Problem)
- Nur **aktive Benutzer** und **aktive Produkte** können verwendet werden
- SQLite-Datei wird in Docker über ein Volume persistent gespeichert

---

## Features

- Mobile-first UI, optimiert für Touch-Bedienung
- JWT-Authentifizierung mit Rollen (Admin/User)
- Dashboard mit Bestand, letzten Buchungen und Salden
- Schnelle Konsumbuchung mit großen Touch-Buttons
- Einkaufserfassung mit mehreren Positionen
- Monatsabrechnung mit CSV-Export
- Swagger UI im Development-Modus
- Health-Endpoint für Monitoring
- Demo-Daten beim ersten Start
