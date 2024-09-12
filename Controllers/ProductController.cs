using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Security.Cryptography.X509Certificates;

namespace Admin3.Controllers
{
    public class ProductController : Controller
    {
       
        private IConfiguration configuration;
        
        #region ConfigurationConnection
        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region ProductTable
        public IActionResult ProductTable()
        {
            SqlConnection Conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            Conn.Open();

            SqlCommand sqlCommand = Conn.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Product_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            dt.Load(reader);

            return View(dt);
        }
        #endregion

        #region ProductForm
        public IActionResult ProductForm()
        {
            return View();
        }
        #endregion

        #region DeleteProduct
        public IActionResult DelProduct(int lol)
        {
            try
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand pdcmd = conn.CreateCommand();
                pdcmd.CommandType = CommandType.StoredProcedure;
                pdcmd.CommandText = "PR_Product_Delete";
                pdcmd.Parameters.AddWithValue("@ProductID", lol);
                pdcmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.Write(ex.ToString());
            }
            return RedirectToAction("ProductTable");

        }
        #endregion

        #region ProdcutAddEdit
        public IActionResult ProductAddEdit(int ProductID = 0)
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand pdcmd = conn.CreateCommand();
            pdcmd.CommandType = CommandType.StoredProcedure;
            pdcmd.CommandText = "PR_User_DropDown";
            SqlDataReader reader = pdcmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            conn.Close();

            List<UserDropDownModel> users = new List<UserDropDownModel>();

            foreach (DataRow row in dt.Rows) {
                UserDropDownModel u = new UserDropDownModel();
                u.UserID = Convert.ToInt32(row["UserID"]);
                u.UserName = row["UserName"].ToString();
                users.Add(u);
            }

            ViewBag.UserList = users;

            if(ProductID != 0)
            {
                SqlConnection conn1 = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn1.Open();

                SqlCommand pdcmd1 = conn1.CreateCommand();
                pdcmd1.CommandType = CommandType.StoredProcedure;
                pdcmd1.CommandText = "PR_Product_SelectByID";
                pdcmd1.Parameters.AddWithValue("@ProductID", ProductID);
                SqlDataReader reader1 = pdcmd1.ExecuteReader();
                DataTable dt1 = new DataTable();
                dt1.Load(reader1);
                conn1.Close();
                ProductModel prodmodel = new ProductModel();

                foreach (DataRow row in dt1.Rows) 
                {
                    prodmodel.ProductID = Convert.ToInt32(@row["ProductID"]);
                    prodmodel.ProductName = @row["ProductName"].ToString();
                    prodmodel.ProductCode = @row["ProductCode"].ToString();
                    prodmodel.ProductPrice = Convert.ToDouble(@row["ProductPrice"]);
                    prodmodel.Description = @row["Description"].ToString();
                    prodmodel.UserID = Convert.ToInt32(@row["UserID"]);
                }
                return View("ProductForm", prodmodel);
            }

            return View("ProductForm");
        }
        #endregion

        #region ProductSave
        public IActionResult ProductSave(ProductModel pdmodel )
        {
            if(pdmodel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "a valid user id is required.");
            }

            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand pdcmd = conn.CreateCommand();
                pdcmd.CommandType = CommandType.StoredProcedure;

                if (pdmodel.ProductID == 0 || pdmodel.ProductID == null)
                {
                    pdcmd.CommandText = "PR_Product_Insert";
                }
                else
                {
                    pdcmd.CommandText = "PR_Product_Update";
                    pdcmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = pdmodel.ProductID;
                }
                pdcmd.Parameters.Add("@ProductName",SqlDbType.VarChar).Value = pdmodel.ProductName;
                pdcmd.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = pdmodel.ProductPrice;
                pdcmd.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = pdmodel.ProductCode;
                pdcmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = pdmodel.Description;
                pdcmd.Parameters.Add("@UserID", SqlDbType.Int).Value = pdmodel.UserID;
                pdcmd.ExecuteNonQuery();
                return RedirectToAction("ProductTable");
            }
            else
            {
                return View("ProductAddEdit",pdmodel);
            }
        }
        #endregion

        #region StaticCrud

        // static CrudCode

        //public IActionResult SaveProduct(ProductModel Pd)
        //{
        //    if (Pd.ProductID == 0)
        //    {
        //        Pd.ProductID = productModels.Max(x => x.ProductID + 1);
        //        productModels.Add(Pd);
        //    }
        //    else
        //    {
        //        int n = productModels.FindIndex(p => p.ProductID == Pd.ProductID);
        //        productModels[n].ProductName = Pd.ProductName;
        //        productModels[n].ProductCode = Pd.ProductCode;
        //        productModels[n].ProductPrice = Pd.ProductPrice;
        //        productModels[n].Description = Pd.Description;
        //        productModels[n].UserId = Pd.UserId;
        //    }
        //    return RedirectToAction("ProductTable");
        //}

        //public IActionResult AddEdit(int ProductId = 0)
        //{
        //    ProductModel pd = new ProductModel();
        //    if (ProductId != 0)
        //    {
        //        ProductModel selectProduct = productModels.Find(p => p.ProductID == ProductId);
        //        pd.ProductID = selectProduct.ProductID;
        //        pd.ProductName = selectProduct.ProductName;
        //        pd.ProductCode = selectProduct.ProductCode;
        //        pd.ProductPrice = selectProduct.ProductPrice;
        //        pd.Description = selectProduct.Description;
        //        pd.UserId = selectProduct.UserId;
        //    }
        //    return View("ProductForm", pd);
        //}
        //public IActionResult DelProduct(int ProductID)
        //{
        //    int n = productModels.FindIndex(p => p.ProductID == ProductID);
        //    productModels.RemoveAt(n);
        //    return RedirectToAction("ProductTable");
        //}
        #endregion

        #region Cancel
        public IActionResult Cancel()
        {
            return RedirectToAction("ProductTable");
        }
        #endregion
    }
}


