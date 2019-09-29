FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic AS build

WORKDIR /source

# Download & Unpack latest Starcounter 3.0.0
RUN apt-get update \
	&& apt-get install -y wget unzip

RUN mkdir artifacts
RUN wget https://starcounter.io/Starcounter/Starcounter.3.0.0-alpha-20190930.zip -O ./artifacts/Starcounter.zip
RUN unzip ./artifacts/Starcounter.zip -d ./artifacts

# Copy source files
RUN mkdir App
COPY ./App/nuget.config ./App/nuget.config
COPY ./App/Program.cs ./App/Program.cs
COPY ./App/App.csproj ./App/App.csproj

WORKDIR /source/App
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-bionic AS runtime

# Install Starcounter dependencies
RUN apt-get update \
	&& apt-get install -y wget unzip \
	&& apt-get install -y software-properties-common \
	&& apt-get install -y libaio1 \
	&& add-apt-repository -y ppa:swi-prolog/stable \
	&& apt-get update \
	&& apt-get install -y swi-prolog-nox=7.\*

ENV ASPNETCORE_URLS http://+:8080
WORKDIR /source/publish
COPY --from=build /source/App/out ./

ENTRYPOINT ["dotnet", "App.dll"]
