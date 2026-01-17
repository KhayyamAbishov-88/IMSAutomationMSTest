using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using IMSAutomation.Entities;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace IMSAutomation.utilities
{
    internal class DatabaseHelper
    {
        public VehicleViewRow GetRandomVehicleFromView ( string connectionString )
        {
            var vehicle = new VehicleViewRow();

            using ( var conn = new SqlConnection( connectionString ) )
            {
                conn.Open();
                var query = @"
            SELECT TOP 1 
                vehicle_brand_name,
                vehicle_model_name,
                vehicle_sub,
                year_old
            FROM [Eagle].[Kasko].[AvailableVehiclesView]
            ORDER BY NEWID();";

                using ( var cmd = new SqlCommand( query, conn ) )
                using ( var reader = cmd.ExecuteReader() )
                {
                    if ( reader.Read() )
                    {
                        vehicle.BrandName = reader["vehicle_brand_name"].ToString();
                        vehicle.ModelName = reader["vehicle_model_name"].ToString();
                        vehicle.SubModel = reader["vehicle_sub"].ToString();
                        vehicle.YearOld = Convert.ToInt32( reader["year_old"] );
                    }
                }
            }

            return vehicle;
        }

        public async Task<(bool Enabled, TimeSpan? OtpSkipTime)> GetUserOtpPermissionAsync ( string username, string connectionString )
        {
            const string sql = @";WITH ParamValues AS (
        SELECT
            rc.role_composition_oid,
            uo.u_logon_name,
            ParamName = pp.name,
            ParamValue =
                ISNULL(
                    STUFF((
                        SELECT N', ' + pv1.value
                        FROM Security.ParameterValue AS pv1
                        WHERE pv1.parameter_oid = pp.parameter_oid
                          AND pv1.role_composition_oid = rc.role_composition_oid
                        ORDER BY pv1.[index]
                        FOR XML PATH(N''), TYPE
                    ).value('.', 'nvarchar(max)'), 1, 2, N''),
                    pp.default_value
                )
        FROM Security.Role AS r
        JOIN Security.RoleComposition AS rc ON r.role_oid = rc.role_oid
        JOIN Security.Permission AS p ON p.permission_oid = rc.permission_oid
        LEFT JOIN Security.PermissionParameter AS pp ON pp.permission_oid = p.permission_oid
        LEFT JOIN Classifiers.LOB AS lob ON lob.lob_oid = p.lob_oid
        LEFT JOIN Security.UserRole ur ON ur.role_oid = rc.role_oid
        LEFT JOIN UserObject uo ON uo.user_guid = ur.user_guid
        WHERE p.name = 'LoginWebIMS'
          AND pp.name IN ('OtpVerificationEnabled','OtpTrustWindowInHours')
          AND uo.u_logon_name = @UserId
    )
    SELECT
        OtpVerificationEnabled =
            MAX(CASE WHEN ParamName = 'OtpVerificationEnabled' THEN ParamValue END),
        OtpTrustWindowInHours =
            MAX(CASE WHEN ParamName = 'OtpTrustWindowInHours' THEN ParamValue END)
    FROM ParamValues;";

            await using var conn = new SqlConnection( connectionString );
            await using var cmd = new SqlCommand( sql, conn );

            cmd.Parameters.Add( "@UserId", SqlDbType.NVarChar, 256 ).Value = username;

            await conn.OpenAsync().ConfigureAwait( false );

            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait( false );

            if ( await reader.ReadAsync().ConfigureAwait( false ) )
            {
                // Enabled flag
                string enabledRaw = reader.IsDBNull( 0 ) ? "False" : reader.GetString( 0 );
                bool enabled = enabledRaw.Equals( "true", StringComparison.OrdinalIgnoreCase );

                // OtpTrustWindowInHours → TimeSpan
                TimeSpan? skip = null;
                if ( !reader.IsDBNull( 1 ) )
                {
                    string raw = reader.GetString( 1 );
                    if ( int.TryParse( raw, out int hours ) )
                        skip = TimeSpan.FromHours( hours );
                }

                return (enabled, skip);
            }

            return (false, null);
        }


        public async Task<(DateTime? LastLoginDate, string? DeviceId)> GetLastLoginDateAsync (
      string username,
      string connectionString )
        {
            await using var conn = new SqlConnection( connectionString );

            await using var cmd = new SqlCommand( @"
        SELECT TOP 1
            d.last_login_date,
            d.device_id
        FROM Eagle.dbo.UserObject o
        LEFT JOIN Eagle.dbo.UserTrustedDevice d
            ON o.user_guid = d.user_guid
        WHERE o.u_logon_name = @UserId
        ORDER BY d.last_login_date DESC
    ", conn );

            cmd.Parameters.Add( "@UserId", SqlDbType.NVarChar, 100 ).Value = username;

            await conn.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync( CommandBehavior.SingleRow );

            if ( await reader.ReadAsync() )
            {
                DateTime? lastLoginDate = reader.IsDBNull( 0 )
                    ? null
                    : reader.GetDateTime( 0 );

                string? deviceId = reader.IsDBNull( 1 )
                    ? null
                    : reader.GetString( 1 );

                return (lastLoginDate, deviceId);
            }

            return (null, null);
        }





        public (DateTime? OtpGenerateTime,string OtpCode, bool SmsSent ) GetLatestOtpCode ( string username, string connectionString )
        {
            const string sql = @"
;select  o.generated_time,o.otp_code, SmsSent =  CASE WHEN Q.created_timestamp IS NOT NULL THEN 1 ELSE 0 END  from dbo.UserObject u 
	left join  dbo.UserOTP o on u.user_guid=o.user_guid
	left join dbo.Queue q on o.user_guid=q.related_object_id where q.recipient=u.u_tel_number and u.u_logon_name= @UserId and u.d_last_login_date<q.created_timestamp
	and u.d_last_login_date<o.generated_time order by o.id desc;";

            using var conn = new SqlConnection( connectionString );
            using var cmd = new SqlCommand( sql, conn );
            cmd.Parameters.AddWithValue( "@UserId", username );

            conn.Open();

            using var reader = cmd.ExecuteReader( CommandBehavior.SingleRow );
            if ( !reader.Read() )
            {
                return (null,string.Empty, false ); // no OTP at all
            }

            DateTime? otpGeneratedTime = reader.IsDBNull( 0 ) ? ( DateTime? )null : reader.GetDateTime( 0 );

            string otp = reader.IsDBNull( 1) ? string.Empty : reader.GetString( 1);
            bool smsSent = !reader.IsDBNull( 2 ) && reader.GetInt32( 2 ) == 1;
           
            return (otpGeneratedTime,otp, smsSent );
        }

        public async Task<(DateTime? OtpGenerateTime, string OtpCode, bool SmsSent)> WaitForLatestOtpCode(string username, string connectionString, int timeoutSeconds = 10)
        {
            var endTime = DateTime.Now.AddSeconds(timeoutSeconds);
            while (DateTime.Now < endTime)
            {
                var result = GetLatestOtpCode(username, connectionString);
                if (result.SmsSent)
                {
                    return result;
                }
                await Task.Delay(500);
            }
            return GetLatestOtpCode(username, connectionString);
        }

       public async Task  RemoveLastOtpCode (string connectionString, string username  )
        {
            const string sql = @"
DECLARE @LastOtpId INT;
DECLARE @UserGuid UNIQUEIDENTIFIER;

SELECT TOP 1 
    @LastOtpId = o.id,
    @UserGuid = o.user_guid
FROM dbo.UserOTP o
JOIN dbo.UserObject uo ON o.user_guid = uo.user_guid
WHERE uo.u_logon_name = @UserName
ORDER BY o.generated_time DESC;

DELETE FROM dbo.Queue WHERE related_object_id = @UserGuid;
DELETE FROM dbo.UserOTP WHERE id = @LastOtpId;";

            using var conn = new SqlConnection( connectionString );
            await conn.OpenAsync();

            using var cmd = new SqlCommand( sql, conn );
            cmd.Parameters.AddWithValue( "@UserName", username );

            await cmd.ExecuteNonQueryAsync();
        }





        public async Task ClearTrustedDevices ( string connectionString, string username )
        {
            const string sql = @"
        DELETE FROM dbo.UserTrustedDevice 
        WHERE user_guid = (
            SELECT user_guid 
            FROM dbo.UserObject 
            WHERE u_logon_name = @UserName
        );";

            using var conn = new SqlConnection( connectionString );
            await conn.OpenAsync();

            using var cmd = new SqlCommand( sql, conn );
            cmd.Parameters.AddWithValue( "@UserName", username );

            await cmd.ExecuteNonQueryAsync();
        }






    }
}

