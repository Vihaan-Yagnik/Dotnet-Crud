using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;

namespace Admin3.Controllers
{
    public class OrderDetailController : Controller
    {
        
        private IConfiguration configuration;

        #region ConfigurationConstructor

        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region OrderDetailTable
        public IActionResult OrderDetailTable()
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand ODcmd = conn.CreateCommand();
            ODcmd.CommandType = System.Data.CommandType.StoredProcedure;
            ODcmd.CommandText = "PR_OrderDetail_SelectAll";
            SqlDataReader reader = ODcmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return View(dt);
        }
        #endregion

        #region OrderDetailForm
        public IActionResult OrderDetailForm()
        {
            return View();
        }
        #endregion

        #region DeleteOrderDetail
        public IActionResult DelOD(int id)
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand ODcmd = conn.CreateCommand();
            ODcmd.CommandType = CommandType.StoredProcedure;
            ODcmd.CommandText = "PR_OrderDetail_Delete";
            ODcmd.Parameters.AddWithValue("@OrderDetailID", id);
            ODcmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("OrderDetailTable");
        }
        #endregion

        #region OrderDetailAddEdit
        public IActionResult OrderDetailAddEdit(int OrderDetailID = 0)
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "PR_Orders_DropDown";
            SqlDataReader data = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(data);

            cmd.CommandText = "PR_Product_DropDown";
            SqlDataReader data1 = cmd.ExecuteReader();
            DataTable dt1 = new DataTable();
            dt1.Load(data1);

            cmd.CommandText = "PR_User_DropDown";
            SqlDataReader data2 = cmd.ExecuteReader();
            DataTable dt2 = new DataTable();
            dt2.Load(data2);

            List<OrderDropDownModel> oddm = new List<OrderDropDownModel>();

            foreach (DataRow dr in dt.Rows)
            {
                OrderDropDownModel omodel = new OrderDropDownModel();
                omodel.OrderID = Convert.ToInt32(dr["OrderID"]);
                omodel.OrderNO = dr["OrderNO"].ToString();
                oddm.Add(omodel);
            }

            List<ProductDropDownModel> pddm = new List<ProductDropDownModel>();

            foreach (DataRow dr in dt1.Rows)
            {
                ProductDropDownModel pmodel = new ProductDropDownModel();
                pmodel.ProductID = Convert.ToInt32(dr["ProductID"]);
                pmodel.ProductName = dr["ProductName"].ToString();
                pddm.Add(pmodel);
            }

            List<UserDropDownModel> uddm = new List<UserDropDownModel>();

            foreach (DataRow dr in dt2.Rows)
            {
                UserDropDownModel umodel = new UserDropDownModel();
                umodel.UserID = Convert.ToInt32(dr["UserID"]);
                umodel.UserName = dr["UserName"].ToString();
                uddm.Add(umodel);
            }

            ViewBag.OrderList = oddm;
            ViewBag.ProductList = pddm;
            ViewBag.UserList = uddm;
            conn.Close();

            if (OrderDetailID != 0 )
            {
                SqlConnection conn1 = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn1.Open();
                
                SqlCommand cmd1 = conn1.CreateCommand();
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.CommandText = "PR_SelectByPK_OrderDetail";
                cmd1.Parameters.AddWithValue("@OrderDetailID", OrderDetailID);
                cmd1.ExecuteNonQuery();
                SqlDataReader data3 = cmd1.ExecuteReader();
                DataTable dt3 = new DataTable();
                dt3.Load(data3);
                conn1.Close();

                OrderDetailModel odm = new OrderDetailModel();

                foreach (DataRow i in dt3.Rows)
                {
                    odm.OrderDetailID = Convert.ToInt32(i["OrderDetailID"]);
                    odm.OrderID = Convert.ToInt32(i["OrderID"]);
                    odm.ProductID = Convert.ToInt32(i["ProductID"]);
                    odm.Quantity = Convert.ToInt32(i["Quantity"]);
                    odm.Amount = Convert.ToDecimal(i["Amount"]);
                    odm.TotalAmount = Convert.ToDecimal(i["TotalAmount"]);
                    odm.UserID = Convert.ToInt32(i["UserID"]);
                }

                return View("OrderDetailForm", odm);
            }
            return View("OrderDetailForm");
        }
        #endregion

        #region OrderDetailSave
        public IActionResult OrderDetialSave(OrderDetailModel odmodel)
        {
            if(odmodel.OrderID <= 0 && odmodel.OrderID == null)
            {
                ModelState.AddModelError("OrderID", "Invalid OrderID");
            }

            if (odmodel.ProductID <= 0 && odmodel.ProductID == null)
            {
                ModelState.AddModelError("ProductID", "Invalid ProductID");
            }

            if (odmodel.UserID <= 0 && odmodel.UserID == null)
            {
                ModelState.AddModelError("UserID", "Invalid UserID");
            }

            if (ModelState.IsValid) 
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;

                if (odmodel.OrderDetailID == 0 || odmodel.OrderDetailID == null) 
                {
                    cmd.CommandText = "PR_OrderDetail_Insert";
                }
                else
                {
                    cmd.CommandText = "PR_OrderDetail_Update";
                    cmd.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = odmodel.OrderDetailID;
                }
                cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = odmodel.OrderID;
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = odmodel.ProductID;
                cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = odmodel.Quantity;
                cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = odmodel.Amount;
                cmd.Parameters.Add("@TotalAmount", SqlDbType.Int).Value = odmodel.TotalAmount;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = odmodel.UserID;
                cmd.ExecuteNonQuery();
                conn.Close();

                return RedirectToAction("OrderDetailTable");
            }
            else
            {
                return View("OrderDetailAddEdit", odmodel);
            }
        }
        #endregion

        #region StaticCrud

        // Static Crud

        //public IActionResult SaveOrderDetial(OrderDetailModel odModel)
        //{
        //    if(odModel.OrderDetailID == 0)
        //    {
        //        odModel.OrderDetailID = ODModels.Max(od => od.OrderDetailID + 1);
        //        ODModels.Add(odModel);
        //    }
        //    else
        //    {
        //        ODModels[odModel.OrderDetailID -1].OrderID = odModel.OrderDetailID;
        //        ODModels[odModel.OrderDetailID -1].ProductID = odModel.ProductID;
        //        ODModels[odModel.OrderDetailID - 1].Quantity = odModel.Quantity;
        //        ODModels[odModel.OrderDetailID - 1].Amount = odModel.Amount;
        //        ODModels[odModel.OrderDetailID - 1].TotalAmount = odModel.TotalAmount;
        //        ODModels[odModel.OrderDetailID - 1].UserID = odModel.UserID;
        //    }
        //    return RedirectToAction("OrderDetailTable");
        //}

        //public IActionResult AddEdit(int OrderDetailID = 0)
        //{
        //    OrderDetailModel odd = new OrderDetailModel();
        //    if(odd.OrderDetailID != 0)
        //    {
        //        OrderDetailModel od = ODModels.Find(o => o.OrderDetailID == OrderDetailID);
        //        Console.WriteLine(od);
        //        odd.OrderDetailID = od.OrderDetailID;
        //        odd.OrderID = od.OrderID;
        //        odd.ProductID = od.ProductID;
        //        odd.Quantity = od.Quantity;
        //        odd.Amount = od.Amount;
        //        odd.TotalAmount = od.TotalAmount;
        //        odd.UserID = od.UserID;
        //    }
        //    return View("OrderDetailForm", odd);
        //}
        //public IActionResult DelOD(int OrderDetailID) 
        //{
        //    int n = ODModels.FindIndex(ood => ood.OrderDetailID == OrderDetailID);
        //    ODModels.RemoveAt(n);
        //    return RedirectToAction("OrderDetailTable");
        //}
        #endregion

        #region Cancel
        public IActionResult Cancel()
        {
            return RedirectToAction("OrderDetailTable");
        }
        #endregion
    }
}
