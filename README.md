IIS Express TestKit
================

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

## License

[Apache License 2.0](https://github.com/shibayan/WinQuickLook/blob/master/LICENSE)