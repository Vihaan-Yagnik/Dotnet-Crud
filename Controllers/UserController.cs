using Admin3.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using OfficeOpenXml;
using System.IO;
using System.Web;
using System.Data.Common;

namespace Admin3.Controllers
{
    public class UserController : Controller
    {



        private IConfiguration configuration;

        #region ConfigrationConnection
        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region UserTable
        public IActionResult UserTable()
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand Ucmd = conn.CreateCommand();
            Ucmd.CommandType = System.Data.CommandType.StoredProcedure;
            Ucmd.CommandText = "PR_User_SelectAll";
            SqlDataReader reader = Ucmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);

            return View(dt);
        }
        #endregion

        #region UserForm
        public IActionResult UserForm()
        {
            return View();
        }
        #endregion

        #region DeleteUser
        public IActionResult DelUser(int id)
        {
            try
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand Ucmd = conn.CreateCommand();
                Ucmd.CommandType = CommandType.StoredProcedure;
                Ucmd.CommandText = "PR_User_Delete";
                Ucmd.Parameters.AddWithValue("@UserID", id);
                Ucmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }
            return RedirectToAction("UserTable");
        }
        #endregion

        #region UserAddEditRender

        public IActionResult UserAddEdit(int UserID = 0)
        {
            if (UserID != 0)
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand Ucmd = conn.CreateCommand();
                Ucmd.CommandType = CommandType.StoredProcedure;
                Ucmd.CommandText = "PR_User_SelectByID";
                Ucmd.Parameters.AddWithValue("@UserID", UserID);
                SqlDataReader reader = Ucmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                conn.Close();

                UserModel umodel = new UserModel();

                foreach (DataRow dr in dt.Rows)
                {
                    umodel.UserID = Convert.ToInt32(dr["UserID"]);
                    umodel.UserName = dr["UserName"].ToString();
                    umodel.Email = dr["Email"].ToString();
                    umodel.Address = dr["Address"].ToString();
                    umodel.Password = dr["Password"].ToString();
                    umodel.MobileNo = dr["MobileNO"].ToString();
                    umodel.IsActive = Convert.ToBoolean(dr["IsActive"]);
                }
                return View("UserForm", umodel);
            }
            return View("UserForm");
        }

        #endregion

        #region UserSave
        public IActionResult UserSave(UserModel umodel)
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand Ucmd = conn.CreateCommand();
            Ucmd.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                if (umodel.UserID == 0 || umodel.UserID == null)
                {
                    Ucmd.CommandText = "PR_User_Insert";
                }
                else
                {
                    Ucmd.CommandText = "PR_User_Update";
                    Ucmd.Parameters.Add("@UserID", SqlDbType.Int).Value = umodel.UserID;
                }
                Ucmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = umodel.UserName;
                Ucmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = umodel.Email;
                Ucmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = umodel.Password;
                Ucmd.Parameters.Add("@MobileNO", SqlDbType.Decimal).Value = umodel.MobileNo;
                Ucmd.Parameters.Add("@IsActive", SqlDbType.VarChar).Value = umodel.IsActive;

                return RedirectToAction("UserTable");
            }
            else
            {
                return View("UserAddEdit", umodel);
            }
        }
        #endregion

        #region Static Crud
        // Static Crud

        //public IActionResult SaveUser(UserModel UM)
        //{
        //    if(UM.UserID == 0)
        //    {
        //        UM.UserID = UserModels.Max(u => u.UserID+1);
        //        UserModels.Add(UM);
        //    }
        //    else
        //    {
        //        UserModels[UM.UserID - 1].UserName = UM.UserName;
        //        UserModels[UM.UserID - 1].Email = UM.Email;
        //        UserModels[UM.UserID - 1].Password = UM.Password;
        //        UserModels[UM.UserID - 1].MobileNo = UM.MobileNo;
        //        UserModels[UM.UserID - 1].Address = UM.Address;
        //        UserModels[UM.UserID - 1].IsActive = UM.IsActive;
        //    }
        //    return RedirectToAction("UserTable");
        //}

        //public IActionResult AddEdit(int UserID = 0)
        //{
        //    UserModel u = new UserModel();
        //    if(UserID != 0)
        //    {
        //       UserModel obj = UserModels.Find(ui =>  ui.UserID == UserID);

        //        u.UserID = obj.UserID;
        //        u.UserName = obj.UserName;
        //        u.Email = obj.Email;
        //        u.Password = obj.Password;
        //        u.MobileNo = obj.MobileNo;
        //        u.Address = obj.Address;
        //        u.IsActive = obj.IsActive;
        //    }
        //    return View("UserForm", u);
        //}
        //public IActionResult DelUser(int UserID)
        //{ 
        //    int n = UserModels.FindIndex(ui =>  ui.UserID == UserID);
        //    UserModels.RemoveAt(n);
        //    return RedirectToAction("UserTable");
        //}
        #endregion

        #region ExcelImport

        [HttpPost]
        public IActionResult Import(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Save the uploaded file to a temporary location
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Load the data from the Excel file
                DataTable dataTable = LoadDataFromExcel(filePath);

                // Insert data into the Users table
                InsertDataIntoDatabase(dataTable);

                // Delete the temporary file
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("UserTable");
        }

        #endregion

        #region LoadExcel
        private DataTable LoadDataFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dt = new DataTable();

                // Assuming the first row contains column names
                for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
                {
                    dt.Columns.Add(worksheet.Cells[1, i].Text);
                }

                // Start loading data from the second row
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    DataRow dataRow = dt.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dt.Rows.Add(dataRow);
                }

                return dt;
            }
        }
        #endregion

        #region InsertInSql
        private void InsertDataIntoDatabase(DataTable dataTable)
        {
            using (SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection")))
            {
                conn.Open();
                foreach (DataRow row in dataTable.Rows)
                {
                    string query = "INSERT INTO Users (UserName, Email, Password, MobileNo, Address, IsActive) " +
                                   "VALUES (@UserName, @Email, @Password, @MobileNo, @Address, @IsActive)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", row["UserName"]);
                        cmd.Parameters.AddWithValue("@Email", row["Email"]);
                        cmd.Parameters.AddWithValue("@Password", row["Password"]);
                        cmd.Parameters.AddWithValue("@MobileNo", row["MobileNo"]);
                        cmd.Parameters.AddWithValue("@Address", row["Address"]);
                        cmd.Parameters.AddWithValue("@IsActive", row["IsActive"]);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion

        #region Export Data to Excel

        [HttpPost]
        public IActionResult Export()
        {
            DataTable dataTable = GetUsersData();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Load the DataTable into the sheet, starting from cell A1.
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, PrintHeaders: true);

                // Optionally, format the header row
                using (var range = worksheet.Cells["A1:F1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                package.Save();
            }

            stream.Position = 0;

            string excelName = $"Users-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        #endregion

        #region GetUserdata

        private DataTable GetUsersData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT UserName, Email, Password, MobileNo, Address, IsActive FROM Users", conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        #endregion

        public IActionResult Cancel()
        {
            return RedirectToAction("UserTable");
        }
    }
}
