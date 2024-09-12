using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data;
using System.Data.SqlClient;

namespace Admin3.Controllers
{
    public class OrderController : Controller
    {
        private IConfiguration configuration;

        #region ConfigurationConstructor
        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region OrderTable
        public IActionResult OrderTable()
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand ocmd = conn.CreateCommand();
            ocmd.CommandType = System.Data.CommandType.StoredProcedure;
            ocmd.CommandText = "PR_Orders_SelectAll";
            SqlDataReader reader = ocmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return View(dt);
        }
        #endregion

        #region OrderForm
        public IActionResult OrderForm()
        {
            return View();
        }
        #endregion

        #region DeleteOrder
        public IActionResult DelOrder(int id)
        {
            try
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand ocmd = conn.CreateCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.CommandText = "PR_Order_Delete";
                ocmd.Parameters.AddWithValue("@OrderId", id);
                ocmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            return RedirectToAction("OrderTable");
        }
        #endregion

        #region OrderAddEdit
        public IActionResult OrderAddEdit(int OrderID = 0)
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand ocmd = conn.CreateCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PR_Customer_DropDown";
            SqlDataReader data1 = ocmd.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(data1);
            ocmd.CommandText = "PR_User_DropDown";
            SqlDataReader data2 = ocmd.ExecuteReader();
            DataTable dt2 = new DataTable();
            dt2.Load(data2);
            conn.Close();

            List<CustomerDropDownModel> custlist = new List<CustomerDropDownModel>();

            foreach (DataRow dr in dt1.Rows)
            {
                CustomerDropDownModel cmodel = new CustomerDropDownModel();
                cmodel.CustomerID = Convert.ToInt32(dr["CustomerID"]);
                cmodel.CustomerName = dr["CustomerName"].ToString();
                custlist.Add(cmodel);
            }

            ViewBag.CList = custlist;

            List<UserDropDownModel> userlist = new List<UserDropDownModel>();

            foreach (DataRow dr in dt2.Rows)
            {
                UserDropDownModel umodel = new UserDropDownModel();
                umodel.UserID = Convert.ToInt32(dr["UserID"]);
                umodel.UserName = dr["UserName"].ToString();
                userlist.Add(umodel);
            }

            ViewBag.UList = userlist;

            if(OrderID != 0)
            {
                SqlConnection conn1 = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn1.Open();

                SqlCommand ocmd1 = conn1.CreateCommand();
                ocmd1.CommandType = CommandType.StoredProcedure;
                ocmd1.CommandText = "PR_Orders_SelectByID";
                ocmd1.Parameters.AddWithValue("@OrderID", OrderID);
                ocmd1.ExecuteNonQuery();
                SqlDataReader data3 = ocmd1.ExecuteReader();
                DataTable dt3 = new DataTable();
                dt3.Load(data3);
                conn1.Close();

                OrderModel O = new OrderModel();

                foreach(DataRow dr in dt3.Rows)
                {
                    O.OrderID = Convert.ToInt32(dr["OrderID"]);
                    O.OrderNO = dr["OrderNO"].ToString();
                    O.OrderDate = Convert.ToDateTime(dr["OrderDate"]);
                    O.CustomerID = Convert.ToInt32(dr["CustomerID"]);
                    O.PaymentMode = dr["PaymentMode"].ToString();
                    O.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    O.ShippingAddress = dr["ShippingAddress"].ToString();
                    O.UserID = Convert.ToInt32(dr["UserID"]);
                }
                return View("OrderForm", O);
            }
            return View("OrderForm");
        }
        #endregion

        #region OrderSave
        public IActionResult OrderSave(OrderModel omodel)
        {
            if(omodel.UserID <= 0 && omodel.CustomerID <= 0)
            {
                ModelState.AddModelError("UserID", "UserID is not valid.");
                ModelState.AddModelError("CustomerID", "CustomerID is not valid.");
            }

            if (ModelState.IsValid) 
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                
                if(omodel.OrderID == 0 || omodel.OrderID == null)
                {
                    cmd.CommandText = "PR_Orders_Insert";
                }
                else
                {
                    cmd.CommandText = "PR_Update_Orders";
                    cmd.Parameters.Add("@OrderID",SqlDbType.Int).Value = omodel.UserID;
                }
                cmd.Parameters.Add("@OrderNO",SqlDbType.VarChar).Value = omodel.OrderNO;
                cmd.Parameters.Add("@OrderDate",SqlDbType.DateTime).Value = omodel.OrderDate;
                cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = omodel.CustomerID ;
                cmd.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = omodel.PaymentMode;
                cmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = omodel.TotalAmount;
                cmd.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = omodel.ShippingAddress;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = omodel.UserID;
                cmd.ExecuteNonQuery();

                return RedirectToAction("OrderTable");
            }
            else
            {
                return View("OrderAddEdit", omodel);
            }
        }
        #endregion

        #region StaticCrud

        // Static Crud Operation

        //public IActionResult SaveOrder(OrderModel OM)
        //{
        //    if(OM.OrderId == 0)
        //    {
        //       OM.OrderId = OrderModelList.Max(o => o.OrderId+1);
        //       OrderModelList.Add(OM);
        //    }
        //    else
        //    {
        //        OrderModelList[OM.OrderId - 1].OrderDate = OM.OrderDate;
        //        OrderModelList[OM.OrderId - 1].CustomerID = OM.CustomerID;
        //        OrderModelList[OM.OrderId - 1].PaymentMode = OM.PaymentMode;
        //        OrderModelList[OM.OrderId - 1].TotalAmount = OM.TotalAmount;
        //        OrderModelList[OM.OrderId - 1].ShippingAddress = OM.ShippingAddress;
        //        OrderModelList[OM.OrderId - 1].UserID = OM.UserID;
        //    }
        //    return RedirectToAction("OrderTable");
        //}

        //public IActionResult AddEdit(int OrderId = 0)
        //{
        //    OrderModel o = new OrderModel();
        //    if (OrderId != 0)
        //    {
        //        OrderModel oo = OrderModelList.Find(oi => oi.OrderId == OrderId);
        //        o.OrderId = oo.OrderId;
        //        o.OrderDate = oo.OrderDate;
        //        o.CustomerID = oo.CustomerID;
        //        o.PaymentMode = oo.PaymentMode;
        //        o.ShippingAddress = oo.ShippingAddress;
        //        o.TotalAmount = oo.TotalAmount;
        //        o.UserID = oo.UserID;
        //    }
        //    return View("OrderForm", o);
        //}

        //public IActionResult DelOrder(int OrderId) 
        //{ 
        //    int n = OrderModelList.FindIndex(o => o.OrderId == OrderId);
        //    OrderModelList.RemoveAt(n);
        //    return RedirectToAction("OrderTable");
        //}
        #endregion

        #region Cancel
        public IActionResult Cancel()
        {
            return RedirectToAction("OrderTable");
        }
        #endregion
    }
}
