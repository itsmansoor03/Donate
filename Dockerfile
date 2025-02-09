# استخدم صورة رسمية لـ .NET Core SDK للبناء
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# نسخ ملفات المشروع واستعادة الـ dependencies
COPY *.csproj ./
RUN dotnet restore

# نسخ بقية ملفات المشروع وبنائه
COPY . ./
RUN dotnet publish -c Release -o out

# استخدم صورة رسمية لـ ASP.NET Core Runtime للتشغيل
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# تعيين البورت الافتراضي
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# أمر تشغيل التطبيق
CMD ["dotnet", "Donate.dll"]
