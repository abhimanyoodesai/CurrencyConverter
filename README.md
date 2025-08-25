# CurrencyConverter

A .NET 8 application for converting currencies and tracking conversion history. This project provides APIs to fetch currency rates, perform conversions to DKK (Danish Krone), and store/retrieve conversion records.

## Features

- Fetch current currency rates
- Convert any supported currency to DKK
- Store and retrieve conversion history
- Filter conversion history by date and currency
- Background service for updating rates

## Technologies

- C# 12
- .NET 8
- Entity Framework Core
- ASP.NET Core Web API

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or another supported database

### Setup

1. Clone the repository:
 https://github.com/abhimanyoodesai/CurrencyConverter.git

2. Update `appsettings.json` with your database connection string.
 - if you are usig docker to setup sql server use below command 
   docker pull mcr.microsoft.com/mssql/server:2022-latest
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name sqlserver-2022 -d mcr.microsoft.com/mssql/server:2022-latest

3. Apply database migrations:
    - dotnet ef database update --project CurrencyConverter.DataAccess
    - Update-Database (if using the Package Manager Console).

5. Build and run the application:


## API Endpoints

- `GET /api/rates` - Get all currency rates
- `GET /api/rates/{currencyCode}` - Get rate for a specific currency
- `POST /api/convert` - Convert an amount to DKK
- `GET /api/conversions` - Get conversion history (supports filtering)

## Project Structure

- `Controllers/` - API controllers
- `Application/Interfaces/` - Service interfaces
- `Application/Services/` - Service implementations
- `DataAccess/` - Entity models, DbContext, and migrations
- `BackgroundJobs/` - Background services for periodic tasks
- `Tests/` - Unit and integration tests

## Note 
- Use the following credentials to obtain the token:
- Username: testUser
- Password: testPassword
