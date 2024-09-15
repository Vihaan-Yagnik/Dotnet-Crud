using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Admin3.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration configuration;
        #region ConfigurationConnection
        public LoginController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion
        public IActionResult LoginForm()
        {
            return View();
        }
        public IActionResult UserLogin(UserLoginModel model)
        {
            SqlConnection Conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            Conn.Open();

            SqlCommand sqlCommand = Conn.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_User_Login";
            sqlCommand.Parameters.AddWithValue("@UserName", model.UserName);
            sqlCommand.Parameters.AddWithValue("@Password", model.Password);
            DataTable dt = new DataTable();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            dt.Load(reader);
            if(dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                    HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                    return Redirect("~/");
                }
            }

            return View("LoginForm");
        }
    }
}
