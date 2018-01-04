[![Build Status](https://travis-ci.org/Mixaill/chtt.svg?branch=master)](https://travis-ci.org/Mixaill/chtt)

# Chtt
Simple ASP.Net Core chat with HTTP API



## Prerequirements

* .NET Core 2.0
* MSSQL

## How to build and use
- install .NET SDK ( http://microsoft.com/net )
- clone repo
- `cd ./chtt/`
- execute `dotnet restore`
- execute `dotnet ef database update`
- execute `dotnet run`

## How to get documentation
- run server
- navigate to `http://localhost:62087/swagger/` to view API docs

![Swagger](/docs/swagger.png?raw=true "Swagger")