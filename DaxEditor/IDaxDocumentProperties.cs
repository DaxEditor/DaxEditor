using System;

namespace DaxEditor
{
    public interface IDaxDocumentProperties
    {
        string ConnectionString { get; set; }
        string ConnectionStringWithoutDatabaseName { get; }
        string DatabaseName { get; }
    }
}
