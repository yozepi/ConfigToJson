/* Copyright 2021 Joe Cleland (yozepi)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
namespace yozepi.ConfigToJson.UnitTests
{
    using FluentAssertions;
    using global::ConfigToJson;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;

    [TestClass]
    [DeploymentItem(ConfigurationExtensions_Specs.SETTINGS_FILE)]
    public class ConfigurationExtensions_Specs : nSpecTestHarness
    {

        [TestMethod]
        public void ConfigurationExtensionsSpecs()
        {
            this.RunMySpecs();
        }

        private const string SETTINGS_FILE = "./testSettings.json";

        IConfigurationRoot? config = null;
        JObject? loadedAsJson = null;

        IConfiguration? subject = null;
        JToken? actual = null;
        JToken? expected = null;

        void before_all()
        {
            // Load the config from the settings file.
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(SETTINGS_FILE, false);
            config = configBuilder.Build();

            // Read JSON directly from the settings file.
            using (StreamReader file = File.OpenText(SETTINGS_FILE))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                loadedAsJson = (JObject)JToken.ReadFrom(reader);
            }
        }

        void before_each()
        {
            subject = null;
            actual = null;
            expected = null;
        }

        void act_each()
        {
            actual = subject.ToJToken();
        }

        void JToken_Conversion_specs()
        {
            describe["converting the confuration root to a JToken..."] = () =>
            {
                beforeEach = () =>
                {
                    subject = config;
                    expected = loadedAsJson;
                };

                it["should convert the configuration into expected JToken instance"] = () =>
                {
                    JToken.DeepEquals(actual, expected).Should().BeTrue();
                };
            };

            describe["converting simple values..."] = () =>
            {
                describe["converting a string value..."] = () =>
                {
                    beforeEach = () =>
                    {
                        subject = config!.GetSection("stringSetting");
                        expected = loadedAsJson!["stringSetting"];
                    };

                    it["should return a JSON string"] = () =>
                    {
                        actual.As<JValue>()
                            .Type.Should().Be(JTokenType.String);
                        JToken.DeepEquals(actual, expected).Should().BeTrue();
                    };
                };

                describe["converting a numeric value..."] = () =>
                {
                    beforeEach = () =>
                    {
                        subject = config!.GetSection("numberSetting");
                        expected = loadedAsJson!["numberSetting"];
                    };

                    it["should return a JSON number"] = () =>
                    {
                        actual.As<JValue>()
                            .Type.Should().Be(JTokenType.Integer);
                        JToken.DeepEquals(actual, expected).Should().BeTrue();
                    };
                };

                describe["converting a boolean value..."] = () =>
                {
                    describe["when the value is true..."] = () =>
                    {
                        beforeEach = () =>
                        {
                            subject = config!.GetSection("boolSettingTrue");
                            expected = loadedAsJson!["boolSettingTrue"];
                        };

                        it["should return true"] = () =>
                        {
                            actual.As<JValue>()
                                .Value.Should().Be(true);
                        };
                    };
                    describe["when the value is false..."] = () =>
                    {
                        beforeEach = () =>
                        {
                            subject = config!.GetSection("boolSettingFalse");
                            expected = loadedAsJson!["boolSettingFalse"];
                        };

                        it["should return false"] = () =>
                        {
                            actual.As<JValue>()
                                .Value.Should().Be(false);
                        };
                    };
                };
                
                describe["converting an array..."] = () =>
                {
                    beforeEach = () =>
                    {
                        subject = config!.GetSection("simpleArray");
                        expected = loadedAsJson!["simpleArray"];
                    };

                    it["should return a JSON array"] = () =>
                    {
                        actual.As<JArray>()
                            .Type.Should().Be(JTokenType.Array);
                        JToken.DeepEquals(actual, expected).Should().BeTrue();
                    };
                };
            };

            describe["converting nested values..."] = () =>
            {
                beforeEach = () =>
                {
                    subject = config!.GetSection("nestedSettings");
                    expected = loadedAsJson!["nestedSettings"];
                };

                it["return the expected JToken value"] = () =>
                {
                    JToken.DeepEquals(actual, expected).Should().BeTrue();
                };
            };

            describe["converting a child configuration section..."] = () =>
            {
                beforeEach = () =>
                {
                    subject = config!.GetSection("deep:nestedSettings");
                    expected = loadedAsJson!["deep"]["nestedSettings"];
                };

                it["return the expected JToken value"] = () =>
                {
                    JToken.DeepEquals(actual, expected).Should().BeTrue();
                };

            };

            context["when the configuration is null..."] = () =>
            {
                beforeEach = () => subject = null;

                it["should throw ArgumentNullException"] = expect<ArgumentNullException>();
            };
        }
        void string_conversion_specs()
        {
            string? expectedString = null;
            string? actualString = null;
            
            beforeEach = () =>
            {
                subject = config!.GetSection("nestedSettings");
                expectedString = loadedAsJson!["nestedSettings"].ToString();

            };

            act = () => actualString = actual!.ToString();

            it["should convert configuration into the expected JSON string"] = () =>
            {
                JToken.DeepEquals(
                    JToken.Parse(actualString),
                    JToken.Parse(expectedString))
                    .Should().BeTrue();
            };
        }
    }
}
