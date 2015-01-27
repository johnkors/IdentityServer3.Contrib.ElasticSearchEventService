Master build: [![Build status](https://ci.appveyor.com/api/projects/status/5o9shqnmao5ln18w/branch/master?svg=true)](https://ci.appveyor.com/project/JohnKorsnes/identityserverv3-contrib/branch/master)

# Contents

Implementation of IdentityServerV3s IEventService using Serilogs ElasticSearchSink to push IdentityServerv3 events on a format useful for Kibana.


## Usage

```
   var elasticUri = new Uri("http://your.elasticsearch.instance/");
   var options = new ElasticsearchSinkOptions(elasticUri);
   var eventService = new ElasticSearchEventService(options);
```

## Dependencies

 * Thinktecture.IdentityServer3 - http://www.nuget.org/packages/Thinktecture.IdentityServer3/
 * Serilog.Sinks.ElasticSearch - http://www.nuget.org/packages/Serilog.Sinks.ElasticSearch/
