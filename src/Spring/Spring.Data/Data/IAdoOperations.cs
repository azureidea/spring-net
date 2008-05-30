#region License

/*
 * Copyright � 2002-2006 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Imports

using System;
using System.Collections;
using System.Data;
using Spring.Data.Common;

#endregion

namespace Spring.Data
{
	/// <summary>
	/// Interface that defines ADO.NET related database operations.
	/// </summary>
	/// <author>Mark Pollack (.NET)</author>
	/// <version>$Id: IAdoOperations.cs,v 1.24 2007/12/04 06:52:19 markpollack Exp $</version>
	public interface IAdoOperations : ICommonAdoOperations
	{        

        #region General Execute Methods with Callbacks
        /// <summary>
        /// Execute a ADO.NET operation on a command object using a delegate callback.
        /// </summary>
        /// <remarks>This allows for implementing arbitrary data access operations
        /// on a single command within Spring's managed ADO.NET environment.</remarks>
        /// <param name="del">The delegate called with a command object.</param>
        /// <returns>A result object returned by the action or null</returns>
        object Execute(CommandDelegate del);

        /// <summary>
        /// Execute a ADO.NET operation on a command object using an interface based callback.
        /// </summary>
        /// <param name="action">the callback to execute</param>
        /// <returns>object returned from callback</returns>
        object Execute(ICommandCallback action);
        
        /// <summary>
        /// Executes ADO.NET operations on a command object, created by the provided IDbCommandCreator,
        /// using the interface based callback IDbCommandCallback.
        /// </summary>
        /// <param name="commandCreator">The command creator.</param>
        /// <param name="action">The callback to execute based on IDbCommand</param>
        /// <returns>A result object returned by the action or null</returns>
        object Execute(IDbCommandCreator commandCreator, ICommandCallback action);

        /// <summary>
        /// Execute ADO.NET operations on a IDbDataAdapter object using an interface based callback.
        /// </summary>
        /// <remarks>This allows for implementing abritrary data access operations
        /// on a single DataAdapter within Spring's managed ADO.NET environment.
        /// </remarks>
        /// <param name="dataAdapterCallback">The data adapter callback.</param>
        /// <returns>A result object returned by the callback or null</returns>
	    object Execute(IDataAdapterCallback dataAdapterCallback);
	    
        #endregion 
	    

	    

	    
        #region Queries With ResultSetExtractor

	    // Static Queries

        /// <summary>
        /// Execute a query given IDbCommand's type and text, processing a 
        /// single result set with an instance of IResultSetExtractor
        /// </summary>
        /// <param name="cmdType">The type of command</param>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="resultSetExtractor">Object that will extract all rows of a result set</param>
        /// <returns>An arbitrary result object, as returned by the IResultSetExtractor</returns>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryWithResultSetExtractor(CommandType cmdType, string cmdText, IResultSetExtractor resultSetExtractor);


	    // Parameterized Queries
	    

        //only input parameters can be used...


        /// <summary>
        /// Execute a query given IDbCommand's type and text with parameters set
        /// via the command setter, processing a 
        /// single result set with an instance of IResultSetExtractor
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="resultSetExtractor">The result set extractor.</param>
        /// <param name="commandSetter">The command setter.</param>
        /// <returns>An arbitrary result object, as returned by the IResultSetExtractor</returns>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryWithResultSetExtractor(CommandType cmdType, string cmdText, IResultSetExtractor resultSetExtractor, 
                                           ICommandSetter commandSetter);

        //only input parameters can be used.
        object QueryWithResultSetExtractor(CommandType commandType, string cmdText, IResultSetExtractor resultSetExtractor,
                                           string parameterName, Enum dbType, int size, object parameterValue);

        //iterate over one or more result sets and then assign output parameters.
        object QueryWithResultSetExtractor(CommandType commandType, string cmdText, IResultSetExtractor resultSetExtractor,
                                           IDbParameters parameters);

	    
        #endregion

        #region Queries With ResultSetExtractorDelegate

        // Static Queries

        /// <summary>
        /// Execute a query given static SQL/Stored Procedure name 
        /// and process a single result set with an instance of ResultSetExtractorDelegate
        /// </summary>
        /// <param name="cmdType">The type of command.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="resultSetExtractorDelegate">Delegate that will extract all rows of a result set</param>
        /// <returns>An arbitrary result object, as returned by the IResultSetExtractor</returns>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>        
        object QueryWithResultSetExtractorDelegate(CommandType cmdType, string cmdText, ResultSetExtractorDelegate resultSetExtractorDelegate);


        // Parameterized Queries


        //only input parameters can be used...
        object QueryWithResultSetExtractorDelegate(CommandType commandType, string sql, ResultSetExtractorDelegate resultSetExtractorDelegate, 
                                                   ICommandSetter commandSetter);

        //only input parameters can be used.
        object QueryWithResultSetExtractorDelegate(CommandType commandType, string sql, ResultSetExtractorDelegate resultSetExtractorDelegate,
                                                   string parameterName, Enum dbType, int size, object parameterValue);

        //iterate over one or more result sets and then assign output parameters.
        object QueryWithResultSetExtractorDelegate(CommandType commandType, string sql, ResultSetExtractorDelegate resultSetExtractorDelegate, 
                                                   IDbParameters parameter);


        #endregion
	    
        #region Queries with RowMapperDelegate

        IList QueryWithRowMapperDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelgate);

        IList QueryWithRowMapperDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelgate, ICommandSetter commandSetter);

        IList QueryWithRowMapperDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelgate,
                                         string parameterName, Enum dbType, int size, object parameterValue);

        IList QueryWithRowMapperDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelgate, IDbParameters parameter);


        #endregion
	    
        #region Queries with RowMapper

        // Static Queries


        /// <summary>
        /// Execute a query given static SQL, mapping each row to a .NET object
        /// iva a RowMapper
        /// </summary>
        /// <param name="cmdType">The type of command</param>
        /// <param name="cmdText">SQL query to execute</param>
        /// <param name="rowMapper">object that will map one object per row</param>
        /// <returns>the result list containing mapped objects</returns>
        IList QueryWithRowMapper(CommandType cmdType, string cmdText, IRowMapper rowMapper);

	    // Parameterized Queries

        //Using IRowMapper to process 1 result set
        IList QueryWithRowMapper(CommandType cmdType, string cmdText, IRowMapper rowMapper, ICommandSetter commandSetter);

        IList QueryWithRowMapper(CommandType cmdType, string cmdText, IRowMapper rowMapper,
                    string name, Enum dbType, int size, object parameterValue);

        IList QueryWithRowMapper(CommandType cmdType, string cmdText, IRowMapper rowMapper, IDbParameters parameter);


	    
        #endregion

        #region Query for Object 



   	  
	    
        /// <summary>
        /// Execute a query with the specified command text, mapping a single result 
        /// row to an object via a RowMapper.
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapper">object that will map one object per row</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
	    object QueryForObject(CommandType cmdType, string cmdText, IRowMapper rowMapper);


        /// <summary>
        /// Execute a query with the specified command text and parameters set via the
        /// command setter, mapping a single result row to an object via a RowMapper.  
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapper">object that will map one object per row</param>
        /// <param name="commandSetter">The command setter.</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryForObject(CommandType cmdType, string cmdText, IRowMapper rowMapper, ICommandSetter commandSetter);

        /// <summary>
        /// Execute a query with the specified command text and parameters, mapping a single result row
        /// to an object via a RowMapper.
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapper">object that will map one object per row</param>
        /// <param name="parameters">The parameter collection to use in the query.</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryForObject(CommandType cmdType, string cmdText, IRowMapper rowMapper, IDbParameters parameters);

        /// <summary>
        /// Execute a query with the specified command text and parameter, mapping a single result row
        /// to an object via a RowMapper.
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapper">object that will map one object per row</param>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="dbType">One of the database parameter type enumerations.</param>
        /// <param name="size">The length of the parameter. 0 if not applicable to parameter type.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryForObject(CommandType cmdType, string cmdText, IRowMapper rowMapper,
                              string parameterName, Enum dbType, int size, object parameterValue);	    

	    
        /*
        /// <summary>
        /// Execute a query for a result list, given static SQL.
        /// </summary>
        /// <remarks>
        /// The results will be mapped to a List (one entry for each row)
        /// of IDictionaries (one entry for each column using he column 
        /// name as the key).  Each element in the list will be of the form
        /// returned by ths interfaces QueryForMap() methods.
        /// </remarks>
        /// <param name="cmdType">The type of the command.</param>
        /// <param name="sql">The SQL query to execute</param>
        /// <returns></returns>
	    IList QueryForList(CommandType cmdType, string sql);
        */
	    //IDictionary QueryForDictionary(CommandType cmdType, string sql);
        //TODO -
        //Object QueryForObject(CommandType cmdType, String sql, Type requiredType) throws DataAccessException;
	    //IDictionary QueryForDictionary
	    //IList QueryForList

        #endregion

        #region Query for ObjectDelegate

        /// <summary>
        /// Execute a query with the specified command text, mapping a single result 
        /// row to an object via a RowMapper.
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapperDelgate">delegate that will map one object per row</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryForObjectDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelgate);



        /// <summary>
        /// Execute a query with the specified command text and parameters set via the
        /// command setter, mapping a single result row to an object via a RowMapper.  
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapperDelegate">delegate that will map one object per row</param>
        /// <param name="commandSetter">The command setter.</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryForObjectDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelegate, ICommandSetter commandSetter);


        /// <summary>
        /// Execute a query with the specified command text and parameters, mapping a single result row
        /// to an object via a RowMapper.
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapperDelegate">delegate that will map one object per row</param>
        /// <param name="parameters">The parameter collection to use in the query.</param>
        /// <returns>The single mapped object.</returns>
        /// <exception cref="Spring.Dao.IncorrectResultSizeDataAccessException">
        /// If the query does not return exactly one row.
        /// </exception>
        /// <exception cref="Spring.Dao.DataAccessException">
        /// If there is any problem executing the query.
        /// </exception>
        object QueryForObjectDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelegate, IDbParameters parameters);


        /// <summary>
        /// Execute a query with the specified command text and parameter, mapping a single result row
        /// to an object via a RowMapper.
        /// </summary>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cmdText">The command text to execute.</param>
        /// <param name="rowMapperDelegate">delegate that will map one object per row</param>
        /// <param name="parameterName">The name of the parameter to map.</param>
        /// <param name="dbType">One of the database parameter type enumerations.</param>
        /// <param name="size">The length of the parameter. 0 if not applicable to parameter type.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns></returns>
        object QueryForObjectDelegate(CommandType cmdType, string cmdText, RowMapperDelegate rowMapperDelegate,
                                      string parameterName, Enum dbType, int size, object parameterValue);	    

	    
        #endregion

        #region Query With CommandCreator

        // Using IResultSetExtractor to process one result set.
        object QueryWithCommandCreator(IDbCommandCreator commandCreator, IResultSetExtractor resultSetExtractor);

	    IList QueryWithCommandCreator(IDbCommandCreator cc, IRowMapper rowMapper);

        // Using IResultSetExtractor to return one result set, multiple output parameters.
        object QueryWithCommandCreator(IDbCommandCreator commandCreator, IResultSetExtractor resultSetExtractor, IDictionary returnedParameters);

        // Using IRowMapper to return one result set, multiple output parameters.
        IList QueryWithCommandCreator(IDbCommandCreator commandCreator, IRowMapper rowMapper, IDictionary returnedParameters);
       	   
       
	    // Multiple return result sets, (A list of lists) 
	    // each with either a IRowMapper, IResultSetExtractor, IRowCallback
	    // and multiple output parameters.
	    IDictionary QueryWithCommandCreator(IDbCommandCreator commandCreator, IList resultProcessors);
       
	    //TODO
	    //Object QueryForObject(... Type requiredType)
        //IDictionary QueryForDictionary
        //IList QueryForList
	    
        #endregion
	    
        // ***************************************************************
	    // NB: Complex DataSet/DataTable operations are best handled with
	    //     DataSetOperations class in Spring.Data.Objects
	    // ***************************************************************
        
	    #region DataTable Create operations without parameters
	    
        DataTable DataTableCreate(CommandType commandType, string sql);
	    
        DataTable DataTableCreate(CommandType commandType, string sql,
                                  string tableMappingName);	  
	    
        DataTable DataTableCreate(CommandType commandType, string sql, 
                                  ITableMapping tableMapping);
	    
        DataTable DataTableCreate(CommandType commandType, string sql, 
                                  ITableMapping tableMapping,
                                  IDataAdapterSetter setter);	
	    
        #endregion
	    
        #region DataTable Create operations with parameters
	    
        DataTable DataTableCreateWithParams(CommandType commandType, string sql,
                                  IDbParameters parameters);
        
        DataTable DataTableCreateWithParams(CommandType commandType, string sql,
                                  IDbParameters parameters,
                                  string tableMappingName);	    

        DataTable DataTableCreateWithParams(CommandType commandType, string sql,
                                  IDbParameters parameters,
                                  ITableMapping tableMapping);
	    
        DataTable DataTableCreateWithParams(CommandType commandType, string sql, 
                                  IDbParameters parameters,
                                  ITableMapping tableMapping,
                                  IDataAdapterSetter dataAdapterSetter);	 
	    
	    
	    //TODO
	    /*
        DataTable DataTableCreateWithParams(IDbDataAdapterCreator dataAdapterCreator);
        */
	    
        #endregion
	    
        #region DataTable Fill operations without parameters
        /// <summary>
        /// Fill a  <see cref="DataTable"/> based on a select command that requires no parameters.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTable"/> to populate</param>
        /// <param name="commandType">The type of command</param>
        /// <param name="sql">SQL query to execute</param>
        /// <returns>The number of rows successfully added to or refreshed in the  <see cref="DataTable"/></returns>
	    int DataTableFill(DataTable dataTable, CommandType commandType, string sql);
	    
        int DataTableFill(DataTable dataTable, CommandType commandType, string sql,
                          string tableMappingName);	  
	    
        int DataTableFill(DataTable dataTable, CommandType commandType, string sql, 
                          ITableMapping tableMapping);
	    
        int DataTableFill(DataTable dataTable, CommandType commandType, string sql, 
                          ITableMapping tableMapping,
                          IDataAdapterSetter setter);
	        
	    
        #endregion
	    
        #region DataTable Fill operations with parameters
        
        int DataTableFillWithParams(DataTable dataTable, CommandType commandType, string sql,
                          IDbParameters parameters);
        
        int DataTableFillWithParams(DataTable dataTable, CommandType commandType, string sql,
                          IDbParameters parameters,
                          string tableName);	    

        int DataTableFillWithParams(DataTable dataTable, CommandType commandType, string sql,
                          IDbParameters parameters,
                          ITableMapping tableMapping);
	    
        int DataTableFillWithParams(DataTable dataTable, CommandType commandType, string sql, 
                          IDbParameters parameters,
                          ITableMapping tableMapping,
                          IDataAdapterSetter dataAdapterSetter);	 
	    
	    //TODO
	    /*
        int DataTableFillWithParams(DataTable dataTable, 
                          IDbDataAdapterCreator dataAdapterCreator);	 
	    
	    */
        #endregion
	    
        #region DataTable Update operations
        //TODO conflict options...
	    
        int DataTableUpdateWithCommandBuilder(DataTable dataTable,
            CommandType commandType,
            string selectSql,
            IDbParameters parameters,
            string tableName);
	    
        int DataTableUpdateWithCommandBuilder(DataTable dataTable,
            CommandType commandType,
            string selectSql,
            IDbParameters parameters,	                                          
            string tableName,
            IDataAdapterSetter dataAdapterSetter);

        int DataTableUpdateWithCommandBuilder(DataTable dataTable,
            CommandType commandType,
            string selectSql,
            IDbParameters parameters,                                              
            ITableMapping tableMapping,
            IDataAdapterSetter dataAdapterSetter);		       
	    
        int DataTableUpdate(DataTable dataTable, 
            string tableName,
            CommandType insertCommandtype, string insertSql, IDbParameters insertParameters,
            CommandType updateCommandtype, string updateSql, IDbParameters updateParameters,
            CommandType deleteCommandtype, string deleteSql, IDbParameters deleteParameters);                          
	    
        int DataTableUpdate(DataTable dataTable, 
            string tableName,
            CommandType insertCommandtype, string insertSql, IDbParameters insertParameters,
            CommandType updateCommandtype, string updateSql, IDbParameters updateParameters,
            CommandType deleteCommandtype, string deleteSql, IDbParameters deleteParameters,
            IDataAdapterSetter dataAdapterSetter); 
	    
        int DataTableUpdate(DataTable dataTable, 
            ITableMapping tableMapping,
            CommandType insertCommandtype, string insertSql, IDbParameters insertParameters,
            CommandType updateCommandtype, string updateSql, IDbParameters updateParameters,
            CommandType deleteCommandtype, string deleteSql, IDbParameters deleteParameters,
            IDataAdapterSetter dataAdapterSetter); 	    
    
        
	    
        #endregion	    	    
	    
        #region DataSet Create operations without parameters
	    
	    DataSet DataSetCreate(CommandType commandType, string sql);
	    
        DataSet DataSetCreate(CommandType commandType, string sql,
                              string[] tableNames);
	    
        DataSet DataSetCreate(CommandType commandType, string sql, 
                              ITableMappingCollection tableMapping);
	    
        DataSet DataSetCreate(CommandType commandType, string sql, 
                              ITableMappingCollection tableMapping,
                              IDataAdapterSetter setter);
	    
        DataSet DataSetCreate(CommandType commandType, string sql, 
                              ITableMappingCollection tableMapping,
                              IDataAdapterSetter setter,
                              IDataSetFillLifecycleProcessor fillLifecycleProcessor);	
        #endregion
	    
        #region DataSet Create operations with parameters
	    
        DataSet DataSetCreateWithParams(CommandType commandType, string sql,
                              IDbParameters parameters);
        
        DataSet DataSetCreateWithParams(CommandType commandType, string sql,
                              IDbParameters parameters,
                              string[] tableNames);	    

        DataSet DataSetCreateWithParams(CommandType commandType, string sql,
                              IDbParameters parameters,
                              ITableMappingCollection tableMapping);
	    
        DataSet DataSetCreateWithParams(CommandType commandType, string sql, 
                                IDbParameters parameters,
                                ITableMappingCollection tableMapping,
                                IDataAdapterSetter dataAdapterSetter);	 
	    
        DataSet DataSetCreateWithParams(CommandType commandType, string sql, 
                                IDbParameters parameters,
                                ITableMappingCollection tableMapping,
                                IDataAdapterSetter dataAdapterSetter,
                                IDataSetFillLifecycleProcessor fillLifecycleProcessor);	 
	    	    
        #endregion
	    
        #region DataSet Fill operations without parameters
	    
	    /// <summary>
	    /// Fill a  <see cref="DataSet"/> based on a select command that requires no parameters.
	    /// </summary>
	    /// <param name="dataSet">The <see cref="DataSet"/> to populate</param>
	    /// <param name="commandType">The type of command</param>
	    /// <param name="sql">SQL query to execute</param>
	    /// <returns>The number of rows successfully added to or refreshed in the  <see cref="DataSet"/></returns>
	    int DataSetFill(DataSet dataSet, CommandType commandType, string sql);
	    
	    /// <summary>
	    /// Fill a  <see cref="DataSet"/> based on a select command that requires no parameters
	    /// that returns one or more result sets that are added as 
	    /// <see cref="DataTable"/>s to the <see cref="DataSet"/>
	    /// </summary>
	    /// <param name="dataSet">The <see cref="DataSet"/> to populate</param>
	    /// <param name="commandType">The type of command</param>
	    /// <param name="sql">SQL query to execute</param>
	    /// <param name="tableNames">The mapping of table names for each <see cref="DataTable"/>
	    /// created</param>
	    /// <returns></returns>
	    int DataSetFill(DataSet dataSet, CommandType commandType, string sql,
	                    string[] tableNames);
	    
        int DataSetFill(DataSet dataSet, CommandType commandType, string sql, 
                        ITableMappingCollection tableMapping);
	    
	    int DataSetFill(DataSet dataSet, CommandType commandType, string sql, 
	                    ITableMappingCollection tableMapping,
	                    IDataAdapterSetter setter);
	    
        int DataSetFill(DataSet dataSet, CommandType commandType, string sql, 
                        ITableMappingCollection tableMapping,
                        IDataAdapterSetter setter,
                        IDataSetFillLifecycleProcessor fillLifecycleProcessor);	
	    
        #endregion
	    
	    #region DataSet Fill operations with parameters
        
	    int DataSetFillWithParameters(DataSet dataSet, CommandType commandType, string sql,
                        IDbParameters parameters);
        
	    int DataSetFillWithParameters(DataSet dataSet, CommandType commandType, string sql,
	                    IDbParameters parameters,
                        string[] tableNames);	    

        int DataSetFillWithParameters(DataSet dataSet, CommandType commandType, string sql,
                        IDbParameters parameters,
                        ITableMappingCollection tableMapping);
	    
        int DataSetFillWithParameters(DataSet dataSet, CommandType commandType, string sql, 
                        IDbParameters parameters,
                        ITableMappingCollection tableMapping,
                        IDataAdapterSetter dataAdapterSetter);	 
	    
        int DataSetFillWithParameters(DataSet dataSet, CommandType commandType, string sql, 
                        IDbParameters parameters,
                        ITableMappingCollection tableMapping,
                        IDataAdapterSetter dataAdapterSetter,
                        IDataSetFillLifecycleProcessor fillLifecycleProcessor);	 	       
	    
        #endregion	   
	    
        #region DataSet Update operations
	    //TODO expose RowUpdate event in a generic manner
	    //TODO any way to integrate Accept/RejectChanges based on tx outcome?

        int DataSetUpdateWithCommandBuilder(DataSet dataSet,
            CommandType commandType,
            string selectSql,
            IDbParameters selectParameters,
            string tableName);
	    
        int DataSetUpdateWithCommandBuilder(DataSet dataSet,
            CommandType commandType,
            string selectSql,
            IDbParameters selectParameters,	                                          
            string tableName,
            IDataAdapterSetter dataAdapterSetter);

        int DataSetUpdateWithCommandBuilder(DataSet dataSet,
            CommandType commandType,
            string selectSql,
            IDbParameters selectParameters,                                              
            ITableMappingCollection tableMapping,
            IDataAdapterSetter dataAdapterSetter);	
	     
	    
	    int DataSetUpdate(DataSet dataSet, 
	                      string tableName,
	                      IDbCommand insertCommand,
	                      IDbCommand updateCommand,
	                      IDbCommand deleteCommand);
	    
	    
        int DataSetUpdate(DataSet dataSet, 
            string tableName,
            CommandType insertCommandtype, string insertSql, IDbParameters insertParameters,
            CommandType updateCommandtype, string updateSql, IDbParameters updateParameters,
            CommandType deleteCommandtype, string deleteSql, IDbParameters deleteParameters);                          
	    
	    
        int DataSetUpdate(DataSet dataSet, 
                            string tableName,
                            IDbCommand insertCommand,
                            IDbCommand updateCommand,
                            IDbCommand deleteCommand,
                            IDataAdapterSetter dataAdapterSetter);

        int DataSetUpdate(DataSet dataSet, 
                          ITableMappingCollection tableMapping,
                          IDbCommand insertCommand,
                          IDbCommand updateCommand,
                          IDbCommand deleteCommand);	    
        
	    int DataSetUpdate(DataSet dataSet, 
                            ITableMappingCollection tableMapping,
                            IDbCommand insertCommand,
                            IDbCommand updateCommand,
                            IDbCommand deleteCommand,
	                        IDataAdapterSetter dataAdapterSetter);	
	    
        #endregion
	    
        #region Parameter Creation Helper Methods
	    
	    IDbParameters CreateDbParameters();
	    
	    /// <summary>
	    /// Note that output parameters are marked input/output after derivation....
	    /// (TODO - double check....)
	    /// </summary>
	    /// <param name="procedureName"></param>
	    /// <returns></returns>
	    IDataParameter[] DeriveParameters(string procedureName);
	    
	    IDataParameter[] DeriveParameters(string procedureName, bool includeReturnParameter);

        #endregion
	}


}
