# استخدم صورة .NET SDK للبناء
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# نسخ ملف المشروع واستعادة الـ dependencies
COPY donate/*.csproj ./
RUN dotnet restore

# نسخ باقي ملفات المشروع وبنائه
COPY donate/. ./
RUN dotnet publish -c Release -o /app/out

# استخدم صورة Runtime لتشغيل التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# تعيين البورت الافتراضي
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# تشغيل التطبيق
CMD ["dotnet", "donate.dll"]
