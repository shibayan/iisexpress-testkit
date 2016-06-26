IIS Express TestKit
================

## Getting Started

1. Create new xUnit test project
2. Package install from NuGet

```
Install-Package IisExpressTestKit
```

3. Write test case

```csharp
[Fact]
public void RewriteRuleTest()
{
    Iis.Request("/hoge")
       .IsPath("/translated/hoge")
       .IsStatusCode(HttpStatusCode.OK);

    Iis.Request("/hoge/foo/bar/baz")
       .IsPath("/translated/hoge/foo/bar/baz")
       .IsStatusCode(HttpStatusCode.OK);
}
```

4. Make happy :)

## License

[Apache License 2.0](https://github.com/shibayan/WinQuickLook/blob/master/LICENSE)