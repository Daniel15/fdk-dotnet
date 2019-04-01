FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Examples/Basic/Basic.csproj", "/src/Examples/Basic/"]

# This is only required because we're building a local version of the FDK.
# Not required if using FDK via NuGet.
#COPY ["FnProject.Fdk", "."]

#WORKDIR /src/Examples/Basic
RUN dotnet restore "Examples/Basic/Basic.csproj"
COPY . .
RUN dotnet build "Examples/Basic/Basic.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Examples/Basic/Basic.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Basic.dll"]