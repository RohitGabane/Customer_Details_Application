using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Customer_Details_Application.Data;
using Customer_Details_Application.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Customer_Details_Application.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IConfiguration _configuration;

        public CustomersController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }



        public IActionResult CommunicationHistory(int customerId)
        {
            List<CommunicationHistory> communications = GetCommunicationHistory(customerId);
            return View(communications);
        }

        [HttpGet]
        public IActionResult AddCommunication(int customerId)
        {
            ViewData["CustomerId"] = customerId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCommunication([Bind("CustomerId,Type,Details")] CommunicationHistory communication)
        {
            if (ModelState.IsValid)
            {
                communication.CommunicationDate = DateTime.Now;
                AddCommunicationEntry(communication);
                return RedirectToAction("CommunicationHistory", new { customerId = communication.CustomerId });
            }

            return View(communication);
        }

        private List<CommunicationHistory> GetCommunicationHistory(int customerId)
        {
            List<CommunicationHistory> communications = new List<CommunicationHistory>();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();

                using (SqlCommand cmd = new SqlCommand("GetCommunicationHistory", sqlConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CustomerId", customerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CommunicationHistory communication = new CommunicationHistory
                            {
                                CommunicationId = Convert.ToInt32(reader["CommunicationId"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CommunicationDate = Convert.ToDateTime(reader["Timestamp"]),
                                CommunicationType = reader["Type"].ToString(),
                                CommunicationDetails = reader["Details"].ToString()

                            };

                            communications.Add(communication);
                        }
                    }
                }
            }

            return communications;
        }

        private void AddCommunicationEntry(CommunicationHistory communication)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();

                using (SqlCommand cmd = new SqlCommand("AddCommunicationEntry", sqlConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CustomerId", communication.CustomerId);
                    cmd.Parameters.AddWithValue("Timestamp", communication.CommunicationDate);
                    cmd.Parameters.AddWithValue("Type", communication.CommunicationType);
                    cmd.Parameters.AddWithValue("Details", communication.CommunicationDetails);


                    cmd.ExecuteNonQuery();
                }
            }
        }



        // GET: Customers
        public  IActionResult Index()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter cmd = new SqlDataAdapter("Customer_List", sqlConnection);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(dataTable);

            }
            return View(dataTable);
        }


        

        // GET: Customers/CreateOrEdit/
        public  IActionResult CreateOrEdit(int? id)
        {

            Customer customer = new Customer();
            if(id>0)
                customer=GetCustomerById(id);
            return View(customer);
        }

        // POST: Customers/CreateOrEdit/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrEdit(int id, [Bind("CustomerId,FirstName,LastName,Email,PhoneNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("CustomerCreateOrEdit", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("CustomerId", customer.CustomerId);
                    cmd.Parameters.AddWithValue("FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("Email", customer.Email);
                    cmd.Parameters.AddWithValue("PhoneNumber", customer.PhoneNumber);
                    SqlParameter errorCodeParam = new SqlParameter("@errorcode", SqlDbType.Int);
                    errorCodeParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(errorCodeParam);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        int errorCode = Convert.ToInt32(cmd.Parameters["@errorcode"].Value);

                        if (errorCode == 200)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else if (errorCode == 201)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else if (errorCode == 400)
                        {
                            ModelState.AddModelError(string.Empty, "Email or phone number already exists.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error occurred during operation. Please try again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "Error occurred during operation. Please try again.");
                    }
                }
            }

            return View(customer);
        }


        // GET: Customers/Delete/5
        public IActionResult Delete(int? id)
        {
            Customer customer = GetCustomerById(id);
            return View(customer);
        }
        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand("CustomerDeleteById", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("CustomerId", id);          
                cmd.ExecuteNonQuery();

            }
            return RedirectToAction(nameof(Index));
        }

        //nonAction only getting the data
       public Customer GetCustomerById(int? id) {
            Customer customer = new Customer();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dataTable = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter cmd = new SqlDataAdapter("CustomerViewById", sqlConnection);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("CustomerId", id);
                cmd.Fill(dataTable);

                if(dataTable.Rows.Count == 1) 
                {
                    customer.CustomerId = Convert.ToInt32(dataTable.Rows[0]["CustomerId"].ToString());
                    customer.FirstName = dataTable.Rows[0]["FirstName"].ToString();
                    customer.LastName = dataTable.Rows[0]["LastName"].ToString();
                    customer.Email = dataTable.Rows[0]["Email"].ToString();
                    customer.PhoneNumber = dataTable.Rows[0]["PhoneNumber"].ToString();

                }
                return customer;

            }
        }
    }




















}
