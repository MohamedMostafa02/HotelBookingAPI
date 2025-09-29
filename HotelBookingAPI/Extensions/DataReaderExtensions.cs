using Microsoft.Data.SqlClient;

namespace HotelBookingAPI.Extensions
{
    public static class DataReaderExtensions
    {
        public static T GetValueByColumn<T>(this SqlDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);

            if(!reader.IsDBNull(index))
            {
                return (T)reader.GetValue(index);
            }

            return default(T);
        }
    }
}
