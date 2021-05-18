using Dapper.Oracle;
using System;
using System.Collections.Generic;

namespace Dapper.OData.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Extensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Returns a list of output parameters with their values after successfull execution of the oracle procedure
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetOutParamsDictionary(this OracleDynamicParameters parameters)
        {
            Dictionary<string, object> keyValuePairs = new();
            foreach (var item in parameters.ParameterNames)
            {
                var parameterInfo = parameters.GetParameter(item);
                if ((parameterInfo.ParameterDirection == System.Data.ParameterDirection.Output || parameterInfo.ParameterDirection == System.Data.ParameterDirection.InputOutput) && parameterInfo.DbType != OracleMappingType.RefCursor)
                {
                    object value = null;
                    switch (parameterInfo.DbType)
                    {
                        case OracleMappingType.BFile:
                        case OracleMappingType.BinaryDouble:
                        case OracleMappingType.BinaryFloat:
                        case OracleMappingType.Blob:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = Array.Empty<byte>();
                            }
                            catch (Exception)
                            {
                                Convert.FromBase64String(parameters.Get<string>(item));
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? Convert.FromBase64String(parameters.Get<string>(item)) : Array.Empty<byte>();
                            break;
                        case OracleMappingType.Byte:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                parameters.Get<byte?>(item);
                            }
                            catch (Exception)
                            {
                                parameters.Get<byte>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<byte>(item) : parameters.Get<byte?>(item);
                            break;
                        case OracleMappingType.Char:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                parameters.Get<char?>(item);
                            }
                            catch (Exception)
                            {
                                parameters.Get<char>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<char>(item) : parameters.Get<char?>(item);
                            break;
                        case OracleMappingType.Clob:
                        case OracleMappingType.NVarchar2:
                        case OracleMappingType.Varchar2:
                        case OracleMappingType.XmlType:
                            value = parameters.Get<string>(item);
                            break;
                        case OracleMappingType.Date:
                        case OracleMappingType.TimeStamp:
                        case OracleMappingType.TimeStampLTZ:
                        case OracleMappingType.TimeStampTZ:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<DateTime?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<DateTime>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<DateTime>(item) : parameters.Get<DateTime?>(item);
                            break;
                        case OracleMappingType.Decimal:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<decimal?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<decimal>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<decimal>(item) : parameters.Get<decimal?>(item);
                            break;
                        case OracleMappingType.Double:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<double?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<double>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<double>(item) : parameters.Get<double?>(item);
                            break;
                        case OracleMappingType.Int16:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<short?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<short>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<short>(item) : parameters.Get<short?>(item);
                            break;
                        case OracleMappingType.Int32:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<int?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<int>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<int>(item) : parameters.Get<int?>(item);
                            break;
                        case OracleMappingType.Int64:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<long?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<long>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<long>(item) : parameters.Get<long?>(item);
                            break;
                        case OracleMappingType.IntervalDS:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<TimeSpan?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<TimeSpan>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<TimeSpan>(item) : parameters.Get<TimeSpan?>(item);
                            break;
                        case OracleMappingType.IntervalYM:
                            value = parameters.Get<string>(item);
                            break;
                        case OracleMappingType.Long:
                            try
                            {
                                var dBNull = parameters.Get<DBNull>(item);
                                value = parameters.Get<long?>(item);
                            }
                            catch (Exception)
                            {
                                value = parameters.Get<long>(item);
                            }
                            //value = parameters.Get<object>(item) != DBNull.Value ? parameters.Get<long>(item) : parameters.Get<long?>(item);
                            break;
                        case OracleMappingType.LongRaw:
                            value = Convert.FromBase64String(parameters.Get<string>(item));
                            break;
                        case OracleMappingType.NChar:
                        case OracleMappingType.NClob:
                            value = parameters.Get<string>(item).ToCharArray();
                            break;
                        case OracleMappingType.Raw:
                            value = Convert.FromBase64String(parameters.Get<string>(item));
                            break;

                    }
                    keyValuePairs.Add(item, value);
                }
            }
            return keyValuePairs;
        }
    }
}
