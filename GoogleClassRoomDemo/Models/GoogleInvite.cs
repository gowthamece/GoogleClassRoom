using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleClassRoomDemo.Models
{
    public class GoogleInvite
    {
        public List<Student> StudentList { get; set; }
        public string CourseId { get; set; }
        
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string EmailID { get; set; }
    }
}
