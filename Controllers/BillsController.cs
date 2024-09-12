using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;


namespace Admin3.Controllers
{

    public class BillsController : Controller
    {
        //public static List<BillsModel> BillModels = new List<BillsModel>()
        //{
        //    new BillsModel
        //    {
        //        BillID = 1,
        //        BillNumber = "BILL2023001",
        //        BillDate = DateTime.Now.AddDays(-10),
        //        OrderID = 1,
        //        TotalAmount = 250.99m,
        //        Discount = 20.00m,
        //        NetAmount = 230.99m,
        //        UserID = 1
        //    },
        //    new BillsModel
        //    {
        //        BillID = 2,
        //        BillNumber = "BILL2023002",
        //        BillDate = DateTime.Now.AddDays(-8),
        //        OrderID = 2,
        //        TotalAmount = 320.50m,
        //        Discount = 15.50m,
        //        NetAmount = 305.00m,
        //        UserID = 2
        //    },
        //    new BillsModel
        //    {
        //        BillID = 3,
        //        BillNumber = "BILL2023003",
        //        BillDate = DateTime.Now.AddDays(-6),
        //        OrderID = 3,
        //        TotalAmount = 150.00m,
        //        Discount = 0.00m,
        //        NetAmount = 150.00m,
        //        UserID = 3
        //    },
        //    new BillsModel
        //    {
        //        BillID = 4,
        //        BillNumber = "BILL2023004",
        //        BillDate = DateTime.Now.AddDays(-4),
        //        OrderID = 4,
        //        TotalAmount = 400.75m,
        //        Discount = 50.75m,
        //        NetAmount = 350.00m,
        //        UserID = 4
        //    },
        //    new BillsModel
        //    {
        //        BillID = 5,
        //        BillNumber = "BILL2023005",
        //        BillDate = DateTime.Now.AddDays(-2),
        //        OrderID = 5,
        //        TotalAmount = 275.40m,
        //        Discount = 10.00m,
        //        NetAmount = 265.40m,
        //        UserID = 5
        //    }
        //};
        private IConfiguration configuration;

        #region ConfigurationConstructor
        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region Billstable
        public IActionResult BillsTable()
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand billcmd = conn.CreateCommand();
            billcmd.CommandType = System.Data.CommandType.StoredProcedure;
            billcmd.CommandText = "PR_Bills_SelectAll";
            SqlDataReader reader = billcmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);

            return View(dt);
        }
        #endregion

        #region BillsForm
        public ActionResult BillsForm()
        {
            return View();
        }
        #endregion

        #region DeleteBills
        public IActionResult DelBill(int id) 
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand billcmd = conn.CreateCommand();
            billcmd.CommandType = CommandType.StoredProcedure;
            billcmd.CommandText = "PR_Bills_Delete";
            billcmd.Parameters.AddWithValue("@BillID", id);
            billcmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("BillsTable");
        }
        #endregion

        #region BillsAddEdit
        public IActionResult BillsAddEdit(int BillID = 0) 
        {
            SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
            conn.Open();

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "PR_Orders_DropDown";
            SqlDataReader data = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(data);

            List<OrderDropDownModel> oddm = new List<OrderDropDownModel>();

            foreach (DataRow dr in dt.Rows)
            {
                OrderDropDownModel omodel = new OrderDropDownModel();
                omodel.OrderID = Convert.ToInt32(dr["OrderID"]);
                omodel.OrderNO = dr["OrderNO"].ToString();
                oddm.Add(omodel);
            }

            ViewBag.OrderList = oddm;

            cmd.CommandText = "PR_User_DropDown";
            SqlDataReader data2 = cmd.ExecuteReader();
            DataTable dt2 = new DataTable();
            dt2.Load(data2);

            List<UserDropDownModel> uddm = new List<UserDropDownModel>();

            foreach (DataRow dr in dt2.Rows)
            {
                UserDropDownModel umodel = new UserDropDownModel();
                umodel.UserID = Convert.ToInt32(dr["UserID"]);
                umodel.UserName = dr["UserName"].ToString();
                uddm.Add(umodel);
            }

            ViewBag.UserList = uddm;
            conn.Close();

            if(BillID != 0 ) 
            {
                SqlConnection conn1 = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn1.Open();

                SqlCommand cmd1 = conn1.CreateCommand();
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.CommandText = "PR_Bills_SelectByID";
                cmd1.Parameters.AddWithValue("@BillID", BillID);
                SqlDataReader d = cmd1.ExecuteReader();
                DataTable dt3 = new DataTable();
                dt3.Load(d);
                conn1.Close();

                BillsModel bmodel = new BillsModel();

                foreach(DataRow dr in dt3.Rows)
                {
                    bmodel.BillID = Convert.ToInt32(dr["BillID"]);
                    bmodel.BillNumber = dr["BillNumber"].ToString() ;
                    bmodel.BillDate = Convert.ToDateTime(dr["BillDate"]);
                    bmodel.OrderID = Convert.ToInt32(dr["OrderID"]);
                    bmodel.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    bmodel.Discount = Convert.ToDecimal(dr["Discount"]);
                    bmodel.NetAmount = Convert.ToDecimal(dr["NetAmount"]);
                    bmodel.UserID = Convert.ToInt32(dr["UserID"]);
                }
                return View("BillsForm", bmodel);
            }
            return View("BillsForm");
        }
        #endregion

        #region BillsSave
        public IActionResult BillsSave(BillsModel bsmodel)
        {
            if (bsmodel.UserID <= 0 && bsmodel.UserID == null)
            {
                ModelState.AddModelError("UserID", "Invalid UserID");
            }

            if (bsmodel.OrderID <= 0 && bsmodel.OrderID == null)
            {
                ModelState.AddModelError("OrderID", "Invalid OrderID");
            }

            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(this.configuration.GetConnectionString("myConnection"));
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                
                if(bsmodel.BillID == 0 || bsmodel.BillID == null)
                {
                    cmd.CommandText = "PR_Bills_Insert";
                }
                else
                {
                    cmd.CommandText = "PR_Bills_Update";
                    cmd.Parameters.Add("@BillID",SqlDbType.Int).Value = bsmodel.BillID;
                }
                cmd.Parameters.Add("@BillNumber",SqlDbType.VarChar).Value = bsmodel.BillNumber;
                cmd.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = bsmodel.BillDate;
                cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = bsmodel.OrderID;
                cmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = bsmodel.TotalAmount;
                cmd.Parameters.Add("@Discount", SqlDbType.Decimal).Value = bsmodel.Discount;
                cmd.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = bsmodel.NetAmount;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = bsmodel.UserID;
                cmd.ExecuteNonQuery();
                return RedirectToAction("BillsTable");
            }
            else
            {
                return View("BillsAddEdit",bsmodel);
            }
        }
        #endregion

        #region StaticCrud
        // Static Crud

        //public ActionResult SaveBills(BillsModel bmodel)
        //{
        //    if (bmodel.BillID == 0) {
        //        bmodel.BillID = BillModels.Max(b => b.BillID + 1);
        //        BillModels.Add(bmodel);
        //    }
        //    else
        //    {
        //        BillModels[bmodel.BillID - 1].BillNumber = bmodel.BillNumber;
        //        BillModels[bmodel.BillID - 1].BillDate = bmodel.BillDate;
        //        BillModels[bmodel.BillID - 1].OrderID = bmodel.OrderID;
        //        BillModels[bmodel.BillID - 1].TotalAmount = bmodel.TotalAmount;
        //        BillModels[bmodel.BillID - 1].Discount = bmodel.Discount;
        //        BillModels[bmodel.BillID - 1].NetAmount = bmodel.NetAmount;
        //        BillModels[bmodel.BillID - 1].UserID = bmodel.UserID;
        //    }
        //    return RedirectToAction("BillsTable",bmodel);
        //}
        //public IActionResult AddEdit(int BillID = 0)
        //{
        //    BillsModel B = new BillsModel();
        //    if(B.BillID != 0)
        //    {
        //        var BB = BillModels.Find(b => b.BillID == BillID);
        //        B.BillID = BB.BillID;
        //        B.BillDate = BB.BillDate;
        //        B.OrderID = BB.OrderID;
        //        B.TotalAmount = BB.TotalAmount;
        //        B.Discount = BB.Discount;
        //        B.NetAmount = BB.NetAmount;
        //        B.BillNumber = BB.BillNumber;
        //        B.UserID = BB.UserID;
        //    }
        //    return View("BillsForm",B);
        //}

        //public IActionResult DelBill(int BillID = 0)
        //{ 
        //    int n = BillModels.FindIndex(B =>  B.BillID == BillID);
        //    BillModels.RemoveAt(n);
        //    return RedirectToAction("BillsTable");
        //}
        #endregion

        #region Cancel
        public IActionResult Cancel()
        {
            return RedirectToAction("BillsTable");
        }
        #endregion
    }
}
