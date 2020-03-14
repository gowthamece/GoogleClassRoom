using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using GoogleClassRoomDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace GoogleClassRoomDemo.Controllers
{
    public class GoogleClassRoomController : Controller
    {
        private static readonly string ClientId = " [Provide client ID]";

        private static readonly string ClientSecret = "[Provide client secret]";

        private static readonly string GoogleApplicationName =
          "GoogleClass";

        public IActionResult Index()
        {
            return View();
        }
        public  bool IsSignedIn()
        {
            string refreshToken = GetRefershToken(1);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult SendInvite([FromBody]GoogleInvite param)
        {
            var Token = GetAuthenticator();
            var token = new TokenResponse()
            {
                AccessToken = Token,
                ExpiresInSeconds = 3600,
                IssuedUtc = DateTime.UtcNow
            };

            var authflow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                }
            });

            var credential = new UserCredential(authflow, "0", token);
            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = GoogleApplicationName,
                HttpClientInitializer = credential
            };

            var googleClassService = new ClassroomService(serviceInitializer);
            // string pageToken = null;

            foreach (var student in param.StudentList)
            {
                Invitation invitation = new Invitation();
                invitation.CourseId = param.CourseId;
                invitation.Role = "STUDENT";
                invitation.UserId = student.EmailID;
                var request = googleClassService.Invitations.Create(invitation);
                try
                {
                    var response = request.Execute();
                }
                catch (Exception ex)
                {
                    //TODO- Return the student List who are not invited
                }
            }

            return Json("");

        }
        public  List<Course> GetCourse()
        {
            var Token = GetAuthenticator();
            var token = new TokenResponse()
            {
                AccessToken = Token,
                ExpiresInSeconds = 3600,
                IssuedUtc = DateTime.UtcNow
            };

            var authflow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                }
            });

            var credential = new UserCredential(authflow, "0", token);
            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = GoogleApplicationName,
                HttpClientInitializer = credential
            };

            var googleService = new ClassroomService(serviceInitializer);

            string pageToken = null;
            var courses = new List<Course>();
            do
            {
                var request = googleService.Courses.List();
                request.PageSize = 100;
                request.PageToken = pageToken;
                var response = request.Execute();
                courses.AddRange(response.Courses);
                pageToken = response.NextPageToken;
            } while (pageToken != null);
            return courses;
        }

        public List<Google.Apis.Classroom.v1.Data.Student> GetStudents(string courseId)
        {
            var Token = GetAuthenticator();
            var token = new TokenResponse()
            {
                AccessToken = Token,
                ExpiresInSeconds = 3600,
                IssuedUtc = DateTime.UtcNow
            };

            var authflow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                }
            });

            var credential = new UserCredential(authflow, "0", token);
            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = GoogleApplicationName,
                HttpClientInitializer = credential
            };

            var googleService = new ClassroomService(serviceInitializer);

            string pageToken = null;
            var students = new List<Google.Apis.Classroom.v1.Data.Student>();
            do
            {
                var request = googleService.Courses.Students.List(courseId);
                request.PageSize = 100;
                request.PageToken = pageToken;
                var response = request.Execute();
                students.AddRange(response.Students);
                pageToken = response.NextPageToken;
            } while (pageToken != null);
            return students;
        }

      
        public List<CourseWork>  GetClassWork(string courseId)
        {
            var Token = GetAuthenticator();
            var token = new TokenResponse()
            {
                AccessToken = Token,
                ExpiresInSeconds = 3600,
                IssuedUtc = DateTime.UtcNow
            };

            var authflow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                }
            });

            var credential = new UserCredential(authflow, "0", token);
            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = GoogleApplicationName,
                HttpClientInitializer = credential
            };

            var googleClassService = new ClassroomService(serviceInitializer);
            string pageToken = null;
            var classWorkList = new List<CourseWork>();
            do
            {
                var request = googleClassService.Courses.CourseWork.List(courseId);
                request.PageSize = 100;
                request.PageToken = pageToken;
                var response = request.Execute();
                if (response.CourseWork != null)
                {
                    classWorkList.AddRange(response.CourseWork);
                }
                pageToken = response.NextPageToken;
            } while (pageToken != null);
            return classWorkList;
        }
        public List<StudentSubmission> GetClassWorkGrades(string courseId, string courseWorkId)
        {
            var Token = GetAuthenticator();
            var token = new TokenResponse()
            {
                AccessToken = Token,
                ExpiresInSeconds = 3600,
                IssuedUtc = DateTime.UtcNow
            };

            var authflow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                }
            });

            var credential = new UserCredential(authflow, "0", token);
            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = GoogleApplicationName,
                HttpClientInitializer = credential
            };

            var googleClassService = new ClassroomService(serviceInitializer);
            string pageToken = null;
            var classWorkGradeList = new List<StudentSubmission>();
            do
            {
                var request = googleClassService.Courses.CourseWork.StudentSubmissions.List(courseId, courseWorkId);
                request.PageSize = 100;
                request.PageToken = pageToken;
                var response = request.Execute();
                if (response.StudentSubmissions != null)
                {
                    classWorkGradeList.AddRange(response.StudentSubmissions);
                }
                pageToken = response.NextPageToken;
            } while (pageToken != null);
            return classWorkGradeList;
        }

        public static string GetAuthenticator()
            {
            string refreshToken = GetRefershToken(1);
            var content = "refresh_token=" + refreshToken + "&client_id=" + ClientId + "&client_secret=" + ClientSecret + "&grant_type=refresh_token";
            var request = WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseDataStream = response.GetResponseStream();
            var reader = new StreamReader(responseDataStream);
            var responseData = reader.ReadToEnd();
            reader.Close();
            responseDataStream.Close();
            string accessToken;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var returnedToken = JsonConvert.DeserializeObject<Token>(responseData);
                accessToken = returnedToken.Access_token;
            }

            else
            {
                return string.Empty;
            }
            response.Close();

            return accessToken;
        }
        [HttpPost]
        public  void GoogleAuthorizationCallback([FromBody]Token token)
        {
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-585QGBN;Initial Catalog=Test;Integrated Security=True");
            SqlCommand cmd = new SqlCommand("Stp_InsertGoogleAccessDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nvAuthCode", token.Auth_code);
            cmd.Parameters.AddWithValue("@nvRefreshToken", token.Refresh_token);
            cmd.Parameters.AddWithValue("@nvAccessCode", token.Access_token);
            cmd.Parameters.AddWithValue("@inUserID", 1);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();            
        }
        public static string GetRefershToken(int userId)
        {
            string refreshToken = string.Empty;
            SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-585QGBN;Initial Catalog=Test;Integrated Security=True");
            SqlCommand sqlCommand = new SqlCommand("Stp_GetGoogleUserInfo", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlCommand cmd = sqlCommand;
            cmd.Parameters.AddWithValue("@inUserID", userId);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                refreshToken = rdr["ReferehToken"].ToString();
            }

            return refreshToken;
        }

     

    }
}