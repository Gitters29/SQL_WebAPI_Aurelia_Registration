using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMSWebAPI.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Http.Cors;


namespace TMSWebAPI.Controllers
{
    [EnableCors(origins: "http://localhost:8080", headers: "*", methods: "*")]

    public class UserController : ApiController
    {

        private string conStr = "Data Source =(localdb)\\.\\IIS_DB; Initial Catalog=TMSDB; " +
            "uid=sa; pwd=123456";

        private string edisonConStr = "Data Source =(localdb)\\.\\IIS_DB; Initial Catalog=edison365; " +
            "uid=sa; pwd=123456";

        User[] user = new User[]
        {
            new User{UserID = 1, BandID = 3},
            new User{UserID = 2},
            new User{UserID = 3}
        };

        /* URI:
           <host IP>:<port>/api/user/<method name>
           or: <host IP>:<port>/api/user/ (For Get())
           <host IP>:<port>/api/user/1 (For GetUser()) */

        [HttpGet]
        [ActionName("GetAllPersons")]
        public HttpResponseMessage GetAllPersons()
        {
            SqlConnection connection = new SqlConnection(this.edisonConStr);
            connection.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [edison365].[dbo].[Person]" +
                "FOR JSON AUTO", connection);
            cmd.CommandType = CommandType.Text;

            StringBuilder jsonResult = new StringBuilder();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                if (!sdr.HasRows)
                {
                    jsonResult.Append("");
                }
                else
                {
                    while (sdr.Read())
                    {
                        jsonResult.Append(sdr.GetValue(0).ToString());
                    }
                }
                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResult.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
        }

        [HttpGet]
        [ActionName("GetPerson")]
        public HttpResponseMessage GetPerson()
        {
            // return user;
            string personID = "1";
            SqlConnection connection = new SqlConnection(this.conStr);
            connection.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP (1) PersonID FROM " +
                "[edison365].[dbo].[Person]" +
                "FOR JSON AUTO", connection);
            cmd.CommandType = CommandType.Text;

            //SqlDataReader sdr = cmd.ExecuteReader();
            StringBuilder jsonResult = new StringBuilder();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                if (!sdr.HasRows)
                {
                    jsonResult.Append("");
                }
                else
                {
                    while (sdr.Read())
                    {
                        jsonResult.Append(sdr.GetValue(0).ToString());
                    }
                }
                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResult.ToString(), 
                    Encoding.UTF8, "application/json");
                return response;
            }
        }

        //IEnumerable<User> jsonResult;
        [HttpGet]
        [ActionName("GetAllUsers")]
        public HttpResponseMessage GetAllUsers()
        {
            // return user;
            string userID = "1";
            SqlConnection connection = new SqlConnection(this.conStr);
            connection.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP (1) UserID FROM [TMSDB].[dbo].[User]" +
                "FOR JSON AUTO", connection);
            cmd.CommandType = CommandType.Text;

            //SqlDataReader sdr = cmd.ExecuteReader();
            StringBuilder jsonResult = new StringBuilder();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                if (!sdr.HasRows)
                {
                    jsonResult.Append("");
                }
                else
                {
                    while (sdr.Read())
                    {
                        jsonResult.Append(sdr.GetValue(0).ToString());
                    }
                }
                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonResult.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GetUser(int id) has only been setup for use with the local User array (user).
        // So you can ignore this method.
        [HttpGet]
        [ActionName("GetUser")]
        public IHttpActionResult GetUser(int id)
        {
            var userVar = user.FirstOrDefault((u) => u.UserID == id);
            if (userVar == null)
            {
                return NotFound();
            }

            return Ok(Url.Content("/~"));
        }

        [HttpPost]
        public IHttpActionResult PostPerson([FromBody] Person personItem)
        {
            int rowsAffected = int.MaxValue;

            // Persist Person data to database.
            using (SqlConnection sqlCon = new SqlConnection(this.edisonConStr))
            {
                string query = string.Empty;
                SqlCommand sqlCmd;

                try
                {
                    // Insert/Update into the Person table...
                    query = "EXEC [edison365].[dbo].[sproc_Person_Upsert]";
                    sqlCon.Open();
                    using (sqlCmd = new SqlCommand("[edison365].[dbo].[sproc_Person_Upsert]", 
                            sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        if (personItem.personID < 1)
                        {
                            personItem.personID = -1;
                        }

                        sqlCmd.Parameters.Add("@PersonID", SqlDbType.Int).Value =
                            personItem.personID;
                        sqlCmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value =
                            personItem.firstName;
                        sqlCmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value =
                            personItem.lastName;
                        sqlCmd.Parameters.Add("@Email", SqlDbType.VarChar).Value =
                            personItem.email;
                        sqlCmd.Parameters.Add("@MobileNumber", SqlDbType.VarChar).Value =
                            personItem.mobileNumber;
                        sqlCmd.Parameters.Add("@AddressLine1", SqlDbType.VarChar).Value =
                            personItem.addressline1;
                        sqlCmd.Parameters.Add("@AddressLine2", SqlDbType.VarChar).Value =
                                    personItem.addressline2;
                        sqlCmd.Parameters.Add("@AddressLine3", SqlDbType.VarChar).Value =
                            personItem.addressline3;
                        sqlCmd.Parameters.Add("@PostCode", SqlDbType.VarChar).Value =
                            personItem.postCode;

                        rowsAffected = sqlCmd.ExecuteNonQuery();

                        // Technically, this should return us the PersonID of the row that
                        // was just added, but it keeps returning NULL. It works for PostUser,
                        // just not for PostPerson.
                        //personItem.personID = (int)sqlCmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex);
                }
            }
            return Ok(personItem);
        }

        [HttpPost]
        public IHttpActionResult PostUser([FromBody] User userItem)
        {
            // Persist both user and questionnaire data to database
            using (SqlConnection sqlCon = new SqlConnection(this.conStr))
            {
                string query = string.Empty;
                SqlCommand sqlCmd;

                try
                {
                    // Insert into the User table first...
                    query = "EXEC [TMSDB].[dbo].[sproc_User_Upsert]";
                    sqlCon.Open();
                    using (sqlCmd = new SqlCommand("[TMSDB].[dbo].[sproc_User_Upsert]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        if (userItem.UserID < 1)
                        {
                            userItem.UserID = -1;
                        }

                        sqlCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userItem.UserID;
                        sqlCmd.Parameters.Add("@RoleID", SqlDbType.Int).Value = userItem.RoleID;
                        sqlCmd.Parameters.Add("@PracticeID", SqlDbType.Int).Value = userItem.PracticeID;
                        sqlCmd.Parameters.Add("@BandID", SqlDbType.Int).Value = userItem.BandID;

                        //int rowsAdded = sqlCmd.ExecuteNonQuery();
                        userItem.UserID = (int)sqlCmd.ExecuteScalar();

                        if (userItem.UserID > 0)
                        {
                            //MessageBox.Show("Row inserted!");
                        }
                        else
                        {
                            //MessageBox.Show("No row inserted");
                        }
                    }

                    //MessageBox.Show("UserID = " + userItem.RoleID);

                    // Insert the results of the questionnaire into the Results table...
                    //query = "INSERT INTO [TMSDB].[dbo].[Results] (UserID, QuestionID, AnswerID, TheatreID) " +
                    //        "VALUES (@UserID, @QuestionID, @AnswerID, @TheatreID)";
                    //sqlCon.Open();

                    //using (sqlCmd = new SqlCommand(query, sqlCon))
                    //{
                    //    sqlCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = iUserID;
                    //    sqlCmd.Parameters.Add("@QuestionID", SqlDbType.Int).Value = null;
                    //    sqlCmd.Parameters.Add("@AnswerID", SqlDbType.Int).Value = null;
                    //    sqlCmd.Parameters.Add("@TheatreID", SqlDbType.Int).Value = iTheatreID;
                    //    for (int i = 0; i < iaRbsChecked.Length; i++)
                    //    {
                    //        sqlCmd.Parameters["@QuestionID"].Value = (i + 1);
                    //        sqlCmd.Parameters["@AnswerID"].Value = iaRbsChecked[i];

                    //        int rowsAdded = sqlCmd.ExecuteNonQuery();
                    //        if (rowsAdded > 0)
                    //        {
                    //            //ClientScript.RegisterStartupScript(GetType(), "hwa", "alert('Row inserted');", true);
                    //            //MessageBox.Show("Row inserted!");
                    //        }
                    //        else
                    //        {
                    //            //ClientScript.RegisterStartupScript(GetType(), "hwa", "alert('Row not inserted');", true);
                    //            //MessageBox.Show("No row inserted");
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    //string jsFunc = "alert(" + ex.Message + ")";
                    //string myString = ex.Message;
                    //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "myJsFn", jsFunc, true);
                    //ClientScript.RegisterStartupScript(GetType(), "hwa", "alert('Error message: ' + '" + myString + "');", true);
                    //MessageBox.Show("Error: " + ex.Message);
                    //Response.Write("<script language='javascript'>alert('" +
                    //Server.HtmlEncode(ex.Message) + "')</script>");
                    //var message = new JavaScriptSerializer().Serialize(ex.Message.ToString());
                    //var script = string.Format("alert({0});", message);
                    //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "", script, true);
                }
            }


            User myUser = new User();
            myUser.UserID = 5;
            //user[user.Length] = userItem;
            return Ok(userItem);
        }

        //// Note: You CAN'T DELETE via browser URL. You MUST use Fiddler/Postman and set
        //// the method to DELETE.
        //[HttpDelete]
        //[Route("{userID:int}")]
        //[AcceptVerbs("DELETE")]
        //public IHttpActionResult DeleteUser(int userID)
        //{
        //    if (userID < 0)
        //    {
        //        return BadRequest("Not a valid User ID");
        //    }
                
        //    int rowDeleted = -1;

        //    // Delete user from database
        //    using (SqlConnection sqlCon = new SqlConnection(this.conStr))
        //    {
        //        //string query = string.Empty;
        //        SqlCommand sqlCmd;
        //        try
        //        {
        //            //query = "EXEC [TMSDB].[dbo].[sproc_User_Delete]";
        //            sqlCon.Open();

        //            using (sqlCmd = new SqlCommand("[TMSDB].[dbo].[sproc_User_Delete]", sqlCon))
        //            {
        //                sqlCmd.CommandType = CommandType.StoredProcedure;

        //                sqlCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

        //                rowDeleted = sqlCmd.ExecuteNonQuery();

        //                if (rowDeleted > 0)
        //                {
        //                    return Ok(rowDeleted);
        //                    //return (IHttpActionResult)Request.CreateResponse
        //                    //                         (HttpStatusCode.OK);
        //                    //MessageBox.Show("Row inserted!");
        //                }
        //                else
        //                {
        //                    return Content(HttpStatusCode.NotFound, "User with ID " + 
        //                        userID.ToString() +
        //                         " not found.");
        //                    //MessageBox.Show("No row inserted");

        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return Content(HttpStatusCode.BadRequest, ex);
        //        }

                
        //    }

            

        //}



        // Note: You CAN'T DELETE via browser URL. You MUST use Fiddler/Postman and set
        // the method to DELETE. URL: <HostURL>/<personID>
        [HttpDelete]
        [Route("{personID:int}")]
        [AcceptVerbs("DELETE")]
        public IHttpActionResult DeletePerson(int personID)
        {
            if (personID < 0)
            {
                return BadRequest("Not a valid Person ID");
            }

            int rowDeleted = -1;

            // Delete person from database
            using (SqlConnection sqlCon = new SqlConnection(this.edisonConStr))
            {
                SqlCommand sqlCmd;
                try
                {
                    sqlCon.Open();

                    using (sqlCmd = new SqlCommand("[edison365].[dbo].[sproc_Person_Delete]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personID;

                        rowDeleted = sqlCmd.ExecuteNonQuery();

                        if (rowDeleted > 0)
                        {
                            return Ok(rowDeleted);
                        }
                        else
                        {
                            return Content(HttpStatusCode.NotFound, "Person with ID " +
                                personID.ToString() +
                                 " not found.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex);
                }
            }
        }
    }
}
