using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ConvenienceStore.Controllers
{
    public class HomeController : Controller
    {
        string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\riche\source\repos\ConvenienceStore\ConvenienceStore\App_Data\Inventory.mdf;Integrated Security=True";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Management()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Management(FormCollection collection, HttpPostedFileBase product_image)
        {
            var product_name = collection["product_name"];
            var sku = collection["sku"];
            var category_id = collection["category_id"];
            var quantity = collection["quantity"];
            var unit_of_measure = collection["unit_of_measure"];
            var description = collection["description"];
            var supplier_id = collection["supplier_id"];
            var expiration_date = collection["expiration_date"];
            var low_stock_threshold = collection["low_stock_threshold"];

            // Validate product image
            if (product_image != null && product_image.ContentLength > 0)
            {
                string imag = Path.GetFileName(product_image.FileName);
                string logpath = "c:\\Upload";
                if (!Directory.Exists(logpath))
                {
                    Directory.CreateDirectory(logpath);
                }
                string filepath = Path.Combine(logpath, imag);
                product_image.SaveAs(filepath);

                using (var db = new SqlConnection(connStr))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO INVENTORY (PRODUCT_NAME, SKU, CATEGORY_ID, QUANTITY, UNIT_OF_MEASURE, DESCRIPTION, EXPIRATION_DATE, LOW_STOCK_THRESHOLD, SUPPLIER_ID, PRODUCT_IMAGE)" +
                                          "VALUES (@product_name, @sku, @category_id, @quantity, @unit_of_measure, @description, @expiration_date, @low_stock_threshold, @supplier_id, @file)";
                        cmd.Parameters.AddWithValue("@product_name", product_name);
                        cmd.Parameters.AddWithValue("@sku", sku);
                        cmd.Parameters.AddWithValue("@category_id", category_id);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@unit_of_measure", unit_of_measure);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@expiration_date", expiration_date);
                        cmd.Parameters.AddWithValue("@low_stock_threshold", low_stock_threshold);
                        cmd.Parameters.AddWithValue("@supplier_id", supplier_id);
                        cmd.Parameters.AddWithValue("@file", imag);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Clear form values by redirecting to the same view
            return RedirectToAction("Management");
        }


        [HttpGet]
        public FileResult Image(string filename)
        {
            var folder = "c:\\Upload";
            var filepath = Path.Combine(folder, filename);
            if (!System.IO.File.Exists(filepath))
            {
                // Return a default image or handle the error
            }
            var mime = System.Web.MimeMapping.GetMimeMapping(Path.GetFileName(filepath));
            return new FilePathResult(filepath, mime);
        }

        public ActionResult CreateProg()
        {
            return View();
        }

        public ActionResult ProdUpdate()
        {
            var data = new List<object>();
            var searchSku = Request["searchSku"];
            var sku = Request["sku"];
            var name = Request["name"];
            var category = Request["category"];
            var quantity = Request["quantity"];
            var description = Request["description"];
            var unit = Request["unit"];
            var low = Request["low"];
            var expiration = Request["expiration"];
            var supplier = Request["supplier"];

            if (supplier == null)
            {
                data.Add(new { mess = "Supplier is null" });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE INVENTORY SET " +
                                      "PRODUCT_NAME = @name, " +
                                      "QUANTITY = @quantity, " +
                                      "CATEGORY_ID = @category, " +
                                      "DESCRIPTION = @description," +
                                      "SKU = @sku," +
                                      "UNIT_OF_MEASURE = @unit," +
                                      "LOW_STOCK_THRESHOLD = @low," +
                                      "SUPPLIER_ID = @supplier," +
                                      "EXPIRATION_DATE = @expiration " +
                                      "WHERE SKU ='" + searchSku + "'";
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@sku", sku);
                    cmd.Parameters.AddWithValue("@unit", unit);
                    cmd.Parameters.AddWithValue("@low", low);
                    cmd.Parameters.AddWithValue("@supplier", supplier);
                    cmd.Parameters.AddWithValue("@expiration", expiration);
                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr > 0)
                    {
                        data.Add(new
                        {
                            mess = 0

                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProdSearch()
        {
            var data = new List<object>();
            var sku = Request["sku"];

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT PRODUCT_NAME, QUANTITY, CATEGORY_ID, DESCRIPTION, SKU, UNIT_OF_MEASURE, LOW_STOCK_THRESHOLD, EXPIRATION_DATE, SUPPLIER_ID  FROM INVENTORY WHERE SKU = @sku";
                    cmd.Parameters.AddWithValue("@sku", sku); // Prevent SQL Injection

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.Add(new
                            {
                                mess = 0,
                                product_name = reader["PRODUCT_NAME"].ToString(),
                                quantity = reader["QUANTITY"].ToString(),
                                category_id = reader["CATEGORY_ID"].ToString(),
                                description = reader["DESCRIPTION"].ToString(),
                                sku = reader["SKU"].ToString(),
                                unit_of_measure = reader["UNIT_OF_MEASURE"].ToString(),
                                low_stock_threshold = reader["LOW_STOCK_THRESHOLD"].ToString(),
                                expiration_date = reader["EXPIRATION_DATE"].ToString(),
                                supplier_id = reader["SUPPLIER_ID"].ToString()
                            });
                        }
                        else
                        {
                            data.Add(new { mess = 1 }); // Product not found
                        }
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult DeleteItem()
        {
            var data = new List<object>();
            var sku = Request["sku"];

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM INVENTORY WHERE SKU = @sku";
                    cmd.Parameters.AddWithValue("@sku", sku);

                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr > 0)
                    {
                        data.Add(new
                        {
                            mess = 0
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PurchaseOrder()
        {
            return View();
        }


        [HttpPost]
        public ActionResult PurchaseLowStock()
        {
            var data = new List<object>();
            try
            {
                // Extract data from POST request
                var quantityOrdered = Request["quantity_ordered"];
                var lowStockId = Request["low_stock_id"];
                var supplierId = Request["supplier_id"];
                var productId = Request["product_id"];

                // Validate inputs
                if (string.IsNullOrEmpty(quantityOrdered) || string.IsNullOrEmpty(lowStockId) ||
                    string.IsNullOrEmpty(supplierId) || string.IsNullOrEmpty(productId))
                {
                    throw new ArgumentException("Invalid input parameters.");
                }

                var orderDate = DateTime.Now; // Current date
                var remarks = (object)DBNull.Value;
                var approvedDate = (object)DBNull.Value; // Nullable column

                using (var db = new SqlConnection(connStr))
                {
                    db.Open();
                    using (var transaction = db.BeginTransaction())
                    {
                        try
                        {
                            // Update LOW_STOCK table
                            using (var updateCmd = db.CreateCommand())
                            {
                                updateCmd.Transaction = transaction;
                                updateCmd.CommandType = CommandType.Text;
                                updateCmd.CommandText = "UPDATE LOW_STOCK SET ORDER_QUANTITY = @quantity_ordered WHERE LOW_STOCK_ID = @low_stock_id";
                                updateCmd.Parameters.AddWithValue("@quantity_ordered", quantityOrdered);
                                updateCmd.Parameters.AddWithValue("@low_stock_id", lowStockId);
                                updateCmd.ExecuteNonQuery();
                            }

                            // Insert into PURCHASEORDER table
                            using (var insertCmd = db.CreateCommand())
                            {
                                insertCmd.Transaction = transaction;
                                insertCmd.CommandType = CommandType.Text;
                                insertCmd.CommandText = @"
                            INSERT INTO PURCHASEORDER 
                            (ORDER_DATE, REMARKS, APPROVED_DATE, SUPPLIER_ID, LOW_STOCK_ID, PRODUCT_ID) 
                            VALUES (@order_date, @remarks, @approved_date, @supplier_id, @low_stock_id, @product_id)";
                                insertCmd.Parameters.AddWithValue("@order_date", orderDate);
                                insertCmd.Parameters.AddWithValue("@remarks", remarks);
                                insertCmd.Parameters.AddWithValue("@approved_date", approvedDate);
                                insertCmd.Parameters.AddWithValue("@supplier_id", supplierId);
                                insertCmd.Parameters.AddWithValue("@low_stock_id", lowStockId);
                                insertCmd.Parameters.AddWithValue("@product_id", productId);
                                insertCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                data.Add(new { success = true, message = "Purchase confirmed successfully!" });
            }
            catch (Exception ex)
            {
                data.Add(new { success = false, message = ex.Message });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }



    }
}