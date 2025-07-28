using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using IMSAutomation.Entities;

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
    }
}

