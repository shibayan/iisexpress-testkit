IIS Express TestKit
================
[![Build status](https://ci.appveyor.com/api/projects/status/v5kgu9runa70wum1?svg=true)](https://ci.appveyor.com/project/shibayan/iisexpress-testkit)
[![License](https://img.shields.io/github/license/shibayan/iisexpress-testkit.svg)](https://github.com/shibayan/iisexpress-testkit/blob/master/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/IisExpressTestKit.svg)](https://www.nuget.org/packages/IisExpressTestKit/)

## Getting Started

- Create new xUnit test project
- Install package from NuGet

```
Install-Package IisExpressTestKit
```

- Write test case

```csharp
[Fact]
public void RewriteRulesTest()
{
    Iis.Request("/hoge")
       .IsPath("/translated/hoge")
       .IsStatusCode(HttpStatusCode.OK);

    Iis.Request("/hoge/foo/bar/baz")
       .IsPath("/translated/hoge/foo/bar/baz")
       .IsStatusCode(HttpStatusCode.OK);
}
```

- Make happy :)

![Test Result](http://cdn-ak.f.st-hatena.com/images/fotolife/s/shiba-yan/20160619/20160619175112.png)

- Running AppVeyor CI

![AppVeyor](http://cdn-ak.f.st-hatena.com/images/fotolife/s/shiba-yan/20160713/20160713235842.png)

## License

[Apache License 2.0](https://github.com/shibayan/WinQuickLook/blob/master/LICENSE)
