# Cake.Microsoft.Extensions.Configuration
[![NuGet](https://img.shields.io/nuget/v/Cake.Microsoft.Extensions.Configuration.svg)](https://www.nuget.org/packages/Cake.Microsoft.Extensions.Configuration/)
[![License](http://img.shields.io/:license-mit-blue.svg)](http://cake-contrib.mit-license.org)

Cake.Microsoft.Extensions.Configuration is an Addin that extends [Cake](http://cakebuild.net/) for creating strongly typed configuration using [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration?tabs=basicconfiguration).

You can write your own `ConfigurationProvider` or use any of the configuration providers supported [out of the box](https://www.nuget.org/packages?q=Microsoft.Extensions.Configuration)

## General Notes

*__This is currently in alpha stage of development, feedback and contribution welcomed__*

Cake.Microsoft.Extensions.Configuration targets the following:

|||
|----|----|
|Cake|0.23.0|
|.NET Standard|1.6|

## Installation

Add the following reference to your cake build script:

```cake
#addin Cake.Microsoft.Extensions.Configuration
```

Or explicit NuGet reference:

```cake
#addin nuget:?package=Cake.Microsoft.Extensions.Configuration&version=<version>
```

> For reliable and repeatable builds, it is strongly advised to pin your addin version.

## Usage

In order to use Cake.Microsoft.Extensions.Configuration, you will need to use at least one of the Configuration Providers. By default, Cake.Microsoft.Extensions.Configuration uses the [In-Memory Collection Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration?tabs=basicconfiguration#in-memory-provider-and-binding-to-a-poco-class), Environment Variables Provider and the [Command Line Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration?tabs=basicconfiguration#commandline-configuration-provider);

## Questions

Feel free to open an [issue](https://github.com/JonCubed/cake-microsoft-extensions-configuration/issues).

## Share the love

If this project helps you in anyway then please :star: the repository.
