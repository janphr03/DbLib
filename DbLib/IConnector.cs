using System;
using System.Data;


namespace DbLib
{
    public interface IConnector
    {

        public int openConnection();
        public int closeConnection();

        public DataTable select(string column, string tableName, string whereCondition, string orderBy);
        public int insert(string tableName, string values);
        public int update(string tableName, string set, string whereCondition, string join);
        public int delete(string tableName,  string wherecondition);

    }
}
