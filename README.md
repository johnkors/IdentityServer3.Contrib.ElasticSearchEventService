master:[![master](https://ci.appveyor.com/api/projects/status/5o9shqnmao5ln18w/branch/master?svg=true)](https://ci.appveyor.com/project/JohnKorsnes/identityserverv3-contrib/branch/master)
dev:[![dev](https://ci.appveyor.com/api/projects/status/5o9shqnmao5ln18w/branch/dev?svg=true)](https://ci.appveyor.com/project/JohnKorsnes/identityserverv3-contrib/branch/dev)
[![NuGet Stable](http://img.shields.io/nuget/v/IdentityServer3.ElasticSearchEventService.svg?style=flat)](https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService/)
[![NuGet Prerelease](https://img.shields.io/nuget/vpre/IdentityServer3.ElasticSearchEventService.svg)](https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService/)
[![Downloads](https://img.shields.io/nuget/dt/IdentityServer3.ElasticSearchEventService.svg)](https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService/)

# Contents

Implementation of IdentityServerV3s IEventService using Serilogs ElasticSearchSink to push IdentityServerv3 events on a format useful for Kibana.


## Usage

```
   var elasticUri = new Uri("http://your.elasticsearch.instance/");
   var options = new ElasticsearchSinkOptions(elasticUri);
   var eventService = new ElasticSearchEventService(options);
```

Also support for custom mapping through the ```MappingConfigurationBuilder``` class.

```
    var configuration = new MappingConfigurationBuilder()
        .DetailMaps(b => b
            .For<AccessTokenIssuedDetails>(t => t
                .Map(d => d.ClientId)
                .Map("ScopeCount", d => d.Scopes.Count())
                .MapRemainingMembersAsJson()
            )
            .DefaultMapAllMembers()
        )
        .AlwaysAdd("key", "value")
        .AlwaysAdd("WeekDay", () => DateTime.Now.DayOfWeek)
        .GetConfiguration();
    
    var eventMapper = new DefaultLogEventMapper(configuration);
    var elasticUri = new Uri("http://your.elasticsearch.instance/");
    var options = new ElasticsearchSinkOptions(elasticUri);
    var eventService = new ElasticSearchEventService(options, eventMapper);
```

where a simple implementation is

```
    public class MyOwnPropertiesAdder : IAddExtraPropertiesToEvents
    {
        public IDictionary<string, string> GetNonIdServerFields()
        {
            return new Dictionary<string, string>{ { "SomeFieldName", "SomeValue" } };
        }
    }

```

This example adds ```SomeFieldName : "SomeValueIWantOnEveryEvent"``` to every log statement sent to ElasticSearch.



## Install

```
  PM> Install-Package IdentityServer3.ElasticSearchEventService
```

NuGet:
https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService


Kibana screenshots
![Kibana](https://cloud.githubusercontent.com/assets/206726/5944395/eafc0ee2-a726-11e4-9238-805555b60165.png)

## Dependencies

 * Thinktecture.IdentityServer3 - http://www.nuget.org/packages/Thinktecture.IdentityServer3/
 * Serilog.Sinks.ElasticSearch - http://www.nuget.org/packages/Serilog.Sinks.ElasticSearch/
 * Newtonsoft.Json - https://www.nuget.org/packages/Newtonsoft.Json/
