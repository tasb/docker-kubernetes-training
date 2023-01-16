FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

LABEL turma=6

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

WORKDIR /app
COPY ./publish .

ENTRYPOINT ["dotnet", "echo-api.dll"]
