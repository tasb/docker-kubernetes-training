FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base

LABEL turma=2

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

WORKDIR /app
COPY ./publish .

ENTRYPOINT ["dotnet", "echo-api.dll"]
