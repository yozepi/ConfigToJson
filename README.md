# ConfigToJson
### A simple to use extension for converting IConfiguration into JSON

Use:
First, add this statement at the top of your c# file.
``` c#
using ConfigToJson;
```

Then just take any `IConfiguration` instance and convert into a JSON formatted string.
``` c#
string jsonString = Configuration.ToJsonString();

// It works on sub-configuration as well.
jsonstring = Configuration.GetSection("nested:sub:config");
```

There's a similar extension method for converting `IConfiguration` into a [NewtonSoft][newtonsoft] `JToken`.
``` c#
JToken token = Configuration.ToJToken();

// It works on sub-configuration as well.
token = Configuration.ToJToken("nested:sub:config");
```

### So why bother?
DotNet's configuration framework is already so rich. Why would you need to convert configuration into raw JSON? Well, in my case it was because of [Akka.Net][akka.net].

[Akka.Net][akka.net] is an amazing framework and if you've never hear of it you should definitely check it out. It has a minor flaw though (in my opinion) in that its configuration is [HOCON][hocon] based - which is incompatible with the dotnet's configuration framework. I suspect this is because [Akka.Net][akka.net] is a port of the Java based [Akka][akka] library.

Then I thought "Hmmm. HOCON is an extension of JSON. my appsettings.json file happens to be JSON." So This library was born. With it I can take full advantage of dotnet's configuration framework withn my [Akka.Net][akka.net] applications.

Anyway, That's why I find this library useful. I hope you find away to make it useful in your own applications as well.

Licensed under [Apache License, Version 2.0][license] so use it all you want.

[newtonsoft]: https://www.newtonsoft.com/json/help/html/Introduction.htm]
[akka.net]: https://getakka.net/index.html
[hocon]: https://en.wikipedia.org/wiki/HOCON
[akka]: https://akka.io/
[license]: ./LICENSE.txt