using System;
using System.Linq;

namespace DaxEditor.Json
{
    using Microsoft.AnalysisServices.Tabular;

    public static class JsonUtilities
    {
        public static bool IsJson(string text)
        {
            return text[0] != '<';
        }
        
        public static Database Deserialize(string text)
        {
            return JsonSerializer.DeserializeDatabase(text);
        }

        public static string Serialize(Database database)
        {
            var options = new SerializeOptions();
            options.SplitMultilineStrings = true;
            return JsonSerializer.SerializeDatabase(database, options);
        }

        public static string SerializeCulture(Culture culture)
        {
            var options = new SerializeOptions();
            options.SplitMultilineStrings = true;
            return JsonSerializer.SerializeObject(culture, options);
        }

        public static string SerializeCultures(CultureCollection cultures)
        {
            if (cultures == null || cultures.Count == 0)
            {
                return string.Empty;
            }

            return $@"""cultures"": [
    {string.Join("," + Environment.NewLine, cultures.Select(culture => SerializeCulture(culture)))}
],
";
        }

        //Custom implementation
        /*
        public static Tabular.Database Deserialize(string text)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Tabular.Database>(text);
        }

        public static string Serialize(Tabular.Database database)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(database, Newtonsoft.Json.Formatting.Indented);
        }
        */
    }

    /*
    namespace Tabular
    {
        using Newtonsoft.Json;
        using System;
        using System.Collections.Generic;

        public class Annotation
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "value", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Value { get; set; }
        }

        public class Level
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "ordinal", Required = Required.Always)]
            public int Ordinal { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "column", Required = Required.Always)]
            public string Column { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Hierarchy
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "isHidden", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsHidden { get; set; }

            [JsonProperty(PropertyName = "displayFolder", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayFolder { get; set; }

            [JsonProperty(PropertyName = "levels", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Level> Levels { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Source
        {
            [JsonProperty(PropertyName = "type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "query", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Query { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "expression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Expression { get; set; }

            [JsonProperty(PropertyName = "dataSource", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DataSource { get; set; }
        }

        public class KPI
        {
            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "targetDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TargetDescription { get; set; }

            [JsonProperty(PropertyName = "targetFormatString", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TargetFormatString { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "targetExpression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TargetExpression { get; set; }

            [JsonProperty(PropertyName = "statusGraphic", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string StatusGraphic { get; set; }

            [JsonProperty(PropertyName = "statusDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string StatusDescription { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "statusExpression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string statusExpression { get; set; }

            [JsonProperty(PropertyName = "trendGraphic", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TrendGraphic { get; set; }

            [JsonProperty(PropertyName = "trendDescription", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TrendDescription { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "TrendExpression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TrendExpression { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Measure
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "expression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Expression { get; set; }

            [JsonProperty(PropertyName = "formatString", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string FormatString { get; set; }

            [JsonProperty(PropertyName = "isHidden", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsHidden { get; set; }

            [JsonProperty(PropertyName = "isSimpleMeasure", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsSimpleMeasure { get; set; }

            [JsonProperty(PropertyName = "displayFolder", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayFolder { get; set; }

            [JsonProperty(PropertyName = "kpi", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public KPI KPI { get; set; }

            [JsonProperty(PropertyName = "translatedCaption", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TranslatedCaption { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Partition
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "mode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Mode { get; set; }

            [JsonProperty(PropertyName = "dataView", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DataView { get; set; }

            [JsonProperty(PropertyName = "source", Required = Required.Always)]
            public Source Source { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Column
        {
            [JsonProperty(PropertyName = "type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "dataType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DataType { get; set; }

            [JsonProperty(PropertyName = "dataCategory", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DataCategory { get; set; }

            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "isKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsKey { get; set; }

            [JsonProperty(PropertyName = "isUnique", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsUnique { get; set; }

            [JsonProperty(PropertyName = "isNullable", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsNullable { get; set; }

            [JsonProperty(PropertyName = "alignment", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Alignment { get; set; }

            [JsonProperty(PropertyName = "tableDetailPosition", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public int? TableDetailPosition { get; set; }

            [JsonProperty(PropertyName = "isAvailableInMdx", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsAvailableInMdx { get; set; }

            [JsonProperty(PropertyName = "keepUniqueRows", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? KeepUniqueRows { get; set; }

            [JsonProperty(PropertyName = "displayOrdinal", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public int? DisplayOrdinal { get; set; }

            [JsonProperty(PropertyName = "sourceProviderType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string SourceProviderType { get; set; }

            [JsonProperty(PropertyName = "displayFolder", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayFolder { get; set; }

            [JsonProperty(PropertyName = "isDataTypeInferred", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsDataTypeInferred { get; set; }

            [JsonProperty(PropertyName = "isNameInferred", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsNameInferred { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "expression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Expression { get; set; }

            [JsonProperty(PropertyName = "columnOriginTable", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string ColumnOriginTable { get; set; }

            [JsonProperty(PropertyName = "columnOriginColumn", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string ColumnOriginColumn { get; set; }

            [JsonProperty(PropertyName = "isHidden", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsHidden { get; set; }

            [JsonProperty(PropertyName = "sourceColumn", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string SourceColumn { get; set; }

            [JsonProperty(PropertyName = "sortByColumn", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string SortByColumn { get; set; }

            [JsonProperty(PropertyName = "formatString", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string FormatString { get; set; }

            [JsonProperty(PropertyName = "summarizeBy", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string SummarizeBy { get; set; }

            [JsonProperty(PropertyName = "isDefaultImage", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsDefaultImage { get; set; }

            [JsonProperty(PropertyName = "isDefaultLabel", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsDefaultLabel { get; set; }

            [JsonProperty(PropertyName = "translatedCaption", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string TranslatedCaption { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Table
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "dataCategory", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DataCategory { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "isHidden", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsHidden { get; set; }

            [JsonProperty(PropertyName = "columns", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Column> Columns { get; set; }

            [JsonProperty(PropertyName = "partitions", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Partition> Partitions { get; set; }

            [JsonProperty(PropertyName = "measures", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Measure> Measures { get; set; }

            [JsonProperty(PropertyName = "hierarchies", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Hierarchy> Hierarchies { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class DataSource
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "connectionString", Required = Required.Always)]
            public string ConnectionString { get; set; }

            [JsonProperty(PropertyName = "impersonationMode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string ImpersonationMode { get; set; }

            [JsonProperty(PropertyName = "account", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Account { get; set; }

            [JsonProperty(PropertyName = "password", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Password { get; set; }

            [JsonProperty(PropertyName = "maxConnections", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public int? MaxConnections { get; set; }

            [JsonProperty(PropertyName = "isolation", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Isolation { get; set; }

            [JsonProperty(PropertyName = "timeout", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public int? Timeout { get; set; }

            [JsonProperty(PropertyName = "provider", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Provider { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class TranslationModel
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "tables", Required = Required.Always)]
            public IList<Table> Tables { get; set; }
        }

        public class Translation
        {
            [JsonProperty(PropertyName = "model", Required = Required.Always)]
            public TranslationModel Model { get; set; }
        }

        public class Culture
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "translations", Required = Required.Always)]
            public Translation Translations { get; set; }
        }

        public class Perspective
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "tables", Required = Required.Always)]
            public IList<Table> Tables { get; set; }

        }

        public class TablePermission
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "filterExpression", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string FilterExpression { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class RoleMember
        {
            [JsonProperty(PropertyName = "memberName", Required = Required.Always)]
            public string MemberName { get; set; }

            [JsonProperty(PropertyName = "memberId", Required = Required.Always)]
            public string MemberId { get; set; }

            [JsonProperty(PropertyName = "identityProvider", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string IdentityProvider { get; set; }

            [JsonProperty(PropertyName = "memberType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string MemberType { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Role
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "modelPermission", Required = Required.Always)]
            public string ModelPermission { get; set; }

            [JsonProperty(PropertyName = "members", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<RoleMember> Members { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Relationship
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "crossFilteringBehavior", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string CrossFilteringBehavior { get; set; }

            [JsonProperty(PropertyName = "joinOnDateBehavior", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string JoinOnDateBehavior { get; set; }

            [JsonProperty(PropertyName = "relyOnReferentialIntegrity", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? RelyOnReferentialIntegrity { get; set; }

            [JsonProperty(PropertyName = "securityFilteringBehavior", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string SecurityFilteringBehavior { get; set; }

            [JsonProperty(PropertyName = "fromCardinality", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string FromCardinality { get; set; }

            [JsonProperty(PropertyName = "toCardinality", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string ToCardinality { get; set; }

            [JsonProperty(PropertyName = "fromTable", Required = Required.Always)]
            public string FromTable { get; set; }

            [JsonProperty(PropertyName = "fromColumn", Required = Required.Always)]
            public string FromColumn { get; set; }

            [JsonProperty(PropertyName = "toTable", Required = Required.Always)]
            public string ToTable { get; set; }

            [JsonProperty(PropertyName = "toColumn", Required = Required.Always)]
            public string ToColumn { get; set; }

            [JsonProperty(PropertyName = "isActive", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public bool? IsActive { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class Model
        {
            [JsonProperty(PropertyName = "name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }

            [JsonConverter(typeof(StringObjectConverter))]
            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "storageLocation", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string StorageLocation { get; set; }

            [JsonProperty(PropertyName = "defaultMode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DefaultMode { get; set; }

            [JsonProperty(PropertyName = "defaultDataView", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string DefaultDataView { get; set; }

            [JsonProperty(PropertyName = "culture", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Culture { get; set; }

            [JsonProperty(PropertyName = "collation", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Collation { get; set; }

            [JsonProperty(PropertyName = "dataSources", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<DataSource> DataSources { get; set; }

            [JsonProperty(PropertyName = "tables", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Table> Tables { get; set; }

            [JsonProperty(PropertyName = "relationships", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Relationship> Relationships { get; set; }

            [JsonProperty(PropertyName = "cultures", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Culture> Cultures { get; set; }

            [JsonProperty(PropertyName = "perspectives", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Perspective> Perspectives { get; set; }

            [JsonProperty(PropertyName = "roles", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Role> Roles { get; set; }

            [JsonProperty(PropertyName = "annotations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public IList<Annotation> Annotations { get; set; }
        }

        public class StringObjectConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                object jsonValue = null;
                var jsonString = value as string;
                if (jsonString != null)
                {
                    var array = jsonString.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
                    jsonValue = array.Length > 1 ? (object)array : jsonString;
                }
                serializer.Serialize(writer, jsonValue ?? value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    return string.Join("\n", serializer.Deserialize<IList<string>>(reader));
                }

                return serializer.Deserialize<string>(reader);
            }

            public override bool CanRead {
                get { return true; }
            }

            public override bool CanConvert(Type objectType)
            {
                return true;
            }
        }

        public class Database
        {
            [JsonProperty(PropertyName = "name", Required = Required.Always, Order = 0)]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "description", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "compatibilityLevel", Required = Required.Always, Order = 1)]
            public int CompatibilityLevel { get; set; }

            [JsonProperty(PropertyName = "model", Required = Required.Always, Order = 2)]
            public Model Model { get; set; }

            [JsonProperty(PropertyName = "id", Required = Required.Always, Order = 3)]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "readWriteMode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string ReadWriteMode { get; set; }
        }

    }
    */
}