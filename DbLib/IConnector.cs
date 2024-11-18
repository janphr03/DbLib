using System;
using System.Data;


namespace DbLib
{
    public interface IConnector
    {
        public errorValues openConnection();
        public errorValues closeConnection();
        public errorValues executeQuery(string query);
        public errorValues select(string column, string tableName, string whereCondition, string orderBy);

        public errorValues insert(string tableName, string values);
        public errorValues update (string tableName, string set, string whereCondition, string join);
        public errorValues delete(string tableName,  string wherecondition, string limit);

    }
}
