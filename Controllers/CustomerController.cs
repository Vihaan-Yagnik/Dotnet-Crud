using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Data.SqlClient;

namespace Admin3.Controllers
{
    public class CustomerController : Controller
    {
        //public static List<CustomerModel> CustModels = new List<CustomerModel>()
        //{
        //    new CustomerModel
        //    {
        //        CustomerID = 1,
        //        CustomerName = "John Doe",
        //        HomeAddress = "123 Elm Street, Springfield, IL",
        //        Email = "john.doe@example.com",
        //        MobileNo = "123-456-7890",
        //        GSTNO = "GSTIN1234567890",
        //        CityName = "Springfield",
        //        PinCode = "62704",
        //        NetAmount = 500.00m,
        //        UserID = 1
        //    },
        //    new CustomerModel
        //    {
        //        CustomerID = 2,
        //        CustomerName = "Jane Smith",
        //        HomeAddress = "456 Oak Avenue, Chicago, IL",
        //        Email = "jane.smith@example.com",
        //        MobileNo = "098-765-4321",
        //        GSTNO = "GSTIN0987654321",
        //        CityName = "Chicago",
        //        PinCode = "60616",
        //        NetAmount = 750.25m,
        //        UserID = 2
        //    },
        //    new CustomerModel
        //    {
        //        CustomerID = 3,
        //        CustomerName = "Robert Brown",
        //        HomeAddress = "789 Pine Road, Evanston, IL",
        //        Email = "robert.brown@example.com",
        //        MobileNo = "555-123-4567",
        //        GSTNO = "GSTIN5551234567",
        //        CityName = "Evanston",
        //        PinCode = "60201",
        //        NetAmount = 320.75m,
        //        UserID = 3
        //    },
        //    new CustomerModel
        //    {
        //        CustomerID = 4,
        //        CustomerName = "Emily Davis",
        //        HomeAddress = "101 Maple Street, Naperville, IL",
        //        Email = "emily.davis@example.com",
        //        MobileNo = "444-321-6547",
        //        GSTNO = "GSTIN4443216547",
        //        CityName = "Naperville",
        //        PinCode = "60540",
        //        NetAmount = 420.00m,
        //        UserID = 4
        //    },
        //    new CustomerModel
        //    {
        //        CustomerID = 5,
        //        CustomerName = "Michael Johnson",
        //        HomeAddress = "202 Birch Lane, Aurora, IL",
        //        Email = "michael.johnson@example.com",
        //        MobileNo = "333-456-7890",
        //        GSTNO = "GSTIN3334567890",
        //        CityName = "Aurora",
        //        PinCode = "60506",
        //        NetAmount = 275.40m,
        //        UserID = 5
        //    }
        //};
        private IConfiguration configuration;

        #region ConnectionConstructor
        public CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region CustomerTable
        public IActionResult CustomerTable()
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand Ccmd = conn.CreateCommand();
            Ccmd.CommandType = System.Data.CommandType.StoredProcedure;
            Ccmd.CommandText = "PR_Customers_SelectAll";
            SqlDataReader reader = Ccmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return View(dt);
        }
        #endregion

        #region CustomerForm
        public IActionResult CustomerForm()
        {
            return View();
        }
        #endregion

        #region DeleteCustomer
        public IActionResult DelCust(int id)
        {
            try
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand Ccmd = conn.CreateCommand();
                Ccmd.CommandType = CommandType.StoredProcedure;
                Ccmd.CommandText = "PR_Customers_Delete";
                Ccmd.Parameters.AddWithValue("@CustomerID", id);
                Ccmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("CustomerTable");

        }
        #endregion

        #region CustomerAddEditPageRender
        public IActionResult CustomerAddEdit(int CustomerID = 0)
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand ccmd = conn.CreateCommand();
            ccmd.CommandType = CommandType.StoredProcedure;
            ccmd.CommandText = "PR_User_DropDown";
            SqlDataReader reader = ccmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            conn.Close();

            List<UserDropDownModel> ulist = new List<UserDropDownModel>();

            foreach (DataRow row in dt.Rows)
            {
                UserDropDownModel ud = new UserDropDownModel();
                ud.UserID = Convert.ToInt32(row["UserID"]);
                ud.UserName = row["UserName"].ToString();
                ulist.Add(ud);
            }

            ViewBag.userlist = ulist;

            if (CustomerID != 0 || CustomerID != null)
            {
                SqlConnection conn1 = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn1.Open();

                SqlCommand ccmd1 = conn1.CreateCommand();
                ccmd1.CommandType = CommandType.StoredProcedure;
                ccmd1.CommandText = "PR_Customers_SelectByID";
                ccmd1.Parameters.AddWithValue("@CustomerID", CustomerID);
                ccmd1.ExecuteNonQuery();
                SqlDataReader reader1 = ccmd1.ExecuteReader();
                DataTable dt1 = new DataTable();
                dt1.Load(reader1);
                conn1.Close();

                CustomerModel c = new CustomerModel();

                foreach (DataRow row1 in dt1.Rows)
                {
                    c.CustomerID = Convert.ToInt32(row1["CustomerID"]);
                    c.CustomerName = row1["CustomerName"].ToString();
                    c.HomeAddress = row1["HomeAddress"].ToString();
                    c.Email = row1["Email"].ToString();
                    c.MobileNO = row1["MobileNo"].ToString();
                    c.GST_NO = row1["GST_NO"].ToString();
                    c.CityName = row1["CityName"].ToString();
                    c.PinCode = row1["PinCode"].ToString();
                    c.NetAmount = Convert.ToDecimal(row1["NetAmount"]);
                    c.UserID = Convert.ToInt32(row1["UserID"]);
                }
                return View("CustomerForm", c);
            }
            return View("CustomerForm");
        }
        #endregion

        #region CustomerSave
        public IActionResult CustomerSave(CustomerModel Cmodel)
        {
            if (Cmodel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "a valid userid is required.");
            }

            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand ccmd = conn.CreateCommand();
            ccmd.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                if (Cmodel.CustomerID == 0 || Cmodel.CustomerID == null)
                {
                    ccmd.CommandText = "PR_Customers_Insert";
                }
                else
                {
                    ccmd.CommandText = "PR_Customers_Update";
                    ccmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = Cmodel.CustomerID;
                }
                ccmd.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = Cmodel.CustomerName;
                ccmd.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = Cmodel.HomeAddress;
                ccmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = Cmodel.Email;
                ccmd.Parameters.Add("@MobileNO", SqlDbType.VarChar).Value = Cmodel.MobileNO;
                ccmd.Parameters.Add("@GST_NO", SqlDbType.VarChar).Value = Cmodel.GST_NO;
                ccmd.Parameters.Add("@CityName", SqlDbType.VarChar).Value = Cmodel.CityName;
                ccmd.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = Cmodel.PinCode;
                ccmd.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = Cmodel.NetAmount;
                ccmd.Parameters.Add("@UserID", SqlDbType.Int).Value = Cmodel.UserID;
                ccmd.ExecuteNonQuery();
                conn.Close();
                return RedirectToAction("CustomerTable");
            }
            else
            {
                return View("CustomerAddEdit",Cmodel);
            }
        }
        #endregion

        // Static Crud

        //public IActionResult SaveCustomer(CustomerModel custmodel)
        //{
        //    if (custmodel.CustomerID == 0) { 
        //        custmodel.CustomerID = CustModels.Max(c => c.CustomerID + 1);
        //        CustModels.Add(custmodel);
        //    }
        //    else
        //    {
        //        CustModels[custmodel.CustomerID - 1].CustomerName = custmodel.CustomerName ;
        //        CustModels[custmodel.CustomerID - 1].HomeAddress = custmodel.HomeAddress;
        //        CustModels[custmodel.CustomerID - 1].Email = custmodel.Email;
        //        CustModels[custmodel.CustomerID - 1].MobileNo = custmodel.MobileNo;
        //        CustModels[custmodel.CustomerID - 1].GSTNO = custmodel.GSTNO;
        //        CustModels[custmodel.CustomerID - 1].CityName = custmodel.CityName;
        //        CustModels[custmodel.CustomerID - 1].PinCode = custmodel.PinCode;
        //        CustModels[custmodel.CustomerID - 1].NetAmount = custmodel.NetAmount;
        //        CustModels[custmodel.CustomerID - 1].UserID = custmodel.UserID;
        //    }
        //    return RedirectToAction("CustomerTable");
        //}

        //public IActionResult AddEdit(int CustomerID = 0)
        //{
        //    CustomerModel c = new CustomerModel();
        //    if(CustomerID != 0)
        //    {
        //        var cc = CustModels.Find(C =>  C.CustomerID == CustomerID);
        //        c.CustomerID = cc.CustomerID;
        //        c.CustomerName = cc.CustomerName;
        //        c.HomeAddress = cc.HomeAddress;
        //        c.Email = cc.Email;
        //        c.MobileNo = cc.MobileNo;
        //        c.GSTNO = cc.GSTNO;
        //        c.CityName = cc.CityName;
        //        c.PinCode = cc.PinCode;
        //        c.NetAmount = cc.NetAmount;
        //        c.UserID = cc.UserID;
        //    }
        //    return View("CustomerForm", c);
        //}
        //public IActionResult DelCust(int CustomerID)
        //{
        //    int n = CustModels.FindIndex(c => c.CustomerID == CustomerID);
        //    CustModels.RemoveAt(n);
        //    return RedirectToAction("CustomerTable");
        //}
    }
}
