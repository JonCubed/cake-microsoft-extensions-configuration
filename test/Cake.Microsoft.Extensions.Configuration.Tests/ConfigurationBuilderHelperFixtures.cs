using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace Cake.Microsoft.Extensions.Configuration.Tests
{
    public sealed class ConfigurationBuilderHelperFixtures
    {
        public sealed class TheLoadConfigurationMethod : IDisposable
        {
            private const string settingsFile = "build-settings.json";

            public void Dispose()
            {
                if (File.Exists(settingsFile))
                {
                    File.Delete(settingsFile);
                }
            }

            [Fact]
            public void Should_ReturnConfigurationRoot_If_NoArgs()
            {
                // Arrange
                CreateSettingsFile(new { });
                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy();

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();
            }

            [Fact]
            public void Should_ReturnConfigurationRoot_If_NoSettingsFileFound()
            {
                // Arrange
                var args = new[] { "arg1", "arg2" };
                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy();

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction, args);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();
            }

            [Theory]
            [InlineData("number", "100")]
            [InlineData("string", "once upon a time")]
            [InlineData("null", null)]
            public void Should_ReturnConfigurationRootLoadedWithInMemoryValues_If_GivenLocalConfiguration(string key, string value)
            {
                // Arrange
                var inMemoryConfiguration = new Dictionary<string, string>
                {
                    [key] = value
                };

                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy(inMemoryConfiguration);

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();
                builder.GetValue<string>(key).Should().Be(value);
            }

            [Theory]
            [InlineData(100)]
            [InlineData("once upon a time")]
            [InlineData(null)]
            public void Should_ReturnConfigurationRootLoadedWithJsonFileValues_If_JsonFileExists(object value)
            {
                // Arrange
                CreateSettingsFile(new { key = value });
                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy();

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();
                builder.GetValue(value?.GetType() ?? typeof(object), "key").Should().Be(value ?? string.Empty);
            }

            [Theory]
            [InlineData("number", "100")]
            [InlineData("string", "once upon a time")]
            [InlineData("null", null)]
            public void Should_ReturnConfigurationRootLoadedWithEnvironmentVariables_If_EnvironmentVariablesExists(string key, string value)
            {
                // Arrange
                CreateEnvironmentVariable(key, value);
                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy();

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();
                builder.GetValue<string>(key).Should().Be(value);
            }

            [Theory]
            [InlineData("number", "100")]
            [InlineData("string", "once upon a time")]
            [InlineData("null", null)]
            public void Should_ReturnConfigurationRootLoadedWithCommandLineVariables_If_GivenCommandLineVariables(string key, string value)
            {
                // Arrange
                CreateEnvironmentVariable(key, value);
                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy();

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();
                builder.GetValue<string>(key).Should().Be(value);
            }

            [Fact]
            public void Should_ReturnConfigurationRootLoadedWithSettingsOverridenCorrectly_If_GivenMoreThanOneSource()
            {
                // Arrange
                var inMemorySettings = new Dictionary<string, string>
                {
                    ["Key1"] = "Default1",
                    ["Key2"] = "Default2",
                    ["Key3"] = "Default3",
                    ["Key4"] = "Default4",
                };
                CreateSettingsFile(new { Key2 = 100, Key3 = "File3", Key4 = "File4" });
                CreateEnvironmentVariable("Key3", "Environment3");
                CreateEnvironmentVariable("Key4", "Environment4");
                var args = new[] { "--Key4", "Command4" };
                var initialiseAction = ConfigurationBuilderHelper.DefaultLoadConfigurationStrategy(inMemorySettings);

                // Act
                var builder = ConfigurationBuilderHelper.LoadConfiguration(initialiseAction, args);

                // Assert
                builder.Should().NotBeNull();
                builder.Should().BeOfType<ConfigurationRoot>();

                builder.GetValue<string>("Key1").Should().Be("Default1");
                builder.GetValue<int>("Key2").Should().Be(100);
                builder.GetValue<string>("Key3").Should().Be("Environment3");
                builder.GetValue<string>("Key4").Should().Be("Command4");
            }

            private void CreateSettingsFile(object settings)
            {
                File.WriteAllText(settingsFile, JsonConvert.SerializeObject(settings));
            }

            private void CreateEnvironmentVariable(string key, string value)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
