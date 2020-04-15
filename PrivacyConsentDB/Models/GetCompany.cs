using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace PrivacyConsentDB.Models
{
    public class GetCompany
    {
        private SqlConnection con;
        private void connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["PUBLISH"].ToString();
            con = new SqlConnection(constring);
        }

        // ********** VIEW EMPLOYEE DETAILS ********************
        public List<Company> GetCompanyList()
        {
            List<Company> companiesList = new List<Company>();


            try
            {
                connection();


                SqlCommand cmd = new SqlCommand("SP_GetCompany", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@NTID";
                param.Value = "KODAVP";
                cmd.Parameters.Add(param);
                //cmd.ExecuteReader();

                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                con.Open();
                sd.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    companiesList.Add(
                        new Company
                        {
                            COMP_CODE = Convert.ToString(dr["COMP_CODE"]),
                            COMP_NAME = Convert.ToString(dr["COMP_NAME"])
                        });
                }
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return companiesList;
        }
    }
}