
# How to use?


## Installation

````cmd
dotnet add package Awesome.Net.WritableOptions
````


## Configuration

```c#
services.ConfigureWritableOptions<MyOptions>(Configuration.GetSection("MySection"));
```

or use custom json file

```c#
services.ConfigureWritableOptions<MyOptions>(Configuration.GetSection("MySection"),"Resources/appsettings.custom.json");
```

## Usage

```c#
private readonly IWritableOptions<MyOptions> _options;

public MyClass(IWritableOptions<MyOptions> options)
{
    _options = options;
}
```

```c#
_options.Update(opt => {
    opt.Field1 = "value1";
    opt.Field2 = "value2";
});
```


See more:
[``How to update values into appsetting.json?``](https://stackoverflow.com/a/45986656)



# JsonFileHelper

### Methods

```c#
public class JsonFileHelper
{
    public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, Action<T> updateAction = null) where T : class, new();

    public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, T value);
}
```


## Usage

```c#
JsonFileHelper.AddOrUpdateSection(jsonFilePath: _jsonFilePath, sectionName: _sectionName, value: true);
```

or 

```c#
JsonFileHelper.AddOrUpdateSection<MyOptions>(jsonFilePath: _jsonFilePath, sectionName: _sectionName, opt => {
    opt.Field1 = "value1";
    opt.Field2 = "value2";
});
```