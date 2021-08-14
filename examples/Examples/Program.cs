using ConfigToJson;
using Microsoft.Extensions.Configuration;
using System;

namespace Examples
{
    class Program
    {
        static Program()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            Configuration = configBuilder.Build();
        }

        static void Main(string[] args)
        {
            /*  Prints all the configuration
                {
                  "anArrayExample": [
                    "abc",
                    "def",
                    "ghi"
                  ],
                  "complexExample": {
                    "subSection": {
                      "subSetting": "Lorum Ipsum"
                    }
                  },
                  "mySimpleSetting": 42
                }
            */
            Console.WriteLine(Configuration.ToJsonString());
            Console.WriteLine();


            //Prints 42
            Console.WriteLine(Configuration.GetSection("mySimpleSetting").ToJsonString());
            Console.WriteLine();


            /*  Prints
                [
                  "abc",
                  "def",
                  "ghi"
                ]
            */
            Console.WriteLine(Configuration.GetSection("anArrayExample").ToJsonString());
            Console.WriteLine();


            /*  Prints
                {
                  "subSection": {
                    "subSetting": "Lorum Ipsum"
                  }
                }
            */
            Console.WriteLine(Configuration.GetSection("complexExample").ToJsonString());
            Console.WriteLine();


            /*  Prints
                {
                  "subSetting": "Lorum Ipsum"
                }
             */
            Console.WriteLine(Configuration.GetSection("complexExample:subSection").ToJsonString());
            Console.WriteLine();

            // Converts a configuration subsection into a JToken
            // and then modifies a value in the JToken.
            var token = Configuration.GetSection("complexExample:subSection").ToJToken();
            token["subSetting"] = "phi beta";

            /*  Prints
                {
                  "subSetting": "phi beta"
                }
             */
            Console.WriteLine(token.ToString());

        }

        private static IConfiguration Configuration { get; set; }
    }

}
