#Dockerfile for kingtech FormGenerator documentation (example) image

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN apk --no-cache add curl icu-libs libcap bash
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["KingTech.Web.FormGenerator.Abstract.NuGet/KingTech.Web.FormGenerator.Abstract.NuGet.csproj", "KingTech.Web.FormGenerator.Abstract.NuGet/"]
RUN dotnet restore "KingTech.Web.FormGenerator.Abstract.NuGet/KingTech.Web.FormGenerator.Abstract.NuGet.csproj" -a $TARGETARCH 
COPY  ["KingTech.Web.FormGenerator.Abstract.NuGet/", "KingTech.Web.FormGenerator.Abstract.NuGet/"]

COPY ["KingTech.Web.FormGenerator.NuGet/KingTech.Web.FormGenerator.NuGet.csproj", "KingTech.Web.FormGenerator.NuGet/"]
RUN dotnet restore "KingTech.Web.FormGenerator.NuGet/KingTech.Web.FormGenerator.NuGet.csproj" -a $TARGETARCH 
COPY  ["KingTech.Web.FormGenerator.NuGet/", "KingTech.Web.FormGenerator.NuGet/"]

COPY ["KingTech.Web.FormGenerator.Example/KingTech.Web.FormGenerator.Example.csproj", "KingTech.Web.FormGenerator.Example/"]
RUN dotnet restore "KingTech.Web.FormGenerator.Example/KingTech.Web.FormGenerator.Example.csproj" -a $TARGETARCH 
COPY  ["KingTech.Web.FormGenerator.Example/", "KingTech.Web.FormGenerator.Example/"]
WORKDIR "/src/KingTech.Web.FormGenerator.Example"
RUN dotnet build "KingTech.Web.FormGenerator.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KingTech.Web.FormGenerator.Example.csproj" -c Release -a $TARGETARCH --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KingTech.Web.FormGenerator.Example.dll"]